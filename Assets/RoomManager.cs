using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    [Space]
    [SerializeField]
    public Transform spawnPoint;


    void Start()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("Test", null, null);

        Debug.Log("We're Connect to a room ");


    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log($"We're Connect to a room {PhotonNetwork.CurrentRoom.Name}");
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.transform.parent = spawnPoint;
        VFXManager.Instance.InitVFX();
    }
}
