using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<ItemPieces.ItemType, System.Action<GamePieces>> itemBehaviors;
    public EnemyCharacter enemy;
    public HeroCharater player;
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
        timeswap = FindObjectOfType<TimeBar>();
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
        itemBehaviors = new Dictionary<ItemPieces.ItemType, System.Action<GamePieces>>
        {
            {ItemPieces.ItemType.Sword, (GamePieces piece) => {
                if(timeswap.role == TimeBar.Role.Player)
                {
                    player.Attack(enemy);
                }
                else
                {
                    enemy.Attack(player);
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
            } }
        };
    }

    public void HandleItemBehaviour(GamePieces piece)
    {
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
                // Debug.LogError($"No item behavior found for {piece.ItemComponent.Item}");
                break;
        }
    }
}
