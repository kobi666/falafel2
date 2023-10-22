using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    int currentTutorialIndex = 0;

    public Image[] tutorialPages;

    public RectTransform ArrowButtonObject;


    public void StartTutorial()
    {
        currentTutorialIndex = 0;
        ArrowButtonObject.anchoredPosition = NextArrowLocations[currentTutorialIndex].anchoredPosition;
        tutorialPages[currentTutorialIndex].gameObject.SetActive(true);
        ArrowButtonObject.gameObject.SetActive(true);
    }

    public void SwitchToNextTutorialPage()
    {
        tutorialPages[currentTutorialIndex].gameObject.SetActive(false);
        currentTutorialIndex++;

        if (currentTutorialIndex >= tutorialPages.Length)
        {
            currentTutorialIndex = 0;
            ArrowButtonObject.gameObject.SetActive(false);
            return;
        }

        ArrowButtonObject.anchoredPosition = NextArrowLocations[currentTutorialIndex].anchoredPosition;
        tutorialPages[currentTutorialIndex].gameObject.SetActive(true);
    }

    public RectTransform[] NextArrowLocations;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}