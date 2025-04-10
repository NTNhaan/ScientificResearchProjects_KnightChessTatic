using UnityEngine;

public class HeroCharater : Character
{
    [SerializeField] private EnemyCharacter enemy;

    public void Start()
    {
        base.Start();
        health = maxHealth;
        attack = 10f;

        if (enemy == null)
        {
            enemy = FindObjectOfType<EnemyCharacter>();
            if (enemy == null)
            {
                Debug.LogError("HeroCharater: Không tìm thấy EnemyCharacter trong scene!");
            }
        }
    }

    public void Update()
    {
        base.Update();
    }
}



// public class HeroCharater : Character
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
//             animator.SetTrigger("Attack3");
//             AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//             while (stateInfo.normalizedTime < 1.0f)
//             {
//                 stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//             }
//             atm.TakeHit(atm.attack);
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
