using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
 
public class HealthSystem : MonoBehaviour, IPunObservable
{
    public int health = 100;
    [SerializeField] GameObject hitVFX;
    [SerializeField] float destroyDelay = 2.5f;
 
    Animator animator;
    PhotonView photonView;

    void Awake() {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
 
    public void TakeDamage(int damageAmount)
    {
        if (health == 0)
            return;

        Debug.Log("Took Hit");
        health -= damageAmount;
        animator.SetTrigger("Take Damage");
        photonView.RPC("UpdateHealth", RpcTarget.All, health);
 
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    private void UpdateHealth(int value)
    {
        health = value;
        animator.SetTrigger("Take Damage");
        // Perform any necessary health-related animations or logic
    }
 
    [PunRPC]
    private void Die()
    {
        StartCoroutine(DestroyAfterDelayRoutine());
    }

    IEnumerator DestroyAfterDelayRoutine()
    {
        yield return new WaitForSeconds(destroyDelay);
        animator.SetTrigger("Die");
        PhotonNetwork.Destroy(this.gameObject);
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
