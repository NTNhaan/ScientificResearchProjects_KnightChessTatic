using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    public override void Attack(Character target)
    {
        target.TakeHit(attack);
    }

    public override void Dead()
    {
        //play Dead animation;
    }

    public override void TakeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Dead();
        }
    }

}
