using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPieces : MonoBehaviour
{
    public enum ItemType
    {
        Apple,
        AppleGreen,
        Beer,
        Sword,
        Heart,
        Armor,
        Shield,
        Mushroom,
        Coin,
        Tourch,
        RedPotion,
        BluePotion,
        Any,
        Count
    }
    [System.Serializable]
    public struct ItemSprite
    {
        public ItemType item;
        public Sprite sprite;
    }
    public ItemSprite[] itemSprites; // mảng chứa các hình ảnh
    private ItemType item;
    public ItemType Item
    {
        get { return item; }
        set { SetItem(value); }
    }
    public int NumItems
    {
        get { return itemSprites.Length; }
    }
    private SpriteRenderer _sprite;
    private Dictionary<ItemType, Sprite> _itemSpriteDict;
    public void Awake()
    {
        _sprite = transform.Find("piece").GetComponent<SpriteRenderer>();
        _itemSpriteDict = new Dictionary<ItemType, Sprite>();
        for (int i = 0; i < itemSprites.Length; i++)
        {
            if (!_itemSpriteDict.ContainsKey(itemSprites[i].item))
            {
                _itemSpriteDict.Add(itemSprites[i].item, itemSprites[i].sprite);
            }
        }
    }
    public void SetItem(ItemType newItem)
    {
        item = newItem;
        if (_itemSpriteDict.ContainsKey(newItem))
        {
            _sprite.sprite = _itemSpriteDict[newItem];
        }
    }
}
