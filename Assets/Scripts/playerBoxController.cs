using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class playerBoxController : MonoBehaviour
{

    public Sprite[] OpenBoxAnimation;

    public boxdeb Boxdeb;
    public Image playerBoxImage;
    public Image[] BambaImages;
    public Image[] Shampooimages;
    public Image[] pantsImages;
    public Image[] CakeImages;
    public Image GreetingImage;



    public void AddItemToBox(ItemType type)
    {
        switch (type)
        {
            case ItemType.Bamba:
                foreach (var image in BambaImages)
                {
                    if (!image.enabled)
                    {
                        image.enabled = true;
                        break;
                    }
                }
                break;
            case ItemType.Shampoo:
                foreach (var image in Shampooimages)
                {
                    if (!image.enabled)
                    {
                        image.enabled = true;
                        break;
                    }
                }
                break;
            case ItemType.Pants:
                foreach (var image in pantsImages)
                {
                    if (!image.enabled)
                    {
                        image.enabled = true;
                        break;
                    }
                }
                break;
            case ItemType.Cake:
                foreach (var image in CakeImages)
                {
                    if (!image.enabled)
                    {
                        image.enabled = true;
                        break;
                    }
                }
                break;
            case ItemType.anotherThing:
                GreetingImage.enabled = true;
                break;
        }
    }


    public void ClearPlayerBox()
    {
        foreach (var image in BambaImages)
        {
            image.enabled = false;
        }
        foreach (var image in Shampooimages)
        {
            image.enabled = false;
        }
        foreach (var image in pantsImages)
        {
            image.enabled = false;
        }
        foreach (var image in CakeImages)
        {
            image.enabled = false;
        }
        GreetingImage.enabled = false;
        UniTask.Void(NewBoxSequence);
    }

    [Required]
    public RectTransform OriginBoxPosition, TargetBoxPosition;

    [Required]
    public RectTransform MyRectTRransfrom;

    public float movementTime = 0.2f;
    float myfloat = 2f;
    public async UniTaskVoid NewBoxSequence()
    {
        playerBoxImage.enabled = false;
        MyRectTRransfrom.anchoredPosition = OriginBoxPosition.anchoredPosition;
        await UniTask.DelayFrame(1);
        playerBoxImage.sprite = OpenBoxAnimation[0];
        playerBoxImage.enabled = true;
        await UniTask.DelayFrame(1);
        float counter = 0f;
        while (counter <= movementTime)
        {
            counter += Time.deltaTime;
            MyRectTRransfrom.anchoredPosition = Vector2.Lerp(OriginBoxPosition.anchoredPosition, TargetBoxPosition.anchoredPosition, counter / movementTime);
            await UniTask.Yield();
        }
        UniTask.Void(OpenBox);
    }

    [Button]
    public void ImageFrameByFrameAnimation()
    {
        UniTask.Void(OpenBox);
    }

    public async UniTaskVoid OpenBox()
    {
        Boxdeb.PlayerHasBox = true;
        playerBoxImage.enabled = true;
        Boxdeb.inBoxAnimation = true;
        foreach (var sprite in OpenBoxAnimation)
        {
            playerBoxImage.sprite = sprite;
            await UniTask.Yield();
        }
        Boxdeb.inBoxAnimation = false;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}