using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    public Slider HealthSlider;
    public Slider EaseHealthSilder;
    public float attack;
    public float health;
    public float MaxHealth = 100f;
    private float LerpSpeed = 0.01f;
    public abstract void Attack(Character target);
    public abstract void TakeHit(float damage);
    public abstract void Dead();
    public abstract void Health(float mount);
    public void Start()
    {
        health = MaxHealth;
    }
    public void Update()
    {
        if (HealthSlider.value != health)
        {
            HealthSlider.value = health;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        if (HealthSlider.value != EaseHealthSilder.value)
        {
            EaseHealthSilder.value = Mathf.Lerp(EaseHealthSilder.value, health, LerpSpeed);
        }
    }
    void TakeDamage(float damege)
    {
        health -= damege;
    }
}
