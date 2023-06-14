using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthSystem : MonoBehaviourPunCallbacks, IPunObservable
{
    public int health = 100;
    [SerializeField] GameObject hitVFX;

    private bool isDead = false;
    private Animator animator;
    private PhotonView photonView;

    private static readonly int TakeDamageAnimation = Animator.StringToHash("Take Damage");
    private static readonly int DieAnimation = Animator.StringToHash("Die");

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (!photonView.IsMine)
            return;

        health -= damageAmount;
        animator.SetTrigger(TakeDamageAnimation);

        if (health <= 0 && !isDead)
        {
            Debug.Log("Dying");
            isDead = true;
            photonView.RPC("Die", RpcTarget.All);
        }

        photonView.RPC("UpdateHealth", RpcTarget.All, health);
        photonView.RPC("TriggerTakeDamageAnimation", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateHealth(int value)
    {
        health = value;
        // Perform any necessary health-related animations or logic
    }

    [PunRPC]
    private void TriggerTakeDamageAnimation()
    {
        animator.SetTrigger(TakeDamageAnimation);
    }

    [PunRPC]
    private void Die()
    {
        Debug.Log("Dying");
        animator.SetTrigger(DieAnimation);
        PhotonNetwork.Destroy(gameObject);
    }

    public void HitVFX(Vector3 hitPosition)
    {
        GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        Destroy(hit, 3f);
        photonView.RPC("TriggerHitVFX", RpcTarget.All, hitPosition);
    }

    [PunRPC]
    private void TriggerHitVFX(Vector3 hitPosition)
    {
        GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        Destroy(hit, 3f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
        }
    }
}