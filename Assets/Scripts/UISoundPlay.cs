using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundPlay : MonoBehaviour
{
    public void Play()
    {
        //Debug.Log("Enter");
        SoundManager.PlaySound(1);
    }

    public void PlaySlider()
    {
        SoundManager.PlaySound(1.8f);
    }
}
