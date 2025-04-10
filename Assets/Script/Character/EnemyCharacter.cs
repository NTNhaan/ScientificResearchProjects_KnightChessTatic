using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private HeroCharater player;

    public void Start()
    {
        base.Start();
        health = maxHealth;
        attack = 5f;

        if (player == null)
        {
            player = FindObjectOfType<HeroCharater>();
            if (player == null)
            {
                Debug.LogError("EnemyCharacter: Không tìm thấy HeroCharater trong scene!");
            }
        }
    }

    public void Update()
    {
        base.Update();
    }
}



// public class EnemyCharacter : Character
// {
//     private Animator animator;
//     public void Awake()
//     {
//         animator = GetComponent<Animator>();
//     }
//     public override void Attack(Character target)
//     {
//         var atm = target.GetComponent<Character>();
//         if (atm != null)
//         {
//             animator.SetTrigger("Attack1");
//             EndAnimation(animator);
//             atm.TakeHit(attack);
//         }
//     }
//     private void EndAnimation(Animator animator)
//     {
//         AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//         while (stateInfo.normalizedTime < 0.5f)
//         {
//             stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//         }
//     }
//     public override void TakeHit(float damage)
//     {
//         health -= damage;
//         lerpTimer = 0f;
//         animator.SetTrigger("Hurt");
//         if (health <= 0)
//         {
//             Dead();
//         }
//     }
//     public override void Dead()
//     {
//         animator.SetTrigger("Death");
//         EndAnimation(animator);
//     }
//     public override void RestoreHealth(float healAmount)
//     {
//         health += healAmount;
//         lerpTimer = 0f;
//     }

// }
