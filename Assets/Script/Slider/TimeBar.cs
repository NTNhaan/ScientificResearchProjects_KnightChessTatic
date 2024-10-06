using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public enum Role
    {
        Player,
        Demon
    }
    public static TimeBar Instance { get; private set; }
    public Slider TimeSliderDemon;
    public Slider TimeSliderHero;
    public float MaxTime = 100;
    public Role role;
    public Animator animator;
    private bool isPaused = false;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        TimeSliderHero.value = MaxTime;
        TimeSliderDemon.value = MaxTime;
    }
    public void Start()
    {
        role = Role.Player;
    }
    public void SwapRole()
    {
        if (role == Role.Player)
        {
            isPaused = false;
            role = Role.Demon;
            TimeSliderHero.value = MaxTime;

        }
        else if (role == Role.Demon)
        {
            isPaused = false;
            role = Role.Player;
            TimeSliderDemon.value = MaxTime;
        }
    }
    public void ResetAnimation()
    {
        animator.ResetTrigger("StartTurn");
        animator.ResetTrigger("StartTurnBack");
    }
    public void PlayAnimation(string nametrigger)
    {
        animator.SetTrigger(nametrigger);
    }
    public void Update()
    {
        bool SwapOnBoard = SwapTurn.Instance.IsSwapping;
        if (role == Role.Player && !isPaused)
        {
            TimeSliderHero.value -= Time.deltaTime * 10;
            if (TimeSliderHero.value <= 0)
            {
                SwapTurn.Instance.StartSwap();
                PlayAnimation("StartTurn");
            }
        }
        if (role == Role.Demon && !isPaused)
        {
            TimeSliderDemon.value -= Time.deltaTime * 10;
            if (TimeSliderDemon.value <= 0)
            {
                SwapTurn.Instance.StartSwap();
                PlayAnimation("StartTurnBack");
            }
        }
    }
    public void Pause()
    {
        isPaused = true;  // Pause the time bar when the player is swapping
        SwapTurn.Instance.StartSwap();  // Pauses to prevent players from swapping while the animation is running
    }
}