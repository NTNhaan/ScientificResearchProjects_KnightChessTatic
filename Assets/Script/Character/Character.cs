using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    public float attack;
    public float health;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image fontHealthBar;
    public Image backHealthBar;
    [HideInInspector]
    public float lerpTimer;

    protected Animator animator;
    protected ICharacterState currentState;

    public void Start()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;

        // Khởi tạo state mặc định
        if (currentState == null)
        {
            ChangeState(new IdleState());
        }
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update(this);
        }
        UpdateHealthUI();
    }

    public void ChangeState(ICharacterState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }

    public void UpdateHealthUI()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        float fillFont = fontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;

        if (fillBack > hFraction)
        {
            fontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = Mathf.Pow(lerpTimer / chipSpeed, 2);
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }
        else
        {
            backHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = Mathf.Pow(lerpTimer / chipSpeed, 2);
            fontHealthBar.fillAmount = Mathf.Lerp(fillFont, hFraction, percentComplete);
        }
    }

    public virtual void PerformAttack(Character target)
    {
        ChangeState(new AttackState(target));
    }

    public virtual void ReceiveDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;

        if (health <= 0)
        {
            ChangeState(new DeadState());
        }
        else
        {
            ChangeState(new HurtState());
        }
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
}


// public abstract class Character : MonoBehaviour
// {
//     public float attack;
//     public float health;
//     [HideInInspector] public float lerpTimer;
//     public float maxHealth = 100f;
//     public float chipSpeed = 2f;
//     public Image fontHealthBar;
//     public Image backHealthBar;
//     // function for state
//     public abstract void Attack(Character target);
//     public abstract void TakeHit(float damage);
//     public abstract void Dead();
//     public abstract void RestoreHealth(float mount);
//     public void Start()
//     {
//         health = maxHealth;
//     }
//     public void Update()
//     {
//         health = Mathf.Clamp(health, 0, maxHealth);
//         float fillFont = fontHealthBar.fillAmount;
//         float fillBack = backHealthBar.fillAmount;
//         float hFraction = health / maxHealth;
//         if (fillBack > hFraction)
//         {
//             fontHealthBar.fillAmount = hFraction;
//             backHealthBar.color = Color.red;
//             lerpTimer += Time.deltaTime;
//             float percentComplete = lerpTimer / chipSpeed;
//             percentComplete = percentComplete * percentComplete;
//             backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
//         }
//         else
//         {
//             backHealthBar.fillAmount = hFraction;
//             backHealthBar.color = Color.green;
//             lerpTimer += Time.deltaTime;
//             float percentComplete = lerpTimer / chipSpeed;
//             percentComplete = percentComplete * percentComplete;
//             fontHealthBar.fillAmount = Mathf.Lerp(fillFont, hFraction, percentComplete);
//         }
//     }
// }
