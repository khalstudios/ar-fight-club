using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ARFighterGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject infoPanelUIGameObject;
    public TextMeshProUGUI informText;
    public GameObject searchForGamesButtonGameObject;
    // Start is called before the first frame update
    void Start()
    {
        infoPanelUIGameObject.SetActive(true);
        informText.text = "Search for Games to Battle!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinRandomRoom() {
        Debug.Log("Trying to join random room");
        informText.text = "Searching for available rooms...";
        PhotonNetwork.JoinRandomRoom();
        searchForGamesButtonGameObject.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log(message);
        informText.text = message;
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom() {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 0) {
            Debug.Log("Joined to " + PhotonNetwork.CurrentRoom.Name + " Waiting for other players to join...");
            informText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + " Waiting for other players to join...";
        } else {
            Debug.Log("Joined to " + PhotonNetwork.CurrentRoom.Name);
            informText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(infoPanelUIGameObject, 2.0f));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        informText.text = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount;

        StartCoroutine(DeactivateAfterSeconds(infoPanelUIGameObject, 2.0f));
    }

    public void CreateAndJoinRoom() {
        string randomRoomName = "Room" + Random.Range(0,1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds) {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }
}
