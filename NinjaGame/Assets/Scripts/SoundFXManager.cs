using UnityEngine;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;
    public static SoundFXManager Instance { get ; private set; }
    private List<AudioSource> audios = new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// creates a gameobject with an audiosource to play a sound. If repeat, it
    /// returns an audioSource so you can track it if you are repeating the audio
    /// to delete it later.
    /// </summary>
    public AudioSource PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume, bool repeat)
    {

        // spawnin gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // if repeat then repeats audio until stopped
        if (repeat)
        {
            audioSource.loop = true;
            audios.Add(audioSource);
        } 

        // assign the audioClip and volume
        audioSource.clip = audioClip;
        audioSource.volume = volume;

        // play sound
        audioSource.Play();

        // get length and destroy when done
        if (!repeat)
        {
            float clipLen = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLen);
        }

        return audioSource;
    }

    /// <summary>
    /// if PlaySFTXClip repeat was true this is how you remove the repeating audio
    /// </summary>
    public void DestroyAudioSource(AudioSource audioSource, bool waitForSoundToFinish)
    {
        if (audioSource == null) return;

        // deletes the sound from the list of repeating audios playing
        audios.Remove(audioSource);
        
        // gets clip len
        float clipLen = 0f;

        // if waitForsoundToFinish is true then it sets clipLen to audio len
        if (waitForSoundToFinish)
        {
            clipLen = audioSource.clip.length;
        }

        // destroy audioSource gameobject
        Destroy(audioSource.gameObject, clipLen);
    }
}
