using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReader : MonoBehaviour
{
    public GameObject robot;
    private Collider[] colliders;

    public AudioClip collisionAudioClip;
    private AudioSource collisionAudio;

    // Start is called before the first frame update
    void Start()
    {
        collisionAudio = gameObject.AddComponent<AudioSource>();
        collisionAudio.clip = collisionAudioClip;
        
        colliders = robot.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            GameObject parent = collider.gameObject;
            CollisionAudioPlayer audioPlayer = parent.GetComponent<CollisionAudioPlayer>();
            if (audioPlayer == null)
            {
                audioPlayer = parent.AddComponent<CollisionAudioPlayer>();
            }
            audioPlayer.setAudioSource(collisionAudio);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
