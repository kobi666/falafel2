using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{


    public TextMeshProUGUI scoreText;

    public int score;

    void Start()
    {
        OriginPerfectScale = PerfectMultiplierRect.localScale;
    }


    [Required]
    public Image PerfectMultiplierImage;
    [Required]
    public RectTransform PerfectMultiplierRect;

    public Sprite[] MultiplierSprites;
    int maxMultiplier = 5;
    public int currentMultiplier = 1;
    void UpdateMultiplier(bool perfect)
    {
        PerfectMultiplierImage.gameObject.SetActive(perfect);
        currentMultiplier = perfect ? Mathf.Clamp(currentMultiplier + 1, 1, maxMultiplier) : 1;
        PerfectMultiplierImage.sprite = MultiplierSprites[Mathf.Clamp(currentMultiplier - 2, 0,3)];
        if (perfect)
        {
            UniTask.Void(() => PopButton(PerfectMultiplierRect));
        }
    }

    public float PopScale = 1.2f;
    public float popTime = 0.12f;
    Vector3 OriginPerfectScale;
    public async UniTaskVoid PopButton(RectTransform transform)
    {
        var originScale = OriginPerfectScale;
        var targetScale = originScale * (PopScale + (0.05f * currentMultiplier));
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
    }

    public void ResetScore()
    {
        UpdateMultiplier(false);
        score = 0;
        scoreText.text = 0.ToString();
    }

    public void AddScore(int amount, bool perfect, Vector2 position)
    {
        int scoreAddition = currentMultiplier > 0 ? amount * currentMultiplier : amount;
        score += scoreAddition;
        UniTask.Void(() => GameManager.instance.ScoreBonusAnimation($"+{(scoreAddition).ToString()}", position, perfect));
        UpdateMultiplier(perfect);

        scoreText.text = score.ToString();
    }
    // Start is called before the first frame update

}