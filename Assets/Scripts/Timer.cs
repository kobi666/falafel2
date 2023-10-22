using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;


public class Timer : MonoBehaviour
{

    public float currentTime;
    public float initalTime;
    public TextMeshProUGUI timerText;

    CancellationTokenSource myCTS;

    bool CheckUnderPressure(float currentTime)
    {
        return currentTime < 8f;
    }

    bool isUnderPressure = false;


    public void AddTime(float time, bool perfect, Vector2 position)
    {

        UniTask.Void(() => GameManager.instance.TimeBonusAnimation($"+{time.ToString("F1")}", position, perfect));
        currentTime += time;
    }

    public float totalGameTime = 0f;

    public float hardshipIncreaseTime = 45f;




    public async UniTask StartTimerSequence()
    {
        totalGameTime = 0f;
        GameManager.instance.HardshipLevel = 0;
        myCTS?.Cancel();
        myCTS = CancellationTokenSource.CreateLinkedTokenSource(GameManager.instance.InGameCTS.Token);
        var token = myCTS.Token;
        //Debug.LogWarning(token.IsCancellationRequested);
        currentTime = initalTime;
        while (currentTime >= 0 && !token.IsCancellationRequested)
        {
            currentTime -= Time.deltaTime;
            totalGameTime += Time.deltaTime;
            if (totalGameTime >= hardshipIncreaseTime)
            {
                GameManager.instance.HardshipLevel = Mathf.Clamp(GameManager.instance.HardshipLevel+1, 0, 12);
                float div = GameManager.instance.HardshipLevel / 12f;

                totalGameTime = 0f;
            }
            bool _isUnderPressure = CheckUnderPressure(currentTime);
            if (isUnderPressure != _isUnderPressure)
            {
                isUnderPressure = _isUnderPressure;
                if (isUnderPressure)
                {
                    timerText.color = Color.red;
                }
                else
                {
                    timerText.color = Color.yellow;
                }
            }
            timerText.text = Mathf.Clamp(currentTime,0f, 999f).ToString("F2");
            await UniTask.Yield();
        }

        timerText.text = Mathf.Clamp(currentTime, 0f, 999f).ToString("F2");
    }

    async void StartTimer()
    {
        await StartTimerSequence();
    }

    // Start is called before the first frame update
    void Start()
    {
        myCTS = CancellationTokenSource.CreateLinkedTokenSource(GameManager.instance.InGameCTS.Token);


    }

    // Update is called once per frame
    void Update()
    {

    }
}