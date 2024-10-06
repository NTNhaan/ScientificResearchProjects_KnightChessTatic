using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }
    public AnimationClip TranslationPlayer;
    public AnimationClip TranslationDemon;
    //public AnimationClip TranslationStart;
    private bool isPlayingPlayerAnim = false;
    private bool isPlayingDemonAnim = false;
    public Grid grid;
    private TimeBar timeBar;
    private Animator animator;
    public GameObject imageTranlation;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //SwapTurn.OnTranslationEnd += TranslationCharacter;
        animator = GetComponent<Animator>();
        grid = FindObjectOfType<Grid>();
        timeBar = FindObjectOfType<TimeBar>();
        imageTranlation.SetActive(true);

    }
    public void Update()
    {
        // if (timeBar.role == TimeBar.Role.Player)
        // {
        //     if (timeBar.TimeSliderHero.value == 0)
        //     {
        //         animator.SetTrigger("Demon");
        //     }
        //     animator.ResetTrigger("Player");
        // }
        // else if (timeBar.role == TimeBar.Role.Demon)
        // {
        //     if (timeBar.TimeSliderDemon.value == 0)
        //     {
        //         animator.SetTrigger("Player");
        //     }
        //     animator.ResetTrigger("Demon");
        // }
    }
    public void TranslationPlayerAnim()
    {
        if (!isPlayingPlayerAnim)
        {
            //isPlayingPlayerAnim = true;
            StartCoroutine(TranslationPlayerCoroutine());
        }
    }
    private IEnumerator TranslationPlayerCoroutine()
    {
        isPlayingPlayerAnim = true;
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(TranslationPlayer.name);
            yield return new WaitForSeconds(TranslationPlayer.length);
        }
        isPlayingPlayerAnim = false;
    }
    public void TranslationDemonAnim()
    {
        if (!isPlayingDemonAnim)
            StartCoroutine(TranslationDemonCoroutine());
    }
    private IEnumerator TranslationDemonCoroutine()
    {
        isPlayingDemonAnim = true;
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(TranslationDemon.name);
            yield return new WaitForSeconds(TranslationDemon.length);
        }
        isPlayingDemonAnim = false;
    }
    public void Loadlevel()
    {
        grid.isFilling = true;
    }
}
