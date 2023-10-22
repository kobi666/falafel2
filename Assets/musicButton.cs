using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicButton : MonoBehaviour
{
    public Sprite MusicOn;
    public Sprite MusicOff;

    public AudioSource MusicAudioSource;
    bool musicOn = true;

    public Image MusicButtonImage;
    public void ToggleMusic()
    {
        musicOn = !musicOn;
        MusicAudioSource.mute = !musicOn;
        MusicButtonImage.sprite = musicOn ? MusicOn : MusicOff;
    }
}