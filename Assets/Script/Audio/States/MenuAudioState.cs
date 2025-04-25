using UnityEngine;

public class MenuAudioState : IAudioState
{
    public void Enter(AudioManager audioManager)
    {
        // Khởi tạo âm thanh menu
        audioManager.Play("MenuSound");
    }

    public void Update(AudioManager audioManager)
    {
        // Cập nhật âm thanh menu nếu cần
    }

    public void Exit(AudioManager audioManager)
    {
        // Dừng âm thanh menu
        audioManager.Stop("MenuSound");
    }
}