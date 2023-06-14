using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{

    public Transform playerSwitcherTransform;

    public Button nextButton;
    public Button previousButton;

    public int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI playerModelTypeText;

    public GameObject[] playerModels;

    public GameObject ui_Selection;
    public GameObject ui_AfterSelection;

    // Start is called before the first frame update
    void Start()
    {
        ui_Selection.SetActive(true);
        ui_AfterSelection.SetActive(false);
        playerSelectionNumber = 0;
        playerModelTypeText.text = "Yellow";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextPlayer() {
        playerSelectionNumber += 1;

        if (playerSelectionNumber >= playerModels.Length) {
            playerSelectionNumber = 0;
        }

        Debug.Log(playerSelectionNumber);
        nextButton.enabled = false;
        previousButton.enabled = false;
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));

        if (playerSelectionNumber == 0) {
            playerModelTypeText.text = "Yellow";
        } else if(playerSelectionNumber == 1) {
            playerModelTypeText.text = "Blue";
        } else if(playerSelectionNumber == 2) {
            playerModelTypeText.text = "Brown";
        } else {
            playerModelTypeText.text = "Green";
        }
    }

    public void PreviousPlayer() {
        playerSelectionNumber -= 1;

        if (playerSelectionNumber < 0) {
            playerSelectionNumber = playerModels.Length - 1;
        }

        Debug.Log(playerSelectionNumber);
        nextButton.enabled = false;
        previousButton.enabled = false;
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));

        if (playerSelectionNumber == 0) {
            playerModelTypeText.text = "Yellow";
        } else if(playerSelectionNumber == 1) {
            playerModelTypeText.text = "Blue";
        } else if(playerSelectionNumber == 2) {
            playerModelTypeText.text = "Brown";
        } else {
            playerModelTypeText.text = "Green";
        }
    }

    public void OnSelectButtonClicked() {
        ui_Selection.SetActive(false);
        ui_AfterSelection.SetActive(true);
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARFighterGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnReSelectButtonClicked() {
        ui_Selection.SetActive(true);
        ui_AfterSelection.SetActive(false);
    }

    public void OnBattleButtonClicked() {
        SceneLoader.Instance.LoadScene("SampleScene");
    }

    public void OnBackButtonClicked() {
        SceneLoader.Instance.LoadScene("LobbyScene");
    }

    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f) {

        Quaternion originalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation*Quaternion.Euler(axis*angle);

        float elapsedTime = 0.0f;
        while (elapsedTime < duration) {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transformToRotate.rotation = finalRotation;
        nextButton.enabled = true;
        previousButton.enabled = true;
    }
}
