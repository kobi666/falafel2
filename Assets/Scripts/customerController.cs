using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyBox;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class customerController : MonoBehaviour
{

    public TextMeshProUGUI customerText;
    [ShowInInspector] public RectTransform myRectTransform;

    [Required] public GameObject readyBox;

    public float movementTime;
    public float stepTime;

    Func<Vector2> getV2;
    public float zigThreshold = 4f;

    public RectTransform inAreaRight,InAreaLeft;
    public RectTransform originPosition, targetPosition;
    public bubbleController bubbleController;


    public float popTime = 0.15f;
    public float PopScale = 1.2f;
    Vector3 originBubbleScale;

    public async UniTaskVoid PopButton(RectTransform transform)
    {
        var originScale = originBubbleScale;
        var targetScale = originScale * PopScale;
        float counter = 0f;
        var _popTime = popTime / 2f;
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
        transform.localScale = originScale;
    }






    RectTransform queuedSlot;

    public int rainbowFactor = 2;
    [Button]
    public async void MoveToInAreaSequence(RectTransform targetslot)
    {
        inGame = true;
        queuedSlot = targetslot;
        readyBox.SetActive(false);
        bool _rainbow = Random.Range(1, rainbowFactor) == rainbowFactor - 1;
        bubbleController.backlightObject.SetActive(_rainbow);
        bubbleController.RainbowMode = _rainbow;
        myRectTransform.anchoredPosition = originPosition.anchoredPosition;
        Vector2 TargetPosition = targetslot.anchoredPosition;
        UniTask.Void(() => MoveInZigZag(TargetPosition, false, true));

    }

    [Button]
    async void MoveOutAreaSequence(bool playSound = true)
    {
        customerAudioSource.clip = goodbyeClips.GetRandom();
        if (playSound)
        {
            customerAudioSource.Play();
        }
        else
        {
            scoreobj.AddScore(0, false, bubbleController.BubbleRect.anchoredPosition);
            inGame = false;
        }

        UniTask.Void(() => MoveInZigZag(targetPosition.anchoredPosition, true));
        bubbleController.gameObject.SetActive(false);
        //
        // GameManager.instance.EnqueueCustomer(this);
        // GameManager.instance.slotsQueue.Enqueue(queuedSlot);
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

    [Required] public AudioSource customerAudioSource;
    public AudioClip[] HelloClips;
    public AudioClip[] goodbyeClips;

    public float RightBasePositionY;
    public CancellationTokenSource movementCTS = new();
    bool isMoving = false;
    public async UniTaskVoid MoveInZigZag(Vector2 Anchordposition, bool enqueueAtend, bool setInsturctions = false)
    {
        UpdateBacklightBubble();
        movementCTS?.Cancel();
        movementCTS = new CancellationTokenSource();

        await UniTask.DelayFrame(1);
        isMoving = true;
        var token = movementCTS.Token;
        float counter = 0f;
        float step = 0f;
        bool zigYDirection = true;
        float baseY = myRectTransform.anchoredPosition.y;
        float OriginY = myRectTransform.anchoredPosition.y;
        float TargetX = Anchordposition.x;
        float targetY = Anchordposition.y;
        float currentX = myRectTransform.anchoredPosition.x;
        float currentY = myRectTransform.anchoredPosition.y;
        Vector2 OriginPosition = myRectTransform.anchoredPosition;
        float _movementTime = Random.Range(movementTime - 1.1f, movementTime + 1.5f);
        float _stepTime = Random.Range(stepTime - 0.05f, stepTime + 0.2f);
        while (counter < _movementTime && !token.IsCancellationRequested)
        {

            counter += Time.deltaTime;
            step += Time.deltaTime;

            myRectTransform.anchoredPosition = Vector2.Lerp(new Vector2(OriginPosition.x, currentY), new Vector2(TargetX,
                currentY), counter / _movementTime);
            currentY = myRectTransform.anchoredPosition.y;
            currentX = myRectTransform.anchoredPosition.x;
            myRectTransform.anchoredPosition = Vector2.Lerp(new Vector2(currentX, currentY), new Vector2(currentX,
                targetY), step / _stepTime);
            if (step >= _stepTime)
            {
                step = 0f;
                if (zigYDirection)
                {
                    targetY = OriginY + zigThreshold;
                }
                else
                {
                    targetY = OriginY - zigThreshold;
                }
                zigYDirection = !zigYDirection;
            }

            await UniTask.Yield();
        }

        if (!token.IsCancellationRequested)
        {
            isMoving = false;
        }


        if (setInsturctions && !token.IsCancellationRequested)
        {
            UniTask.Void(PatianceSequence);
            bubbleController.gameObject.SetActive(true);

            SetInstructions();
            UpdateBacklightBubble();
            customerAudioSource.clip = HelloClips.GetRandom();
            customerAudioSource.Play();
        }

        if (enqueueAtend == true)
        {
            GameManager.instance.customerQueue.Enqueue(this);
            GameManager.instance.slotsQueue.Enqueue(queuedSlot);
        }



    }





    public ItemInstructions instructions;

    public void SetInstructions()
    {
        instructions = GameManager.CreateRandomInstruction();
        bubbleController.SetImagesAccordingToInsturctions(instructions);
        //UpdateText();
    }

    CancellationTokenSource patianceCTS = new();
    public float initialPatiance = 14f;
    public Color FullPatianceColor;
    public Color EmptyPatianceColor;

    public async UniTaskVoid PatianceSequence()
    {
        patianceCTS?.Cancel();
        bubbleController.PatianceMeter.fillAmount = 0f;
        patianceCTS = new CancellationTokenSource();
        var token = patianceCTS.Token;
        float counter = 0f;

        float maxTime = Mathf.Clamp(initialPatiance - ((float)GameManager.instance.HardshipLevel * 0.8f), 2.5f, 20f);
        while (!token.IsCancellationRequested && counter <= maxTime)
        {
            counter += Time.deltaTime;
            // this should be reversed
            bubbleController.PatianceMeter.fillAmount = counter / maxTime;
            bubbleController.PatianceMeter.color = Color.Lerp(FullPatianceColor,EmptyPatianceColor , counter /
                maxTime);
            await UniTask.Yield();
        }
        if (!token.IsCancellationRequested)
        {
            MoveOutAreaSequence(playSound: false);
        }
    }



    public void OnTapHandler()
    {
        if (isMoving)
        {
            return;
        }
        GiveBox();
    }


    public boxdeb box => boxdeb.instance;
    public Score scoreobj;
    public async void GiveBox()
    {
        var score = 0;

        foreach (var item in box.items)
        {
            if (item.Key == ItemType.Bamba)
            {

                score += item.Value;
            }
            else if (item.Key == ItemType.Pants)
            {

                score += item.Value;
            }
            else if (item.Key == ItemType.Shampoo)
            {

                score += item.Value;
            }
            else if (item.Key == ItemType.Cake)
            {

                score += item.Value;
            }
            else if (item.Key == ItemType.anotherThing)
            {

                score += item.Value * 1;
            }

        }

        //Debug.LogWarning("Score: " + score + "");

        if (score <= 0)
        {
            Shake();
        }
        else
        {


            bubbleController.gameObject.SetActive(false);
            readyBox.SetActive(true);
            if (PerfectMatch)
            {
                GameManager.instance.PlayPerfectAudio(scoreobj.currentMultiplier);
            }
            timer.AddTime(PerfectMatch ? PerfectMatchBonus : NonPerefectMatchTimebonus, PerfectMatch, bubbleController.BubbleRect.anchoredPosition);
            scoreobj.AddScore(score + (PerfectMatch ? 10 : 0), PerfectMatch, bubbleController.BubbleRect.anchoredPosition);


            patianceCTS?.Cancel();
            if (PerfectMatch)
            {
                xLButton.AddStarPower(0.15f);
            }

            if (bubbleController.RainbowMode)
            {
                if (PerfectMatch)
                {
                    xLButton.AddStarPower(2f);
                    bubbleController.RainbowMode = false;
                }
            }

            box.clearBox();
            GameManager.instance.PlayerBox.ClearPlayerBox();
            await UniTask.DelayFrame(1);
            bubbleController.backlightImage.color = bubbleController.normalFillColor;

            MoveOutAreaSequence();
        }
    }

    public float NonPerefectMatchTimebonus = 2f;
    public float PerfectMatchBonus = 5f;

    [Required] public XLButton xLButton;

    public float TimerToScoreMultiplier = 0.20f;
    public Timer timer;
    public TextMeshProUGUI TimerBonusText;

    // void UpdateText()
    // {
    //     var text = "";
    //     text += "Bamba: " + instructions.AmountBamba + "\n";
    //     text += "Bissli: " + instructions.AmountPants + "\n";
    //     text += "Shampoo: " + instructions.AmountShampoo + "\n";
    //     text += "Cake: " + instructions.AmountCake + "\n";
    //
    //     customerText.text = text;
    // }


    public void Shake()
    {
        shaker.Shake(0.5f, 10);
        GameManager.instance.vibrator.OnVibrate2();
    }
    public Shaker shaker;


    bool CheckContents()
    {
        int total = 0;
        foreach (var VARIABLE in box.items)
        {
            total += VARIABLE.Value;
        }

        if (total > 0)
        {
            // match box contents with instructions
            // if match, give score
            // if not, shake
            return (instructions.AmountBamba == box.items[ItemType.Bamba] &&
                    instructions.AmountPants == box.items[ItemType.Pants] &&
                    instructions.AmountShampoo == box.items[ItemType.Shampoo] &&
                    instructions.AmountCake == box.items[ItemType.Cake] &&
                    instructions.amountGreeting == box.items[ItemType.anotherThing]);
        }

        return false;


    }

    bool PerfectMatch = false;

    bool inGame;



    void UpdateBacklightBubble()
    {

        bool match = CheckContents();
        if (!bubbleController.RainbowMode)
        {
            bubbleController.backlightObject.gameObject.SetActive(match);
        }


        PerfectMatch = match;
        if (inGame)
        {
            if (match)
            {
                //UniTask.Void(() => PopButton(bubbleController.BubbleRect));
                UniTask.Void(() => PopButton(bubbleController.BubbleRect));
                GameManager.instance.PlayPerfectReady();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        box.onBoxContentsChange += UpdateBacklightBubble;
        originBubbleScale = bubbleController.BubbleRect.localScale;
    }

    float flickerTime = 0.1f;
    float flickerCounter = 0f;
    Color BaseColor;


}