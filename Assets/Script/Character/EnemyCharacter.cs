using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private Animator animator;
    public void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public override void Attack(Character target)
    {
        var atm = target.GetComponent<Character>();
        if (atm != null)
        {
            animator.SetTrigger("Attack1");
            EndAnimation(animator);
            atm.TakeHit(attack);
        }
    }
    private void EndAnimation(Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }
    public override void Dead()
    {
        animator.SetTrigger("Death");
        EndAnimation(animator);
    }
    public override void TakeHit(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        animator.SetTrigger("Hurt");
        if (health <= 0)
        {
            Dead();
        }
    }
    public override void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

}
