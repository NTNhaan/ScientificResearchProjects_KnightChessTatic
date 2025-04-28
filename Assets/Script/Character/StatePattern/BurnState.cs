using UnityEngine;

public class BurnState : ICharacterState
{
    private float burnDuration = 3f;
    private float burnDamagePerSecond = 5f;
    private float burnTimer;
    private Color burnColor = new Color(1f, 0.5f, 0f, 0.5f);
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public void Enter(Character character)
    {
        burnTimer = burnDuration;
        spriteRenderer = character.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Phát animation thiêu đốt nếu có
        character.GetComponent<Animator>()?.SetTrigger("Burn");
    }

    public void Update(Character character)
    {
        if (burnTimer > 0)
        {
            // Gây sát thương
            character.ReceiveDamage(burnDamagePerSecond * Time.deltaTime);

            // Hiệu ứng nhấp nháy
            spriteRenderer.color = Color.Lerp(originalColor, burnColor, Mathf.PingPong(Time.time * 10f, 1f));

            burnTimer -= Time.deltaTime;
        }
        else
        {
            // Kết thúc hiệu ứng thiêu đốt
            spriteRenderer.color = originalColor;
            character.ChangeState(new IdleState());
        }
    }

    public void Exit(Character character)
    {
        // Reset màu sắc
        spriteRenderer.color = originalColor;
    }
}