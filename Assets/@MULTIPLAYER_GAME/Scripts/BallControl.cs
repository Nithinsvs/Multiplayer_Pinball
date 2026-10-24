using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using Photon.Pun.UtilityScripts;

/// <summary>
/// Author: nithin.s
/// </summary>
public class BallControl : MonoBehaviourPunCallbacks
{
    public static GameObject localPlayerInstance;

    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float ballFallingSpeed;
    [Range(0.2f, 2f)] public float gravityModifier = 0.5f;
    public GameObject particle;


    Rigidbody rb;
    Vector3 pos;
    GameObject go;
    float dirx;
    float dirz;
    float _horizontalInput;
    float _verticalInput;

    //[SerializeField] GameObject finish;


    Vector3 startPos;
    Vector3 lastPos;
    int direction = 0;


    void OnGUI()
    {
        if (!PhotonNetwork.IsConnected) return;

        // Get stats
        var peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
        var stats = peer.TrafficStatsGameLevel;

        Debug.Log($"transform.position = {transform.position}");
        Debug.Log($"bytes sent = {stats.TotalOutgoingByteCount}");
    }


    private void Awake()
    {
        PhotonNetwork.SendRate = 20;         // how often messages are sent (Hz)
        PhotonNetwork.SerializationRate = 10; // how often OnPhotonSerializeView runs

        if (photonView.IsMine)
        {
            BallControl.localPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }


        /*if (photonView.IsMine)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("Touch began");
                    startPos = touch.position;
                    lastPos = touch.position;
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    Debug.Log("Touch moving");
                    lastPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Debug.Log("Touch ended");
                    lastPos = touch.position;

                    if (Mathf.Abs(lastPos.x - startPos.x) > Mathf.Abs(lastPos.y - startPos.y))
                    {
                        Debug.Log("Moved in X axis");
                        if (lastPos.x - startPos.x > 0)
                        {
                            Debug.Log("Moved in right");
                            if (direction != 2)
                                direction = 1;
                        }
                        else if (lastPos.x - startPos.x < 0)
                        {
                            Debug.Log("Moved in left");
                            if (direction != 1)
                                direction = 2;
                        }
                    }
                    else if (Mathf.Abs(lastPos.x - startPos.x) < Mathf.Abs(lastPos.y - startPos.y))
                    {
                        Debug.Log("Moved in Y axis");
                        if (lastPos.y - startPos.y > 0)
                        {
                            Debug.Log("Moved in up");
                            if (direction != 4)
                                direction = 3;
                        }
                        else if (lastPos.y - startPos.y < 0)
                        {
                            Debug.Log("Moved in down");
                            if (direction != 3)
                                direction = 4;
                        }
                    }
                }
            }
        }*/


        /*if (photonView.IsMine)
        {
            dirx = Input.acceleration.x * gravityModifier;
            dirz = Input.acceleration.y * gravityModifier;

            if (Input.GetMouseButtonDown(0))
            {
                photonView.RPC("Shoot", RpcTarget.Others);
            }                     
        }*/

    }
    private void FixedUpdate()
    {

        if (photonView.IsMine)
        {
            /*if (direction == 1)
                rb.velocity = Vector3.right * Time.fixedDeltaTime * movementSpeed;
            else if (direction == 2)
                rb.velocity = Vector3.left * Time.fixedDeltaTime * movementSpeed;
            else if (direction == 3)
                rb.velocity = Vector3.forward * Time.fixedDeltaTime * movementSpeed;
            else if (direction == 4)
                rb.velocity = Vector3.back * Time.fixedDeltaTime * movementSpeed;*/

            rb.velocity = new Vector3(rb.velocity.x + _horizontalInput, 0f, rb.velocity.z + _verticalInput);
            //Debug.Log(transform.position);
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (photonView.IsMine)
        {
            if (coll.gameObject.tag == "Finish")
            {
                StartCoroutine(FallDown());
                //finish.SetActive(true);
            }
        }
    }
    IEnumerator FallDown()
    {
        GetComponent<Animator>().enabled = true;
        print("reducing size");
        while (true)
        {
            print(transform.position);
            rb.velocity = Vector3.zero * ballFallingSpeed;
            yield return null;
        }
    }

    #region PARTICLE SYSTEM RPC
    [PunRPC]
    public void Shoot()
    {
        pos.y = 0f;
        PhotonNetwork.Instantiate(particle.name, pos, Quaternion.identity);
    }
    #endregion

}