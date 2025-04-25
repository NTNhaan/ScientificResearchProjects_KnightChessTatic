using UnityEngine;

public class GameplayAudioState : IAudioState
{
    public void Enter(AudioManager audioManager)
    {
        // Khởi tạo âm thanh gameplay
        audioManager.PlayBackgroundMusic("PuzzleSound");
    }

    public void Update(AudioManager audioManager)
    {
        // Cập nhật âm thanh gameplay nếu cần
    }

    public void Exit(AudioManager audioManager)
    {
        // Không dừng âm thanh background khi thoát state
    }
}