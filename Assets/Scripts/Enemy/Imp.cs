using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(gameObject + " took damage" + damage);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
