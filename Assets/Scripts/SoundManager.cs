using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    private float musicVolume = 0.75f;  
    private float soundVolume = 0.75f;
    private float uiSoundVolume = 0.75f;
    private AudioSource audioSrc;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider UISoundSlider;
    [SerializeField] private AudioSource UIPlayer;
    [SerializeField] private AudioClip UISound;
    [SerializeField] private AudioMixerGroup mixer;

    void Awake()
    {
        audioSrc = gameObject.GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        InitializeManager();
    }

    private void Update()
    {
        if (musicSlider != null & soundSlider != null)
        {
            musicVolume = musicSlider.value;
            soundVolume = soundSlider.value;
            uiSoundVolume = UISoundSlider.value;
        }
        mixer.audioMixer.SetFloat("music", Mathf.Log10(musicVolume) * 60);
        mixer.audioMixer.SetFloat("sound", Mathf.Log10(soundVolume) * 60);
        mixer.audioMixer.SetFloat("UI", Mathf.Log10(uiSoundVolume) * 60);
    }

    private void InitializeManager()
    {
        musicVolume = PlayerPrefs.GetFloat("Music");
        soundVolume = PlayerPrefs.GetFloat("Sound");
        uiSoundVolume = PlayerPrefs.GetFloat("UISound");

        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
        UISoundSlider.value = uiSoundVolume;

        audioSrc.clip = Resources.Load("Music1") as AudioClip;
        audioSrc.Play();
    }

    private void playSound(float pitch)
    {
        UIPlayer.pitch = pitch;
        UIPlayer.PlayOneShot(UISound);
    }

    public static void PlaySound(float pitch)
    {
        instance.playSound(pitch);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("Music", musicVolume);
        PlayerPrefs.SetFloat("Sound", soundVolume);
        PlayerPrefs.SetFloat("UISound", uiSoundVolume);
        PlayerPrefs.Save();
    }

    public void SetLinq(GameObject obj)
    {
        if(obj.CompareTag("music"))
        {
            musicSlider = obj.GetComponent<Slider>();
            musicSlider.value = musicVolume;
            //Debug.Log("music");
        }
        else if (obj.CompareTag("sound"))
        {
            soundSlider = obj.GetComponent<Slider>();
            soundSlider.value = soundVolume;
            //Debug.Log("sound");
        }
        else if (obj.CompareTag("UI"))
        {
            UISoundSlider = obj.GetComponent<Slider>();
            UISoundSlider.value = uiSoundVolume;
        }
    }

    private void OnDestroy()
    {
        SaveVolume();
    }
}
