using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Author: nithin.s
/// Manages player spawning, room events, countdown, and scene transitions.
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    GameObject go;
    Vector3 pos;
    [SerializeField] private Text timeText;

    public GameObject[] countDown;

    /// <summary>
    /// Maps a player's ActorNumber to a deterministic spawn X offset.
    /// Uses ActorNumber instead of PlayerCount to avoid race conditions.
    /// </summary>
    private float GetSpawnXForActor(int actorNumber)
    {
        // ActorNumbers start at 1 in Photon
        switch (actorNumber)
        {
            case 1: return 2f;
            case 2: return 0f;
            case 3: return -2f;
            default: return -2f * (actorNumber - 1);
        }
    }

    private PhotonView pv;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // Ensure a PhotonView exists on this GameObject for RPCs
        pv = GetComponent<PhotonView>();
        if (pv == null)
        {
            pv = gameObject.AddComponent<PhotonView>();
            // No observed components needed for this manager
        }
    }

    void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("GameManager.Start() called but not in a Photon room. Returning to lobby.");
            SceneManager.LoadScene(0);
            return;
        }

        // Use ActorNumber for deterministic, race-free spawn positioning
        pos.x = GetSpawnXForActor(PhotonNetwork.LocalPlayer.ActorNumber);
        pos.y = playerPrefab.transform.position.y;

        if (playerPrefab == null)
        {
            Debug.LogError("GameManager.Start: playerPrefab is not assigned in the Inspector.");
            return;
        }
        if (BallControl.localPlayerInstance == null)
        {
            // Use the computed pos instead of the prefab's transform position
            go = PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity, 0);
        }
        else
        {
            Debug.Log("Player instance already exists");
        }

        // Start countdown — MasterClient triggers it for synchronization
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            pv.RPC("RPC_StartCountdown", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_StartCountdown()
    {
        StartCoroutine(TimeCount());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"Player joined: {newPlayer.NickName} (ActorNumber: {newPlayer.ActorNumber})");

            // Clamp the scene index to valid build range (0 = Main, 1 = Room1, 2 = Room2)
            int targetScene = Mathf.Clamp(PhotonNetwork.CurrentRoom.PlayerCount, 1, 2);
            PhotonNetwork.LoadLevel(targetScene);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"Player left: {otherPlayer.NickName}");

            int targetScene = Mathf.Clamp(PhotonNetwork.CurrentRoom.PlayerCount, 1, 2);
            PhotonNetwork.LoadLevel(targetScene);
        }
    }

    public void Leave()
    {
        Debug.Log("Leaving room...");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // Destroy the persistent ball instance to prevent ghost objects in the menu
        if (BallControl.localPlayerInstance != null)
        {
            Destroy(BallControl.localPlayerInstance);
            BallControl.localPlayerInstance = null;
        }

        SceneManager.LoadScene(0);
    }


    IEnumerator TimeCount()
    {
        if (countDown != null && countDown.Length >= 3)
        {
            countDown[0].SetActive(true);
            yield return new WaitForSeconds(1f);
            countDown[0].SetActive(false);
            countDown[1].SetActive(true);
            yield return new WaitForSeconds(1f);
            countDown[1].SetActive(false);
            countDown[2].SetActive(true);
            yield return new WaitForSeconds(1f);
            countDown[2].SetActive(false);
        }

        while (true)
        {
            if (timeText != null)
                timeText.text = Time.timeSinceLevelLoad.ToString("F1");
            yield return null;
        }
    }

}
