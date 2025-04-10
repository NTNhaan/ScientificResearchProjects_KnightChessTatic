using UnityEngine;

public class IdleState : ICharacterState
{
    public void Enter(Character character)
    {
        character.GetComponent<Animator>().SetTrigger("Idle");
    }

    public void Update(Character character)
    {
        // Không cần thêm logic gì ở đây
        // Trạng thái idle sẽ được duy trì cho đến khi có sự kiện khác
    }

    public void Exit(Character character)
    {
        // Không cần thêm logic gì ở đây
    }
}