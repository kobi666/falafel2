using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour
{
    [Required]
    public TMPro.TextMeshProUGUI scoreText;



    void Start() //Fetches the Data at the beginning
    {
        //myScores = GetComponent<HighScores>();
        //StartCoroutine("RefreshHighscores");
    }


    public void SetHighScore(int score)
    {
        scoreText.text = score.ToString();
    }




}