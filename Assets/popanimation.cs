using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popanimation : MonoBehaviour
{
    Vector3 initialScale;
    Vector3 targetScale;
    public float PopMultiplier = 1.3f;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale;
        targetScale = initialScale * PopMultiplier;
        rectTransform = GetComponent<RectTransform>();
    }

    float popTime = 0.25f;
    float popCounter = 0f;
    bool popInOrOut = false;
    void Update()
    {
        popCounter += Time.deltaTime;
        if (popCounter > popTime)
        {
            popCounter = 0f;
            popInOrOut = !popInOrOut;
        }
        rectTransform.localScale = popInOrOut ? Vector3.Lerp(initialScale, targetScale, popCounter / popTime) : Vector3.Lerp(targetScale, initialScale, popCounter / popTime);
    }
}