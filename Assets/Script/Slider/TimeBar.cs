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
    public float timeScale = 1f;   // tg cho speedupState
    public float baseSpeed = 1f;
    public float currentSpeed;
    public Role role;
    public Animator animator;
    private bool isPaused = false;
    private bool hasPlayedWarning = false;
    private const float WARNING_THRESHOLD = 30f;
    private bool isGameStarted = false;
    public float maxTimeScale = 3f; // Giới hạn tốc độ tối đa
    public float minTimeScale = 0.5f;
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
        currentSpeed = baseSpeed;
        role = Role.Player;
        // Đăng ký lắng nghe sự kiện board đã fill xong
        Grid.OnBoardFilled += StartGame;
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi object bị destroy
        Grid.OnBoardFilled -= StartGame;
    }

    private void StartGame()
    {
        isGameStarted = true;
    }

    public void SwapRole()
    {
        if (role == Role.Player)
        {
            isPaused = false;
            role = Role.Demon;
            TimeSliderHero.value = MaxTime;
            hasPlayedWarning = false;
        }
        else if (role == Role.Demon)
        {
            isPaused = false;
            role = Role.Player;
            TimeSliderDemon.value = MaxTime;
            hasPlayedWarning = false;
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
        currentSpeed = baseSpeed * timeScale;
        // Chỉ cập nhật thời gian khi game đã bắt đầu
        if (!isGameStarted) return;

        bool SwapOnBoard = SwapTurn.Instance.IsSwapping;
        if (role == Role.Player && !isPaused)
        {
            TimeSliderHero.value -= Time.deltaTime * 10;

            // Kiểm tra và phát âm thanh cảnh báo
            if (TimeSliderHero.value <= WARNING_THRESHOLD && !hasPlayedWarning)
            {
                AudioManager.Instance.ChangeState(new TimeLeftState());
                hasPlayedWarning = true;
            }

            if (TimeSliderHero.value <= 0)
            {
                // Dừng âm thanh cảnh báo khi slider về 0
                AudioManager.Instance.Stop("TimeWarning");
                SwapTurn.Instance.StartSwap();
                PlayAnimation("StartTurn");
            }
        }
        if (role == Role.Demon && !isPaused)
        {
            TimeSliderDemon.value -= Time.deltaTime * 10;

            // Kiểm tra và phát âm thanh cảnh báo
            if (TimeSliderDemon.value <= WARNING_THRESHOLD && !hasPlayedWarning)
            {
                AudioManager.Instance.ChangeState(new TimeLeftState());
                hasPlayedWarning = true;
            }

            if (TimeSliderDemon.value <= 0)
            {
                // Dừng âm thanh cảnh báo khi slider về 0
                AudioManager.Instance.Stop("TimeWarning");
                SwapTurn.Instance.StartSwap();
                PlayAnimation("StartTurnBack");
            }
        }
    }
    public void SetTimeScale(float newScale)
    {
        timeScale = Mathf.Clamp(newScale, 0.5f, 3f); // Giới hạn tốc độ từ 0.5x đến 3x
        currentSpeed = baseSpeed * timeScale;
        Debug.Log($"Time scale set to: {timeScale}");
    }

    public void ResetTimeScale()
    {
        timeScale = 1f;
        currentSpeed = baseSpeed;
        Debug.Log("Time scale reset to normal");
    }
    public void Pause()
    {
        isPaused = true;
        SwapTurn.Instance.StartSwap();
    }
}