using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider soundFXSlider;
    [SerializeField] private Slider musicSlider;

    void Start()
    {
        // load the saved values from a folder 
        // if players dont have any saved values then it just defaults to 1
        float masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        float soundFXVolume = PlayerPrefs.GetFloat("soundFXVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);

        // if change scene then set the slider to correct value
        SetMasterVolume(masterVolume);
        SetSoundFXVolume(soundFXVolume);
        SetMusicVolume(musicVolume);
        masterSlider.value = masterVolume;
        soundFXSlider.value = soundFXVolume;
        musicSlider.value = musicVolume;
    }
    
    /// <summary>
    /// all of these methods below are
    /// used in the ui slider in the Settings ui
    /// </summary>
    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20);
        
        // save the value when changed
        PlayerPrefs.SetFloat("masterVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20);
        
        PlayerPrefs.SetFloat("soundFXVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20);
        
        PlayerPrefs.SetFloat("musicVolume", level);
    }
}