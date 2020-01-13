using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnimScript : MonoBehaviour
{
    Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        anim.PlayQueued("HitIndication", QueueMode.CompleteOthers);
    }
}
