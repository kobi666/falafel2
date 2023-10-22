using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public ItemType itemType;

    public int itemAmount;
    public int ItemMaxAmount;

    [Required]
    public AudioSource itemAudio;

    public Image itemImage;
    public Shaker shaker;

    public Image[] itemImages = new Image[0];

    public void SetimagesAccordingToAmount(int currentAmount)
    {
        for (int i = 0; i < itemImages.Length; i++)
        {
            itemImages[i].gameObject.SetActive(currentAmount > i);
        }
    }

    public RectTransform MyRectTransform;

    public Color ItemColor;

    public bool hasCat = false;

    public void SetCat()
    {
        Debug.LogWarning($"Cat put on {gameObject.GetInstanceID()} ");
        hasCat = true;
        GameManager.instance.Cat.anchoredPosition = MyRectTransform.anchoredPosition;
    }

    public bool AddAmount()
    {
        int currentAmount = itemAmount;
        itemAmount = Mathf.Clamp(itemAmount + 1, 0, ItemMaxAmount);
        if (currentAmount != itemAmount)
        {
            //itemImage.color = Color.Lerp(Color.black, Color.white, (float)itemAmount / ItemMaxAmount);
            SetimagesAccordingToAmount(itemAmount);
            UpdateOutlineImage();
        }
        return currentAmount != itemAmount;
    }

    public Image outlineImage;

    void UpdateOutlineImage()
    {
        outlineImage.color = GameManager.instance.ItemBoxOutlineMaximumColor;
        //outlineImage.color = Color.Lerp(GameManager.instance.ItemBoxOutlineMinimumColor, GameManager.instance.ItemBoxOutlineMaximumColor, (float)itemAmount / ItemMaxAmount);
    }

    public boxdeb boxdeb;


    public void RemoveAmount()
    {
        if (GameManager.instance.mode == TapMode.AddItemsToBox)
        {
            if (boxdeb.PlayerHasBox)
            {
                if (itemAmount > 0 && boxdeb.items[itemType] < 3)
                {
                    if (itemType == ItemType.anotherThing && boxdeb.items[itemType] > 0)
                    {
                        //AudioManager.instance.PlayerBadMove();
                        shaker.Shake(0.5f);
                        GameManager.instance.vibrator.OnVibrate2();
                        return;
                    }
                    itemAudio.Play();

                    itemAmount = Mathf.Clamp(itemAmount - 1, 0, ItemMaxAmount);
                    if (GameManager.instance.FirstBox && itemAmount <= 0)
                    {
                        UniTask.Void(GameManager.instance.RachelCookies.glowScaleAnimation);
                    }
                    //itemImage.color = Color.Lerp(Color.black, Color.white, (float)itemAmount / ItemMaxAmount);
                    boxdeb.AddItem(itemType);
                    boxdeb.OnBoxContentsChange();
                    GameManager.instance.PlayerBox.AddItemToBox(itemType);
                    UpdateOutlineImage();
                    SetimagesAccordingToAmount(itemAmount);
                }
                else
                {
                    shaker.Shake(0.5f);
                    AudioManager.instance.PlayerBadMove();
                }
            }
            else if (!boxdeb.PlayerHasBox)
            {
                GameManager.instance.newBoxController.shaker.Shake(0.25f);
                AudioManager.instance.PlayerBadMove();
            }
        }

        if (GameManager.instance.mode == TapMode.FillItems)
        {
            if (itemAmount < ItemMaxAmount)
            {

                itemAmount = Mathf.Clamp(itemAmount + 1, 0, ItemMaxAmount);
                    //itemImage.color = Color.Lerp(Color.black, Color.white, (float)itemAmount / ItemMaxAmount);
                    GameManager.instance.RachelCookies.SetRachelAnimation();
                    UpdateOutlineImage();
                SetimagesAccordingToAmount(itemAmount);
            }
            else
            {
                shaker.Shake(0.5f);
                GameManager.instance.vibrator.OnVibrate2();
                AudioManager.instance.PlayerBadMove();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //MyRectTransform = GetComponent<RectTransform>();
        itemAmount = ItemMaxAmount;
        UpdateOutlineImage();
        AddAmount();
    }

    public void PrintItemType()
    {
        Debug.Log(itemType);
    }

    // Update is called once per frame
    void Update()
    {

    }
}


[System.Serializable]
public enum ItemType
{
    Bamba,
    Pants,
    Shampoo,
    Cake,
    anotherThing
}