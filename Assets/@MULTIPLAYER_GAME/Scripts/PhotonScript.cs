using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Author: nithin.s
/// </summary>
public class PhotonScript : MonoBehaviourPunCallbacks
{
    bool isConnecting;
    [SerializeField] GameObject loadingText = null;
    [SerializeField] GameObject startButton = null;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public void Connect()
    {
        print("Clicked on start");
        isConnecting = PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.SendRate = 20;         // how often messages are sent (Hz)
        PhotonNetwork.SerializationRate = 10; // how often OnPhotonSerializeView runs


        startButton.SetActive(false);
        loadingText.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        if (isConnecting)
            PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined");
        PhotonNetwork.LoadLevel(1);
    }
}