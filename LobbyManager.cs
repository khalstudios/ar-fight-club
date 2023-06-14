using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject uiLoginGameObject;

    [Header("Lobby UI")]
    public GameObject uiLobbyGameObject;
    public GameObject ui3DGameObject;

    [Header("Connection Status UI")]
    public GameObject uiConnectionStatusGameObject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;

    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        // Activating only Lobby UI
        if (PhotonNetwork.IsConnected) {
            uiLobbyGameObject.SetActive(true);
            ui3DGameObject.SetActive(true);

            uiConnectionStatusGameObject.SetActive(false);
            uiLoginGameObject.SetActive(false);
        } else {
            uiLobbyGameObject.SetActive(false);
            ui3DGameObject.SetActive(false);
            uiConnectionStatusGameObject.SetActive(false);

            uiLoginGameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showConnectionStatus) {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    
    }

    public void OnEnterGameButtonClicked() {
        string playerName = playerNameInputField.text;
        if (!string.IsNullOrEmpty(playerName)) {
            uiLobbyGameObject.SetActive(false);
            ui3DGameObject.SetActive(false);
            uiLoginGameObject.SetActive(false);

            showConnectionStatus = true;
            uiConnectionStatusGameObject.SetActive(true);

            if (!PhotonNetwork.IsConnected) {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        } else {
            Debug.Log("Player name is invalid or empty!");
        }
    }

    public void OnQuickMatchButtonClicked() {
        //SceneManager.LoadScene("LoadingScene");
        SceneLoader.Instance.LoadScene("PlayerSelectionScene");
    }

    #endregion


    #region PHOTON Callback Methods
    public override void OnConnected() {
        Debug.Log("We have connected to the Internet.");
    }

    public override void OnConnectedToMaster() {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server.");
        uiLobbyGameObject.SetActive(true);
        ui3DGameObject.SetActive(true);
        uiLoginGameObject.SetActive(false);
        uiConnectionStatusGameObject.SetActive(false);
    }
    #endregion

}