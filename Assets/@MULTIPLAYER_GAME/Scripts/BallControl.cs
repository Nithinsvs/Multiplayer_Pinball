using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Author: nithin.s
/// Controls player ball movement, physics, and particle effects over Photon network.
/// </summary>
public class BallControl : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject localPlayerInstance;

    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float ballFallingSpeed = 2f;
    [Range(0.2f, 2f)] public float gravityModifier = 0.5f;
    public GameObject particle;

    Rigidbody rb;
    float _horizontalInput;
    float _verticalInput;

    // Network sync for remote players
    private Vector3 _networkPosition;
    private Quaternion _networkRotation;
    private bool _isFalling = false;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            BallControl.localPlayerInstance = this.gameObject;

            // Set network rates only once (from local player)
            PhotonNetwork.SendRate = 20;
            PhotonNetwork.SerializationRate = 10;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError($"[BallControl] No Rigidbody found on {gameObject.name}!", this);
        }

        _networkPosition = transform.position;
        _networkRotation = transform.rotation;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }
        else
        {
            // Smooth interpolation for remote players
            transform.position = Vector3.Lerp(transform.position, _networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _networkRotation, Time.deltaTime * 10f);
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || rb == null || _isFalling) return;

        // Apply input as a force using movementSpeed, with velocity clamping
        Vector3 inputForce = new Vector3(_horizontalInput, 0f, _verticalInput) * movementSpeed;
        rb.AddForce(inputForce, ForceMode.Acceleration);

        // Clamp velocity to prevent runaway acceleration
        Vector3 clampedVelocity = rb.velocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
        clampedVelocity.z = Mathf.Clamp(clampedVelocity.z, -maxSpeed, maxSpeed);
        clampedVelocity.y = 0f;
        rb.velocity = clampedVelocity;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (photonView.IsMine)
        {
            if (coll.CompareTag("Finish"))
            {
                StartCoroutine(FallDown());
            }
        }
    }

    IEnumerator FallDown()
    {
        _isFalling = true;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = true;
        }

        Debug.Log("Ball falling — reducing size");

        float elapsed = 0f;
        float fallDuration = 3f; // Fall for 3 seconds then destroy

        while (elapsed < fallDuration)
        {
            if (rb != null)
            {
                rb.velocity = Vector3.down * ballFallingSpeed;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Clean up after falling
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    #region NETWORK SERIALIZATION
    /// <summary>
    /// Sync position and rotation to remote players via PhotonView.
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb != null ? rb.velocity : Vector3.zero);
        }
        else
        {
            // Network player: receive data
            _networkPosition = (Vector3)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();
            Vector3 velocity = (Vector3)stream.ReceiveNext();

            if (rb != null)
            {
                rb.velocity = velocity;
            }

            // Compensate for network lag
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            _networkPosition += velocity * lag;
        }
    }
    #endregion

    #region PARTICLE SYSTEM RPC
    [PunRPC]
    public void Shoot()
    {
        if (particle == null)
        {
            Debug.LogWarning("[BallControl] Particle prefab not assigned!", this);
            return;
        }

        // Use ball's actual position, not the uninitialized field
        Vector3 spawnPos = transform.position;
        spawnPos.y = 0f;
        PhotonNetwork.Instantiate(particle.name, spawnPos, Quaternion.identity);
    }
    #endregion

    private void OnDestroy()
    {
        // Clear the static reference if this is the local instance being destroyed
        if (localPlayerInstance == this.gameObject)
        {
            localPlayerInstance = null;
        }
    }
}