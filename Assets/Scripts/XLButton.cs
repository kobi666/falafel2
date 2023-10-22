using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class XLButton : MonoBehaviour
{

    public Image StarImage;
    public RectTransform XLButtonRect;

    public Color StarFillColor;

    bool StarPopInProgress = false;
    public async void doFill()
    {
        if (StarPowerReady)
        {
            StarPowerReady = false;
            AddStarPower(-1f);
            popCts?.Cancel();

            await FillItems();
        }
        else
        {
            shaker.Shake(0.2f);
        }
    }

    public async void FillForce()
    {
        popCts?.Cancel();

        await FillItems();
    }

    [Required] public Shaker shaker;

    Vector3 initialScale;

    public Color[] RainbowColors;

    public float PopScale = 1.2f;
    public float popTime = 0.1f;
    CancellationTokenSource popCts = new();
    public async UniTaskVoid PopButton(RectTransform transform)
    {
        popCts?.Cancel();
        popCts = new();
        var token = popCts.Token;
        var originScale = initialScale;
        var targetScale = originScale * PopScale;
        float counter = 0f;
        var _popTime = popTime / 2f;
        while (!token.IsCancellationRequested)
        {
            counter = 0f;
            while (counter < _popTime)
            {
                counter += Time.deltaTime;
                transform.localScale = Vector3.Lerp(originScale, targetScale, counter / _popTime);
                await UniTask.Yield();
            }

            counter = 0f;
            while (counter < _popTime)
            {
                counter += Time.deltaTime;
                transform.localScale = Vector3.Lerp(targetScale, originScale, counter / _popTime);
                await UniTask.Yield();
            }
        }
        transform.localScale = originScale;
        StarImage.color = StarFillColor;
    }

    [Button]
    public void AddStarPower(float power)
    {
        if (!StarPowerReady)
        {
            StarPowerPrecentage = Mathf.Clamp01(StarPowerPrecentage + power);
        }
        if (StarPowerPrecentage >= 1f)
        {
            StarPowerReady = true;
            UniTask.Void(() => PopButton(XLButtonRect));
        }
        else
        {
            StarPowerReady = false;
            popCts?.Cancel();
        }
        StarImage.fillAmount = StarPowerPrecentage;
    }

    public bool StarPowerReady = false;
    public float StarPowerPrecentage = 0f;
    public float colorShiftTime = 0.1f;

    public List<ItemButton> Items;
    public float fillTime = 0.05f;
    CancellationTokenSource myCts;

    [Required] public AudioSource FillSoundSource1;
    [Required] public AudioSource FillSoundSource2;

    async UniTask FillItems()
    {
        int allItemsIndex = 0;
        bool aSource = false;
        myCts?.Cancel();
        myCts = new();
        var token = myCts.Token;
        FillSoundSource1.pitch = 1f;
        FillSoundSource2.pitch = 1f;
        float counter = 0f;
        int itemIndex = 0;
        int maxItemIndex = Items.Count - 1;
        int maxTotalItems = 0;
        for (int i = 0; i < Items.Count; i++)
        {
            maxTotalItems += Items[i].ItemMaxAmount - Items[i].itemAmount;
        }
//        Debug.LogWarning("max items to fill" + maxTotalItems.ToString());
        while (Items.Any(x => x.itemAmount < x.ItemMaxAmount) && !token.IsCancellationRequested)
        {
            counter += Time.deltaTime;
            if (counter >= fillTime)
            {
                bool b = Items[itemIndex].AddAmount();
                if (b)
                {
                    counter = 0f;
                    aSource = !aSource;
                    allItemsIndex++;
                    if (aSource)
                    {
                        FillSoundSource1.pitch = Mathf.Lerp(1f, 1.4f, (float)allItemsIndex / (float)maxTotalItems);
                        FillSoundSource1.Stop();
                        FillSoundSource1.Play();
                    }
                    else
                    {
                        FillSoundSource2.pitch = Mathf.Lerp(1f, 1.4f, (float)allItemsIndex / (float)maxTotalItems);
                        FillSoundSource2.Stop();
                        FillSoundSource2.Play();
                    }
                }

                itemIndex++;
                if (itemIndex > maxItemIndex)
                {
                    itemIndex = 0;
                }
            }



            //Debug.LogWarning($" allitemsindex : {allItemsIndex} , maxItems : {maxTotalItems} , divided {(float)allItemsIndex / (float)maxTotalItems}");

            await UniTask.Yield();
        }
    }

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        myCts = CancellationTokenSource.CreateLinkedTokenSource(GameManager.instance.InGameCTS.Token);
        AddStarPower(-1f);
        StarImage.color = StarFillColor;
        initialScale = XLButtonRect.localScale;
    }

    int colorIndex = 0;
    int nextColorindex = 1;
    float colorShiftCounter = 0f;


    // Update is called once per frame
    void Update()
    {
        if (StarPowerReady)
        {
            colorShiftCounter += Time.deltaTime;
            if (colorShiftCounter >= colorShiftTime)
            {
                colorShiftCounter = 0f;
                colorIndex++;
                nextColorindex++;
                if (colorIndex >= RainbowColors.Length)
                {
                    colorIndex = 0;
                }
                if (nextColorindex >= RainbowColors.Length)
                {
                    nextColorindex = 0;
                }
            }
            StarImage.color = Color.Lerp(RainbowColors[colorIndex], RainbowColors[nextColorindex], colorShiftCounter / colorShiftTime);
        }
    }
}