using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyBox;
using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;
public class PlayfabManager : MonoBehaviour
{



    public static List<string> male_first_names = new List<string>()
    {
        "ארנב",
        "כלב",
        "דוב",
        "חתול",
        "שימפנז",
        "דגיגון",
        "אייל",
        "ינשוף",
        "קוף",
        "לטאה",
        "סנאי",
        "דרור",
        "צב",
        "עכבר",
        "פרפר",
        "פודל",
        "פיל",
        "דבור"
    };

// חיות חמודות, נקבה
    public static List<string> female_first_names = new List<string>()
    {
        "ארנבת",
        "כלבה",
        "דובית",
        "חתולה",
        "גורילה",
        "דגיגה",
        "איילה",
        "ינשופה",
        "קופית",
        "לטאה",
        "סנאית",
        "תנשמת",
        "צבה",
        "עכברת",
        "פרפרה",
        "פודלית",
        "פילה",
        "דבורה"
    };

    public static List<string> female_last_names = new List<string>()
    {
        "מתנדנדת",
        "מתגלגלת",
        "מתחבקת",
        "משחקת",
        "צוחקת",
        "מנמנמת",
        "מחייכת",
        "ישנה",
        "חולמת",
        "עוזרת",
        "מלווה",
        "נוהגת",
        "מטיילת",
        "משחקת",
        "קוראת",
        "כותבת",
        "מציירת"
    };

// שמות תואר פועליים חמודים, זכר
    public static List<string> male_last_names = new List<string>()
    {
        "מתנדנד",
        "מתגלגל",
        "מתחבק",
        "משחק",
        "צוחק",
        "מנמנם",
        "מחייך",
        "ישן",
        "חולם",

        "עוזר",
        "מלווה",
        "נוהג",
        "מטייל",
        "משחק",
        "קורא",
        "כותב",
        "מצייר"
    };


    void getOrSetIDFromPlayerPrefs()
    {
        string s = System.Guid.NewGuid().ToString();

        try
        {
            var fromPrefs = PlayerPrefs.GetString("FALAFEL_ID");
            if (fromPrefs.Length <= 0)
            {
                PlayerPrefs.SetString("FALAFEL_ID", s);
                userID = s;
                return;
            }

            userID = fromPrefs;
            return;
        }
        catch (Exception e)
        {
            userID = s;
        }

        userID = s;
    }


    void Login()
    {
        try
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = userID,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }

            };

            PlayFabClientAPI.LoginWithCustomID(request, OnSuccessLogin, OnError);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    // get leaderboard top ten

    [Button]
    void GetStatistics()
    {
        try
        {
            var request = new GetPlayerStatisticsRequest
            {
                StatisticNames = new List<string>
                {
                    "scores"
                }
            };

            PlayFabClientAPI.GetPlayerStatistics(request, OnGetStatistics, OnError);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());

        }
    }
    [Button]
    public async void GetLeaderBoard(bool isEndGame)
    {
        float counter = 0f;
        while (counter <= 2.5f)
        {
            counter += Time.deltaTime;
            await UniTask.Yield();
        }
            while (UpdateScoreInProgress)
            {
                await UniTask.Yield();
            }

        await UniTask.DelayFrame(24);
        try
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = "scores",
                StartPosition = 0,
                MaxResultsCount = 5
            };

            PlayFabClientAPI.GetLeaderboard(request, (r) => OnGetLeaderBoard(r, isEndGame), (r) => OnGetLeaderBoardError(r,isEndGame));
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            UpdateScoreInProgress = false;
        }
    }

    int lastScore = 0;

    public int CheckifScoreIsInLeaderBoard()
    {
        var _board = LeaderBoard.Leaderboard;
        try
        {
            for (int i = 0; i < _board.Count; i++)
            {
                if (_board[i].StatValue == lastScore && _board[i].DisplayName == userDisplayName)
                {
                    return i;
                }
            }

            return -1;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            return -1;
        }
    }



    GetLeaderboardResult lastLeaderboardResult = new GetLeaderboardResult();



    public GetLeaderboardResult LeaderBoard
    {
        get => lastLeaderboardResult;
    }

    bool UpdateScoreInProgress = false;


    async void OnGetLeaderBoard(GetLeaderboardResult result, bool isEndGame)
    {
        if (result == null )
        {
            Debug.LogWarning($"Leader board is null ");
            GameManager.instance.EndGameStateHandler(MyLeaderBoardState.JustScoreSummary);
            return;
        }

        if (result.Leaderboard.Count <= 0)
        {
            Debug.LogWarning($"Leader board is empty ");
            GameManager.instance.EndGameStateHandler(MyLeaderBoardState.JustScoreSummary);
        }
        lastLeaderboardResult = result;
        await UniTask.DelayFrame(1);
        try
        {
            if (isEndGame)
            {
                while (UpdateScoreInProgress)
                {
                    await UniTask.Yield();
                }
            }

            //Debug.LogWarning("Received the following Leaderboard:");
            if (CheckifScoreIsInLeaderBoard() != -1)
            {
                await UniTask.DelayFrame(1);
                GameManager.instance.EndGameStateHandler(MyLeaderBoardState.DisplayHighScores);
            }
            else
            {
                await UniTask.DelayFrame(1);
                GameManager.instance.EndGameStateHandler(MyLeaderBoardState.JustScoreSummary);
            }
        }
        catch (Exception e)
        {
            GameManager.instance.EndGameStateHandler(MyLeaderBoardState.JustScoreSummary);
            Debug.LogWarning("LeaderBoard error : "  + e.ToString());
        }


    }


    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        //Debug.LogWarning("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            //Debug.LogWarning($"Statistic ({eachStat.StatisticName}): {eachStat.Value}");
        }
    }

    [Button]
    string CreateDisplayName()
    {
        bool isMale = Random.value > 0.5f;
        string _name = "";
        if (isMale)
        {
            _name = male_first_names.GetRandom() + " " + male_last_names.GetRandom() + " " + $"#{Random.Range(1, 99)}";
        }
        else
        {
            _name = female_first_names.GetRandom() + " " + female_last_names.GetRandom() + " " + $"#{Random.Range(1, 99)}";
        }
        //Debug.LogWarning(_name);
        return _name;
    }

    [Button]
    void testUpdateDisplayName()
    {
        try
        {
            var newName = CreateDisplayName();
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = newName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }


    string userID = "";
    string userDisplayName = "";

    void OnSuccessLogin(LoginResult result)
    {
        try
        {
            Debug.LogWarning("Successful login/account create!");
            if (result.NewlyCreated)
            {
                var newName = CreateDisplayName();
                var request = new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = newName
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
            }
            else
            {
                userDisplayName = result.InfoResultPayload.PlayerProfile.DisplayName;
                Debug.LogWarning("Logged in!");
            }
        }
        catch (Exception e)
        {
            userDisplayName = CreateDisplayName();
            Debug.LogWarning("Login error: " + e.ToString());
        }
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        userDisplayName = result.DisplayName;
        Debug.LogWarning("Updated display name!");
    }

    void OnError(PlayFabError error)
    {
        userDisplayName = CreateDisplayName();
        Debug.LogWarning("Error while logging in/creating account!");
        Debug.LogWarning(error.GenerateErrorReport());
    }

    async void OnLeaderBoardUpdateError(PlayFabError error)
    {
        UpdateScoreInProgress = false;
        Debug.LogWarning(error.GenerateErrorReport());
    }

    async void OnGetLeaderBoardError(PlayFabError error, bool isEndGame)
    {
        await UniTask.DelayFrame(1);
        if (isEndGame)
        {
            GameManager.instance.EndGameStateHandler(MyLeaderBoardState.JustScoreSummary);
        }
        else
        {
            GameManager.instance.EndGameStateHandler(MyLeaderBoardState.Error);
        }

        Debug.LogWarning("LeaderboardError: " + error.GenerateErrorReport());
    }

    [Button]
    public void TestAddLeaderBoard()
    {
        var score = 9999;

        SendScoreToLeaderBoard(score);
    }

    public void SendScoreToLeaderBoard(int score)
    {
        lastScore = score;
        UpdateScoreInProgress = true;
        try
        {
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "scores",
                        Value = score,
                    }
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnLeaderBoardUpdateError);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Update Error " + e.ToString());
            UpdateScoreInProgress = false;
        }
    }

    async void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        await UniTask.DelayFrame(1);
        UpdateScoreInProgress = false;
        Debug.LogWarning(result.ToJson());
        // Debug.LogWarning("Successfully updated leaderboard");
    }

    // Start is called before the first frame update
    void Start()
    {
        lastLeaderboardResult.Leaderboard = new List<PlayerLeaderboardEntry>
        {
            new PlayerLeaderboardEntry()
            {
                DisplayName = CreateDisplayName(),
                StatValue = 100
            },
            new PlayerLeaderboardEntry()
            {
                DisplayName = CreateDisplayName(),
                StatValue = 200
            },
            new PlayerLeaderboardEntry()
            {
                DisplayName = CreateDisplayName(),
                StatValue = 300
            },
            new PlayerLeaderboardEntry()
            {
                DisplayName = CreateDisplayName(),
                StatValue = 400
            },
            new PlayerLeaderboardEntry()
            {
                DisplayName = CreateDisplayName(),
                StatValue = 500
            }
        };
        getOrSetIDFromPlayerPrefs();
        Login();
    }

    // Update is called once per frame
    void Update()
    {

    }
}