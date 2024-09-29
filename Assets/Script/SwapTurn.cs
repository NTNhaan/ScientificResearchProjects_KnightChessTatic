using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTurn : MonoBehaviour
{
    private bool isSwapping = false;
    public static SwapTurn Instance { get; private set; }
    public static event Action OnTranslationEnd;
    public bool IsSwapping
    {
        get { return isSwapping; }
    }
    public void StartSwap()
    {
        isSwapping = true;
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //TimeBar.Instance.animator = GetComponent<Animator>();
    }
    public void OnAnimationEnd()
    {
        isSwapping = false;
        TimeBar.Instance.SwapRole();
        TimeBar.Instance.ResetAnimation();
    }
    public void TranslationHero()
    {
        LevelLoader.Instance.TranslationPlayerAnim();
    }
    public void TranslationDemon()
    {
        LevelLoader.Instance.TranslationDemonAnim();
    }
}
