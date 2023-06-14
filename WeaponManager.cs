using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("Collided");
            Health health = collision.gameObject.GetComponent<Health>();
            health.TakeDamage();
        }
    }
}
