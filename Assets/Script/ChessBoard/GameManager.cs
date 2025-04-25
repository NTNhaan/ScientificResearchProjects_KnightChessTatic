using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<ItemPieces.ItemType, System.Action<GamePieces>> itemBehaviors;
    [SerializeField] private EnemyCharacter enemy;
    [SerializeField] private HeroCharater player;
    [SerializeField] private TimeBar timeswap;

    // Score and combo system
    private int currentScore = 0;
    private float comboMultiplier = 1f;
    private float comboTimer = 0f;
    private const float COMBO_DURATION = 2f;
    private const float MAX_COMBO = 4f;
    public int RemainingMoves { get; private set; }

    public float GetComboMultiplier()
    {
        return comboMultiplier;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void AddScore(int points)
    {
        currentScore += Mathf.RoundToInt(points * comboMultiplier);
        comboMultiplier = Mathf.Min(comboMultiplier + 0.5f, MAX_COMBO);
        comboTimer = COMBO_DURATION;
    }

    void Awake()
    {
        // Khởi tạo các tham chiếu nếu chưa được gán trong Inspector
        if (timeswap == null)
            timeswap = FindObjectOfType<TimeBar>();

        if (enemy == null)
            enemy = FindObjectOfType<EnemyCharacter>();

        if (player == null)
            player = FindObjectOfType<HeroCharater>();

        comboMultiplier = 1f;
        currentScore = 0;
    }

    void Update()
    {
        // Update combo timer
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboMultiplier = 1f;
            }
        }
    }

    void Start()
    {
        // Kiểm tra xem các tham chiếu đã được khởi tạo chưa
        if (enemy == null || player == null || timeswap == null)
        {
            Debug.LogError("GameManager: Các tham chiếu quan trọng chưa được khởi tạo!");
            return;
        }

        itemBehaviors = new Dictionary<ItemPieces.ItemType, System.Action<GamePieces>>
        {
            {ItemPieces.ItemType.Sword, (GamePieces piece) => {
                if(timeswap.role == TimeBar.Role.Player)
                {
                    player.PerformAttack(enemy);
                }
                else
                {
                    enemy.PerformAttack(player);
                }
            } },
            {ItemPieces.ItemType.Apple, (GamePieces piece) => {
                if(timeswap.role == TimeBar.Role.Player)
                {
                    player.RestoreHealth(5);
                }
                else
                {
                    enemy.RestoreHealth(5);
                }
            } },
            {ItemPieces.ItemType.Heart, (GamePieces piece) => {
                if(timeswap.role == TimeBar.Role.Player)
                {
                    player.RestoreHealth(player.maxHealth);
                }
                else
                {
                    enemy.RestoreHealth(enemy.maxHealth);
                }
            } }
        };
    }

    public void HandleItemBehaviour(GamePieces piece)
    {
        // Kiểm tra null trước khi sử dụng
        if (piece == null || piece.ItemComponent == null)
        {
            Debug.LogError("GameManager: GamePieces hoặc ItemComponent là null!");
            return;
        }

        if (itemBehaviors.ContainsKey(piece.ItemComponent.Item))
        {
            itemBehaviors[piece.ItemComponent.Item].Invoke(piece);
        }
        else
        {
            // Debug.Log("No item behavior found for " + piece.ItemComponent.Item);
        }
        switch (piece.ItemComponent.Item)
        {
            case ItemPieces.ItemType.Apple:
                // Handle Apple behavior
                break;
            case ItemPieces.ItemType.AppleGreen:
                // Handle AppleGreen behavior
                break;
            case ItemPieces.ItemType.Beer:
                // Handle Beer behavior
                break;
            case ItemPieces.ItemType.Sword:
                // Handle Sword behavior
                break;
            case ItemPieces.ItemType.Heart:
                // Handle Heart behavior
                break;
            case ItemPieces.ItemType.Armor:
                // Handle Armor behavior
                break;
            case ItemPieces.ItemType.Shield:
                // Handle Shield behavior
                break;
            case ItemPieces.ItemType.Mushroom:
                // Handle Mushroom behavior
                break;
            default:
                Debug.LogError($"No item behavior found for {piece.ItemComponent.Item}");
                break;
        }
    }
}
