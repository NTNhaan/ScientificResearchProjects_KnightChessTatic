using UnityEngine;

public class HurtState : ICharacterState
{
    private float hurtDuration = 0.5f;
    private float hurtTimer = 0f;

    public void Enter(Character character)
    {
        character.GetComponent<Animator>().SetTrigger("Hurt");
        hurtTimer = 0f;
    }

    public void Update(Character character)
    {
        hurtTimer += Time.deltaTime;

        if (hurtTimer >= hurtDuration)
        {
            character.ChangeState(new IdleState());
        }
    }

    public void Exit(Character character)
    {
        // Không cần thêm logic gì ở đây
    }
}