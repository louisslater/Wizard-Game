using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public int attackDamage = 30;

    public bool canAttack = true;
    public bool attacking = false;
    public float attackCooldown = 1.0f;
    public AudioClip weaponAttackSound;

    public override void Use()
    {
        if (canAttack)
        {
            canAttack = false;
            attacking = true;
            Animator animator = itemGameObject.GetComponent<Animator>();
            animator.SetTrigger("Attack");
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(weaponAttackSound);
            StartCoroutine(ResetAttackCooldown());
        }
    }

    public void OnTriggerEnter(Collider enemy)
    {
        if(attacking)
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
    }


    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
        canAttack = true;
    }

}
