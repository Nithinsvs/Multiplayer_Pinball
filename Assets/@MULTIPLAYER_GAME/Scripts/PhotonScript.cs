using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Author: nithin.s
/// Handles Photon connection, room join/create, and scene loading for the lobby.
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

        if (startButton != null) startButton.SetActive(false);
        if (loadingText != null) loadingText.SetActive(true);

        // If already connected (e.g. returning from a game), skip straight to joining
        if (PhotonNetwork.IsConnected)
        {
            isConnecting = true;
            PhotonNetwork.JoinRandomRoom();
            return;
        }

        // GameVersion MUST be set before ConnectUsingSettings
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.SendRate = 20;         // how often messages are sent (Hz)
        PhotonNetwork.SerializationRate = 10; // how often OnPhotonSerializeView runs

        isConnecting = PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        if (isConnecting)
            PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");

        // Reset UI so user can retry
        isConnecting = false;
        if (startButton != null) startButton.SetActive(true);
        if (loadingText != null) loadingText.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined");
        isConnecting = false;

        // With AutomaticallySyncScene = true, only MasterClient should call LoadLevel
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}