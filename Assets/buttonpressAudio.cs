using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class buttonpressAudio : MonoBehaviour
{

    public AudioSource audioSource;

    [Button]
    public void PlayAudio()
    {
        audioSource.Play();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}