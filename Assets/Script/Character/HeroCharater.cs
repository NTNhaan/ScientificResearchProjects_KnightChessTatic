using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCharater : Character
{
    private Animator animator;
    public override void Attack(Character target)
    {
        var atm = target.GetComponent<Character>();
        animator = GetComponent<Animator>();
        if (atm != null)
        {
            atm.TakeHit(attack);
            animator.SetTrigger("Attack3");
        }
    }
    public override void TakeHit(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        if (health <= 0)
        {
            Dead();
        }
    }
    public override void Dead()
    {
        //play Dead animation;
    }
    public override void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
}
