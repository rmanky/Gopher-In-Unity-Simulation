using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{

    AudioSource tickSound;

    // Start is called before the first frame update
    void Start()
    {
        tickSound = GetComponent<AudioSource>();
    }


    void OnCollisionEnter(Collision collsionInfo)
    {
        Debug.Log("You hit " + collsionInfo.collider.name);
        if(!tickSound.isPlaying)
        {
        tickSound.Play();
        }
    }

}
