using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
    public List<AudioSource> AudioSources = new();

    public Sprite MusicOn;
    public Sprite MusicOff;


    bool musicOn = true;

    public Image MusicButtonImage;

    public void ToggleMusic()
    {
        musicOn = !musicOn;
        AudioSources.ForEach(x => x.mute = !musicOn);
        MusicButtonImage.sprite = musicOn ? MusicOn : MusicOff;
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