using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class rachelCookies : MonoBehaviour
{

    public CancellationTokenSource tokenSource = new();

    [Required] public RectTransform glowEffectRect;


    public float initialScale = 0.457f;
    public float finalScale = 0.475f;
    bool fillButtonPresesed = false;
    public float glowScaleTime = 0.5f;
    public AnimationCurve glowScaleCurve;
    public async UniTaskVoid glowScaleAnimation()
    {
        glowEffectRect.gameObject.SetActive(true);
        Vector3 _initialScale = new Vector3(-1, 1, 1) * this.initialScale;
        Vector3 targetScale = new Vector3(-1, 1, 1) * this.finalScale;
        glowEffectRect.localScale = _initialScale;

        float counter = 0f;
        float scaleTime = glowScaleTime / 2f;
        while (fillButtonPresesed == false)
        {
            while (counter <= scaleTime)
            {
                counter += Time.deltaTime;
                glowEffectRect.localScale = Vector3.Lerp(_initialScale, targetScale, glowScaleCurve.Evaluate(counter / scaleTime));
                await UniTask.Yield();
            }
            counter = 0f;
        }
        glowEffectRect.gameObject.SetActive(false);
    }


    [Required]
    public AudioSource audioSource;
    public AudioClip[] onFillStartClips;

    public void PlayFillClip()
    {
        audioSource.PlayOneShot(onFillStartClips.GetRandom());
    }

     public Image[] outlineImages;

    [Required]
    public RectTransform FillSign;

    public Sprite[] AnimationFrames;
    public Image MyImage;

    public float timeBetweenFrames = 0.01f;
    [Button]
    public void SetRachelAnimation()
    {
        UniTask.Void(Animation);
    }

    public async UniTaskVoid Animation()
    {

        tokenSource?.Cancel();
        tokenSource = new();
        var token = tokenSource.Token;
        foreach (var sprite in AnimationFrames)
        {
            await DelayTime(timeBetweenFrames, token);
            MyImage.sprite = sprite;
        }


    }

    public async UniTask DelayTime(float time, CancellationToken token)
    {
        float counter = 0f;
        while (counter <= time && !token.IsCancellationRequested)
        {
            counter += Time.deltaTime;
            if (counter >= time)
            {
                break;
            }
            await UniTask.Yield();
        }

    }

    CancellationTokenSource signMoveCTS = new();
    public RectTransform SignEnterPos;
    public RectTransform SignExitPos;

    async UniTaskVoid moveSignIn()
    {
        signMoveCTS?.Cancel();
        signMoveCTS = new();
        var token = signMoveCTS.Token;
        await UniTask.DelayFrame(1);
        float counter = 0f;
        float moveTime = 0.2f;
        var originPos = FillSign.anchoredPosition;
        while (counter <= moveTime && !token.IsCancellationRequested)
        {
            counter += Time.deltaTime;
            FillSign.anchoredPosition = Vector2.Lerp(originPos,
                SignEnterPos.anchoredPosition,  counter /  moveTime );
            await UniTask.Yield();
        }
    }

    async UniTaskVoid moveSignOut()
    {
        signMoveCTS?.Cancel();
        signMoveCTS = new();
        var token = signMoveCTS.Token;
        await UniTask.DelayFrame(1);
        float counter = 0f;
        float moveTime = 0.2f;
        var originPos = FillSign.anchoredPosition;
        while (counter <= moveTime && !token.IsCancellationRequested)
        {
            counter += Time.deltaTime;
            FillSign.anchoredPosition = Vector2.Lerp(originPos,
                SignExitPos.anchoredPosition,  counter /  moveTime );
            await UniTask.Yield();
        }
    }

    public void SetItemFillMode()
    {
        //Debug.LogWarning("pRessed");
        fillButtonPresesed = true;
        if (GameManager.instance.mode == TapMode.FillItems)
        {
            GameManager.instance.mode = TapMode.AddItemsToBox;
            for (int i = 0; i < outlineImages.Length; i++)
            {
                outlineImages[i].gameObject.SetActive(false);
            }
            UniTask.Void(moveSignOut);
        }
        else if (GameManager.instance.mode == TapMode.AddItemsToBox)
        {
            PlayFillClip();
            GameManager.instance.mode = TapMode.FillItems;
            for (int i = 0; i < outlineImages.Length; i++)
            {
                outlineImages[i].gameObject.SetActive(true);
            }
            UniTask.Void(moveSignIn);
        }
        OnSetMode(GameManager.instance.mode);
    }

    void OnSetMode(TapMode mode)
    {

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