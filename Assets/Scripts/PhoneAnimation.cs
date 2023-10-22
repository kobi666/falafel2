using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneAnimation : MonoBehaviour
{

    public Sprite[] animation = new Sprite[100];

    public Image image;
    // Start is called before the first frame update
    void Start()
    {

    }
    int imageIndex = 0;

    float timebetweenFrames = 0.02f;
    float counter = 0;
    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > timebetweenFrames)
        {
            counter = 0;
            imageIndex++;
            if (imageIndex >= animation.Length)
            {
                imageIndex = 0;
            }
            image.sprite = animation[imageIndex];
        }

    }
}