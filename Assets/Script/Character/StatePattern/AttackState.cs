using UnityEngine;

public class AttackState : ICharacterState
{
    private Character target;
    private bool hasAttacked = false;

    public AttackState(Character target)
    {
        this.target = target;
    }

    public void Enter(Character character)
    {
        character.GetComponent<Animator>().SetTrigger("Attack3");
    }

    public void Update(Character character)
    {
        var stateInfo = character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        // Gây sát thương khi animation đạt đến điểm thích hợp
        if (stateInfo.normalizedTime >= 0.5f && !hasAttacked)
        {
            hasAttacked = true;
            if (target != null)
            {
                target.ReceiveDamage(character.attack);
            }
        }

        // Khi animation kết thúc, chuyển về trạng thái Idle
        if (stateInfo.normalizedTime >= 1.0f)
        {
            character.ChangeState(new IdleState());
        }
    }

    public void Exit(Character character)
    {
        hasAttacked = false;
    }
}