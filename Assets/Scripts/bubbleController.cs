using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class bubbleController : MonoBehaviour
{

    public Image[] BamabaImages = new Image[3];
    public Image[] Shampooimages = new Image[3];
    public Image[] pantsImages = new Image[3];
    public Image[] CakeImages = new Image[3];
    public Image GreetingImage;

    [Required] public Image PatianceMeter;





    [Required]
    public RectTransform BubbleRect;

    [Required]
    public GameObject backlightObject;

    public RectTransform backlightRectTransform;



    public void SetImagesAccordingToInsturctions(ItemInstructions instructions)
    {
        for (int i = 0; i < BamabaImages.Length; i++)
        {
            BamabaImages[i].gameObject.SetActive(instructions.AmountBamba > i);
        }

        for (int i = 0; i < Shampooimages.Length; i++)
        {
            Shampooimages[i].gameObject.SetActive(instructions.AmountShampoo > i);
        }

        for (int i = 0; i < pantsImages.Length; i++)
        {
            pantsImages[i].gameObject.SetActive(instructions.AmountPants > i);
        }

        for (int i = 0; i < CakeImages.Length; i++)
        {
            CakeImages[i].gameObject.SetActive(instructions.AmountCake > i);
        }
        GreetingImage.gameObject.SetActive(instructions.amountGreeting > 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        backlightRectTransform = backlightObject.GetComponent<RectTransform>();
    }

    [Required] public Color normalFillColor;
    public float colorShiftTime = 0.5f;
    public float colorShiftCounter = 0f;
    public Color[] RainbowColors = new Color[0];
    int colorIndex = 0;
    int nextColorIndex = 1;
    public Image backlightImage;
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

            backlightImage.color = Color.Lerp(RainbowColors[colorIndex], RainbowColors[nextColorIndex],
                colorShiftCounter / colorShiftTime);
        }
    }
}