using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip boom;
    [SerializeField] private AudioClip bigBoom;
    [SerializeField] private AudioClip blup;
    [SerializeField] private AudioClip dragDrop;

    private AudioSource AC;

    void Awake()
    {
        AC = gameObject.GetComponent<AudioSource>();
    }

    public void PlayBoom()
    {
        AC.PlayOneShot(boom);
    }

    public void PlayBigBoom()
    {
        AC.PlayOneShot(bigBoom);
    }

    public void PlayBlup()
    {
        AC.PlayOneShot(blup);
    }

    public void PlayDragDrop() 
    {
        AC.PlayOneShot(dragDrop);
    }
}
