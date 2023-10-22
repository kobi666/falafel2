using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class boxdeb : MonoBehaviour
{
    public static boxdeb instance;
    //public TextMeshProUGUI addedItemsText;


    public bool inBoxAnimation = false;

    void Awake()
    {
        instance = this;
    }

    public Dictionary<ItemType, int> items = new Dictionary<ItemType, int>()
    {
        { ItemType.Bamba, 0 },
        { ItemType.Pants, 0 },
        { ItemType.Shampoo, 0 },
        { ItemType.Cake, 0 },
        { ItemType.anotherThing ,0}
    };

    public bool PlayerHasBox = false;

    public void clearBox()
    {
        items[ItemType.Bamba] = 0;
        items[ItemType.Pants] = 0;
        items[ItemType.Shampoo] = 0;
        items[ItemType.Cake] = 0;
        items[ItemType.anotherThing] = 0;
        PlayerHasBox = false;
        onBoxContentsChange?.Invoke();
    }

    public void OnBoxContentsChange()
    {
        onBoxContentsChange?.Invoke();
    }


    public void AddItem(ItemType itemType)
    {
        items[itemType]++;
        onBoxContentsChange?.Invoke();
    }

    public event Action onBoxContentsChange;


}