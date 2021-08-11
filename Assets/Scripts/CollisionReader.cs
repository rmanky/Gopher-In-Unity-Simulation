using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReader : MonoBehaviour
{
    public GameObject robot;
    private Collider[] colliders;

    public AudioClip collisionAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        colliders = robot.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            GameObject parent = collider.gameObject;
            
            // Add audio
            /*
            if (collisionAudioClip != null && 
                parent.GetComponent<CollisionAudioPlayer>() == null)
            {
                CollisionAudioPlayer audioPlayer = parent.AddComponent<CollisionAudioPlayer>();
                audioPlayer.collisionAudioClip = collisionAudioClip;
            }
            */
            CollisionAudioPlayer audioPlayer = parent.GetComponent<CollisionAudioPlayer>();
            if (collisionAudioClip != null && audioPlayer == null)
            {
                audioPlayer = parent.AddComponent<CollisionAudioPlayer>();
                audioPlayer.collisionAudioClip = collisionAudioClip;
            }
            else if (audioPlayer != null)
            {
                audioPlayer.SetAudioClip(collisionAudioClip);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
