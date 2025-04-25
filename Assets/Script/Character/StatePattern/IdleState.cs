using UnityEngine;

public class IdleState : ICharacterState
{
    public void Enter(Character character)
    {
        Animator animator = character.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // Kiểm tra xem parameter có tồn tại không
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "Idle")
                {
                    animator.SetTrigger("Idle");
                    break;
                }
            }
        }
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