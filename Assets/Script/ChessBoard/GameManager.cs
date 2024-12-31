using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<ItemPieces.ItemType, System.Action<GamePieces>> itemBehaviors;
    public EnemyCharacter enemy;
    public HeroCharater player;
    [SerializeField] private TimeBar timeswap;

    void Awake()
    {
        timeswap = FindObjectOfType<TimeBar>();
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
            Debug.Log("No item behavior found for " + piece.ItemComponent.Item);
        }
    }
}
