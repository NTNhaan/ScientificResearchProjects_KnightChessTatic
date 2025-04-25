using UnityEngine;

public interface IAudioState
{
    void Enter(AudioManager audioManager);
    void Update(AudioManager audioManager);
    void Exit(AudioManager audioManager);
}