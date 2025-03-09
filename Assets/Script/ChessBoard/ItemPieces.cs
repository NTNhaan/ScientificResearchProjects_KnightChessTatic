using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        // Coin,
        // Tourch,
        // RedPotion,
        // Any,
        // Count
    }
    [System.Serializable]
    public struct ItemSprite
    {
        public ItemType item; // loại mảng ghép
        public Sprite sprite; // hình ảnh
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
        get { return Enum.GetValues(typeof(ItemType)).Length; }
    }
    private SpriteRenderer _sprite;
    private Dictionary<ItemType, Sprite> _itemSpriteDict;
    public void Awake()
    {
        _sprite = transform.Find("piece").GetComponent<SpriteRenderer>();
        if (_sprite == null)
        {
            Debug.LogError("SpriteRenderer not found on piece");
        }

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
    { // thiết lập các loại mảng ghép với các hình ảnh tương ứng
        item = newItem;
        if (_itemSpriteDict.ContainsKey(newItem))
        {
            _sprite.sprite = _itemSpriteDict[newItem];
        }
    }
}
