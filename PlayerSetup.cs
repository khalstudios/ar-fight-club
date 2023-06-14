using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public TMP_Text playerNameText;
    public GameObject canvas; 

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine) {
            //The player is the local player
            transform.GetComponent<PlayerMovement>().enabled = true;
            canvas.SetActive(true);
            transform.GetComponent<PlayerMovement>()._joystick.gameObject.SetActive(true);
        } else {
            // The player is remote player
            transform.GetComponent<PlayerMovement>().enabled = false;
            canvas.SetActive(false);
            transform.GetComponent<PlayerMovement>()._joystick.gameObject.SetActive(false);
        }
        SetPlayerName();
    }

    void SetPlayerName()
    {

        if (playerNameText != null)
        {
            if (photonView.IsMine)
            {
                playerNameText.text = "YOU";
                playerNameText.color = Color.red;
            }
            else
            {
                playerNameText.text = photonView.Owner.NickName;

            }

        }

    }
}
