using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderFinder : MonoBehaviour
{
    SoundManager soundManager;

    void Awake()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.SetLinq(gameObject);
    }

}
