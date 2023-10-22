using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HighScoreObject : MonoBehaviour
{
    [FormerlySerializedAs("text")] [Required] public TextMeshProUGUI nameText;
    [Required] public TextMeshProUGUI scoreText;

    public void UpdateScore(string name, int score)
    {
        nameText.text = name;
        nameText.color = normalFillColor;
        scoreText.text = score.ToString();
        scoreText.color = normalFillColor;
    }




    // Update is called once per frame
    [Required] public Color normalFillColor;
    public float colorShiftTime = 0.5f;
    public float colorShiftCounter = 0f;
    public Color[] RainbowColors = new Color[0];
    int colorIndex = 0;
    int nextColorIndex = 1;


    public bool RainbowMode = false;

    // Update is called once per frame
    void Update()
    {
        if (RainbowMode)
        {
            colorShiftCounter += Time.deltaTime;
            if (colorShiftCounter >= colorShiftTime)
            {
                colorShiftCounter = 0f;
                colorIndex++;
                nextColorIndex++;
                if (colorIndex >= RainbowColors.Length)
                {
                    colorIndex = 0;
                }

                if (nextColorIndex >= RainbowColors.Length)
                {
                    nextColorIndex = 0;
                }
            }

            nameText.color = Color.Lerp(RainbowColors[colorIndex], RainbowColors[nextColorIndex],
                colorShiftCounter / colorShiftTime);
        }
    }
}