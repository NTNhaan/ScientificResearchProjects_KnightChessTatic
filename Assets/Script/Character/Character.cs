using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public int health;
    public int attack;

    public abstract void Attack(Character target);
    public abstract void TakeHit(int damage);
    public abstract void Dead();
    public void Health(int mount)
    {
        health += mount;
    }
}
