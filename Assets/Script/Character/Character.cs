using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    public float attack;
    [HideInInspector] public float health;
    [HideInInspector] public float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image fontHealthBar;
    public Image backHealthBar;
    public abstract void Attack(Character target);
    public abstract void TakeHit(float damage);
    public abstract void Dead();
    public abstract void RestoreHealth(float mount);
    public void Start()
    {
        health = maxHealth;
    }
    public void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        float fillFont = fontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillBack > hFraction)
        {
            fontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }
        else
        {
            backHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            fontHealthBar.fillAmount = Mathf.Lerp(fillFont, hFraction, percentComplete);
        }
    }
}
