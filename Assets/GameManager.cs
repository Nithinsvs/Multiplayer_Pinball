using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    GameObject go;
    Vector3 pos;
    [SerializeField] private Text timeText;

    public GameObject[] countDown;


    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            pos.x = 2f;
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            pos.x = 0f;
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            pos.x = -2f;


        if (BallControl.localPlayerInstance == null)
        {
            go = PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position, Quaternion.identity, 0);
        }
        else
            Debug.Log("Player instance already exists");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            StartCoroutine(TimeCount());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print("Player joined");
            PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print("Player left");
            PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }
    public void Leave()
    {
        print("I'm leaving....gud bye");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    IEnumerator TimeCount()
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

        while (true)
        {
            timeText.text = Time.timeSinceLevelLoad.ToString();
            yield return null;
        }
    }

}
