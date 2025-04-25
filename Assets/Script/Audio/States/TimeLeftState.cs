using UnityEngine;

public class TimeLeftState : IAudioState
{
    public void Enter(AudioManager audioManager)
    {
        // Phát âm thanh khi swap piece
        audioManager.Play("TimeWarning");
    }

    public void Update(AudioManager audioManager)
    {
        // Không cần cập nhật gì thêm
    }

    public void Exit(AudioManager audioManager)
    {
        audioManager.Stop("TimeWarning");
    }
}