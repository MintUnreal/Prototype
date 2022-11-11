using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks 
{
    [SerializeField] private InputManager InputManager;
    [SerializeField] private GameObject PlayerPrefab;

    private PhotonView LocalPhotonView;
    public List<Character> Players;

    public static NetworkManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameObject NewPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.zero, Quaternion.identity);
        LocalPhotonView = NewPlayer.GetComponent<PhotonView>();
        InputManager.ConnectedCharacter = NewPlayer.GetComponent<Character>();
        InputManager.ConnectedCharacter.InputManager = InputManager;
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(newPlayer.NickName + "entered room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + "left room");
    }
}
