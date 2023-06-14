using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage()
    {
        health -= health * 0.1f; // Reduce health by 10%
    }
}
