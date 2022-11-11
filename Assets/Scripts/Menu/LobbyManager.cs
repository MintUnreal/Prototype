using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField NicknameField;

    void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(0, 100);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }
    public void Play()
    {
        if (NicknameField.text != string.Empty)
        {
            PhotonNetwork.NickName = NicknameField.text;
            PhotonNetwork.JoinRandomOrCreateRoom();
        }
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master Server");
    }

    public void CreateRoom()
    {
        Debug.Log("Creating room...");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 10 });
    }

    public void JoinRoom()
    {
        Debug.Log("Start joining...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room joined!");
        PhotonNetwork.LoadLevel(1);
    }

}
