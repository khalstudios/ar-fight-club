using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
 
public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealtDamage;
 
    [SerializeField] float weaponLength;
    [SerializeField] int weaponDamage;
    
    PhotonView photonView;

    void Awake() 
    {
        photonView = GetComponentInParent<PhotonView>();
    }

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }
 
    void Update()
    {
        if (canDealDamage && photonView.IsMine)
        {
            RaycastHit hit;
 
            //int layerMask = 1 << 9;
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength))
            {
                if (hit.transform.TryGetComponent(out PhotonView view)) 
                {
                    if (!view.IsMine && hit.transform.TryGetComponent(out HealthSystem enemy) && !hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        enemy.TakeDamage(weaponDamage);
                        enemy.HitVFX(hit.point);
                        hasDealtDamage.Add(hit.transform.gameObject);
                    }
                }
                
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }
 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}