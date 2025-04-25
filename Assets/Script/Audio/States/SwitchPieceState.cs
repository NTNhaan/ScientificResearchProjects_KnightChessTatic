using UnityEngine;

public class SwitchPieceState : IAudioState
{
    public void Enter(AudioManager audioManager)
    {
        // Phát âm thanh khi swap piece
        audioManager.Play("SwitchPiece");
    }

    public void Update(AudioManager audioManager)
    {
        // Không cần cập nhật gì thêm
    }

    public void Exit(AudioManager audioManager)
    {
        // Không cần dừng âm thanh vì nó sẽ tự kết thúc
    }
}