using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<ItemPieces.ItemType, System.Action<GamePieces>> itemBehaviors;
    public EnemyCharacter enemy;
    public HeroCharater player;


    void Start()
    {
        itemBehaviors = new Dictionary<ItemPieces.ItemType, System.Action<GamePieces>>
        {
            {ItemPieces.ItemType.Sword, (GamePieces piece) => { player.Attack(enemy); } },

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
