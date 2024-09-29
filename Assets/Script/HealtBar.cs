using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealtBar : MonoBehaviour
{
    public Slider HealthSlider;
    public Slider EaseHealthSilder;
    public float MaxHealth = 100f;
    public float health;
    private float LerpSpeed = 0.01f;
    public void Start()
    {
        health = MaxHealth;        
    }

    // Update is called once per frame
    public void Update()
    {
        if(HealthSlider.value != health)
        {
            HealthSlider.value = health;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        if(HealthSlider.value != EaseHealthSilder.value)
        {
            EaseHealthSilder.value = Mathf.Lerp(EaseHealthSilder.value, health, LerpSpeed);
        } 
    }
    void TakeDamage(float damege)
    {
        health -= damege;
    }
}
