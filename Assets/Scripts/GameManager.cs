using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyBox;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-200)]
public class GameManager : MonoBehaviour
{

    [Required] public AudioSource PerfectReadySource;

    public void PlayPerfectReady()
    {
        PerfectReadySource.Play();
    }

    public int HardshipLevel = 0;






    [Required] public PlayfabManager playfabManager;
    async void UpdatePlayerScore(int score)
    {
        playfabManager.SendScoreToLeaderBoard(score);
        await UniTask.DelayFrame(10);
        playfabManager.GetLeaderBoard(isEndGame:true);
        EndGameStateHandler(MyLeaderBoardState.Loading);
        ScoreSummaryText.text = score.ToString();
    }


    [Button]
    public void TestEndGameState(MyLeaderBoardState state)
    {
        EndGameStateHandler(state);
    }

    [Required] public HighScoreObject[] highScoreTexts;

    void UpdateLeaderBoardText()
    {
        var board = playfabManager.LeaderBoard.Leaderboard;
        var newHighScore = playfabManager.CheckifScoreIsInLeaderBoard();
        newHighScoreObject.gameObject.SetActive(newHighScore != -1);

        for (int i = 0; i < board.Count; i++)
        {
            highScoreTexts[i].gameObject.SetActive(true);
            highScoreTexts[i].UpdateScore(board[i].DisplayName , board[i].StatValue);
            highScoreTexts[i].RainbowMode = (i == newHighScore);
        }
    }

    [Required] public MenuController MenuController;

    [Required] public AudioHighPassFilter musicFIlter;

    public TextMeshProUGUI TimeBonusText;
    public TextMeshProUGUI ScoreBonusText;
    public RectTransform scoreBonusPosition;
    public RectTransform timeBonusPosition;

    CancellationTokenSource timeBonusCTS = new CancellationTokenSource();
    CancellationTokenSource scoreBonusCTS = new CancellationTokenSource();
    public Color TimeBonusColor, ScoreBonusColor;


    float TextAnimationTime = 1.1f;

    public async UniTaskVoid TimeBonusAnimation(string text, Vector2 position, bool perfect = false )
    {
        // create sequence to put object at position, float it up and fade it out
        TimeBonusText.text = text;
        //TimeBonusText.gameObject.SetActive(true);
        TimeBonusText.rectTransform.anchoredPosition = position + (Vector2.right * 400);
        float counter = 0f;
        timeBonusCTS?.Cancel();
        timeBonusCTS = new();
        var originPos = TimeBonusText.rectTransform.anchoredPosition;
        var targetPos = originPos + new Vector2(0, 100);
        var token = timeBonusCTS.Token;
        Color targetColor = perfect ? Color.yellow : TimeBonusColor;
        while (counter < 1f && !token.IsCancellationRequested)
        {
            counter += Time.deltaTime;
            TimeBonusText.rectTransform.anchoredPosition = Vector2.Lerp(originPos, targetPos, counter);
            TimeBonusText.color = Color.Lerp(targetColor, Color.clear, counter);
            await UniTask.Yield();
        }
    }

    public async UniTaskVoid ScoreBonusAnimation(string text, Vector2 position, bool perfect = false)
    {
        // create sequence to put object at position, float it up and fade it out
        ScoreBonusText.text = text;
        var originPos = position + (Vector2.left * 500);
        //ScoreBonusText.gameObject.SetActive(true);
        ScoreBonusText.rectTransform.anchoredPosition = originPos;
        float counter = 0f;
        scoreBonusCTS?.Cancel();
        scoreBonusCTS = new();
        var targetPos = originPos + new Vector2(0, 100);
        Color targetColor = perfect ? Color.yellow : TimeBonusColor;
        var token = scoreBonusCTS.Token;
        while (counter < 1f && !token.IsCancellationRequested)
        {
            counter += Time.deltaTime;
            ScoreBonusText.rectTransform.anchoredPosition = Vector2.Lerp(originPos, targetPos, counter
                );
            ScoreBonusText.color = Color.Lerp(targetColor, Color.clear, counter);
            await UniTask.Yield();
        }
    }

    public static GameManager instance;
    [FormerlySerializedAs("PlayerBoxImage")] public playerBoxController PlayerBox;
    [Required]
    public FullScreenWebGLExample fullscreen;

    public RectTransform[] customerSlots = new RectTransform[0];

    public Queue<RectTransform> slotsQueue = new Queue<RectTransform>();


    [Required] public RectTransform Cat;

    public Timer timer;
    public Score ScoreObj;

    public TapMode mode;

    public List<ItemButton> items = new();
    [ShowInInspector]
    Dictionary<int, ItemButton> itemsDict = new();






    public CancellationTokenSource InGameCTS = new CancellationTokenSource();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        foreach (var VARIABLE in items)
        {
            itemsDict.Add(VARIABLE.GetInstanceID(), VARIABLE);
        }
    }

    public customerController[] customers;
    [FormerlySerializedAs("availableCustomers")] [ShowInInspector]
    public Queue<customerController> customerQueue = new();
    [Required]
    public newBoxController newBoxController;

    public HashSet<int> EnqueuedCustomers = new HashSet<int>();

    public void EnqueueCustomer(customerController cust)
    {
        if (EnqueuedCustomers.Add(cust.GetInstanceID()))
        {
            customerQueue.Enqueue(cust);
        }
    }

    public Color ItemBoxOutlineMinimumColor;
    public Color ItemBoxOutlineMaximumColor;

    public bool inGame = false;
    [Required] public XLButton XlButton;
    [Required] public boxdeb _boxdeb;
    public bool firstTime = true;
    public bool FirstBox = true;

    int firstTimeForTutorial = 0;
    public AudioClip[] PerfectAudioClips;
    [Required]
    public AudioSource PerfectAudioSource;

    [Required] public buttonVibrator vibrator;

    public void PlayPerfectAudio(int perfectLevel)
    {
        PerfectAudioSource.PlayOneShot(PerfectAudioClips[Mathf.Clamp(perfectLevel, 0, PerfectAudioClips.Length - 1)]);
    }

    [Required] public AudioSource musicAudioSource;
    [Required] public TutorialManager tutorialManager;
    public async void StartGame()
    {
        if (firstTimeForTutorial == 0)
        {
            PlayerPrefs.SetInt("FIRST_TIME_TUTORIAL", 1);
            firstTimeForTutorial = 1;
            tutorialManager.StartTutorial();
            return;
        }

        if (firstTime)
        {
            MenuController.MoveMainMenu(false);
            firstTime = false;
        }
        HardshipLevel = 0;
        musicFIlter.cutoffFrequency = 10;

        if (mode == TapMode.FillItems)
        {
            RachelCookies.SetItemFillMode();
        }
        //fullscreen.enterfullscreen();
        _boxdeb.clearBox();
        PlayerBox.ClearPlayerBox();
        InGameCTS?.Cancel();
        musicAudioSource.pitch = 1f;
        InGameCTS = new();

        UniTask.Void(PlayerBox.OpenBox);
        XlButton.AddStarPower(-5f);
        XlButton.FillForce();
        customerQueue.Clear();
        EnqueuedCustomers.Clear();
        var token = InGameCTS.Token;

        ScoreObj.ResetScore();
        timer.AddTime(-9999, false, new Vector2(-9999, -9999));
        customerQueue.Clear();
        customers.Shuffle();
        customerSlots.Shuffle();
        foreach (var cust in customers)
        {
            cust.movementCTS?.Cancel();
            cust.myRectTransform.anchoredPosition = cust.originPosition.anchoredPosition;
            EnqueueCustomer(cust);
        }
        slotsQueue.Clear();
        foreach (var VARIABLE in customerSlots)
        {
            slotsQueue.Enqueue(VARIABLE);
        }

        InGameCTS = new();
        inGame = true;
        currSpawnTime = 1.5f;

        await timer.StartTimerSequence();
        if (!token.IsCancellationRequested)
        {
            EndGame();
        }
    }



    void EndGame()
    {
        inGame = false;
        InGameCTS?.Cancel();
        InGameCTS = new();
        XlButton.AddStarPower(-5f);
        ScoreObj.AddScore(0, false, new Vector2(9999,9999));
        bool direction = false;
        foreach (var customerController in customers)
        {

            direction = !direction;
            customerController.bubbleController.gameObject.SetActive(false);
            UniTask.Void(() => customerController.MoveInZigZag(direction ? customerController.originPosition.anchoredPosition : customerController.targetPosition.anchoredPosition, false) );
        }

        string userName = "שימי די";
        try
        {
            UpdatePlayerScore(ScoreObj.score);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            EndGameStateHandler(MyLeaderBoardState.JustScoreSummary);
        }

        //musicFIlter.cutoffFrequency = 1500;
        MenuController.MoveGameOverMenu(true);



    }

    [Required] public TextMeshProUGUI newHighScoreText;
    [Required] public TextMeshProUGUI HighScoreText;





    public static ItemInstructions CreateRandomInstruction()
    {
        var ins = new ItemInstructions()
        {
            AmountBamba = Mathf.Clamp(Random.Range(1, 10) / 3, 0,3),
            AmountPants = Mathf.Clamp(Random.Range(1, 10) / 3, 0, 3),
            AmountShampoo = Mathf.Clamp(Random.Range(1, 10) / 3, 0, 3),
            AmountCake = Mathf.Clamp(Random.Range(1, 10) / 3, 0, 3),
            amountGreeting = Mathf.Clamp(Random.Range(1, 10) / 6, 0, 1)
        };
        return hasAnyItmes(ins) ? ins : CreateRandomInstruction();

    }

    [Required] public RectTransform LoadingThingy;
    [Required] public TextMeshProUGUI ScoreSummaryText;
    [Required] public GameObject highscoreTextsParent;
    [Required] public TextMeshProUGUI newHighScoreObject;
    [Required] public TextMeshProUGUI LeadrBoardErrorText;


    public void EndGameStateHandler(MyLeaderBoardState state)
    {
        //Debug.LogError(state.ToString());
        switch (state)
        {
            case MyLeaderBoardState.Loading:
                LoadingThingy.gameObject.SetActive(true);
                ScoreSummaryText.gameObject.SetActive(false);
                highscoreTextsParent.SetActive(false);
                newHighScoreObject.gameObject.SetActive(false);
                LeadrBoardErrorText.gameObject.SetActive(false);
                break;
            case MyLeaderBoardState.JustScoreSummary:
                ScoreSummaryText.gameObject.SetActive(true);
                LoadingThingy.gameObject.SetActive(false);
                highscoreTextsParent.SetActive(false);
                newHighScoreObject.gameObject.SetActive(false);
                LeadrBoardErrorText.gameObject.SetActive(false);
                break;
            case MyLeaderBoardState.DisplayHighScores:
                UpdateLeaderBoardText();
                LoadingThingy.gameObject.SetActive(false);
                ScoreSummaryText.gameObject.SetActive(false);
                highscoreTextsParent.SetActive(true);
                LeadrBoardErrorText.gameObject.SetActive(false);
                break;
            case MyLeaderBoardState.Error:
                LoadingThingy.gameObject.SetActive(false);
                ScoreSummaryText.gameObject.SetActive(false);
                highscoreTextsParent.SetActive(false);
                newHighScoreObject.gameObject.SetActive(false);
                LeadrBoardErrorText.gameObject.SetActive(true);
                break;
        }
    }

    static bool hasAnyItmes(ItemInstructions ins)
    {
        return ins.AmountBamba + ins.AmountCake + ins.AmountPants + ins.AmountShampoo + ins.amountGreeting >= 2;
    }

    public string DeviceID;
    // Start is called before the first frame update
    void Start()
    {
        musicAudioSource.pitch = 1f;
        TimeBonusColor = TimeBonusText.color;
        ScoreBonusColor = ScoreBonusText.color;
        MenuController.MoveMainMenu(true);
        DeviceID = SystemInfo.deviceUniqueIdentifier;
            //Debug.LogError(DeviceID);
        var _firstForTutorial = PlayerPrefs.GetInt("FIRST_TIME_TUTORIAL", 0);
        firstTimeForTutorial = _firstForTutorial;
    }

    public rachelCookies RachelCookies;
    public float spawnCounter = 0;
    public float SpwanMinTimeSeconds, SpawnMaxTimeSeconds = 10f;
    public float SpawnTime => Random.Range(SpwanMinTimeSeconds, SpawnMaxTimeSeconds) - ((float)HardshipLevel * 0.5f);
    float currSpawnTime = 5f;

    public float CatTimer = 0f;
    public float CatTimerMax = 20f;
    public bool InCatMode = false;

    // Update is called once per frame
    void Update()
    {
        if (inGame)
        {
            spawnCounter += Time.deltaTime;
            if (spawnCounter >= currSpawnTime)
            {
                spawnCounter = 0;
                currSpawnTime = SpawnTime;
                if (customerQueue.Count > 0 && slotsQueue.Count > 0)
                {
                    var cust = customerQueue.Dequeue();
                    EnqueuedCustomers.Remove(cust.GetInstanceID());

                    cust.MoveToInAreaSequence(slotsQueue.Dequeue());
                }
            }

            // if (!InCatMode)
            // {
            //     CatTimer += Time.deltaTime;
            //     if (CatTimer >= CatTimerMax)
            //     {
            //         CatTimer = 0f;
            //         InitializeCat();
            //     }
            // }
        }

        [Button]
        void asdasd()
        {
            var ass = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (var a in ass)
            {
                a.playOnAwake = false;
            }
        }


    }
}


[System.Serializable]
public struct ItemInstructions
{
    public int AmountBamba;
    [FormerlySerializedAs("AmountBissli")] public int AmountPants;
    public int AmountShampoo;
    public int AmountCake;
    public int amountGreeting;
}


[System.Serializable]
public enum TapMode
{
    AddItemsToBox,
    FillItems,

}

[System.Serializable]
public enum MyLeaderBoardState
{
    DisplayHighScores,
    Loading,
    JustScoreSummary,
    Error
}