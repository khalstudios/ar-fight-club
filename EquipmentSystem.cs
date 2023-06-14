using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [SerializeField] private GameObject weapon;
    
    public void StartDealDamage()
    {
        weapon.GetComponentInChildren<DamageDealer>().StartDealDamage();
    }
    public void EndDealDamage()
    {
        weapon.GetComponentInChildren<DamageDealer>().EndDealDamage();
    }
}
