using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReader : MonoBehaviour
{
    public GameObject robotRoot;
    private Collider[] colliders;

    public AudioClip collisionAudioClip;
    private AudioSource collisionAudio;

    public int storageIndex;
    public int storageLength;
    public string[] collisionSelfNames;
    public string[] collisionOtherNames;

    void Start()
    {
        // Audio effect
        collisionAudio = gameObject.AddComponent<AudioSource>();
        collisionAudio.clip = collisionAudioClip;
        collisionAudio.volume = 0.5f;
        
        // Get collision detections
        colliders = robotRoot.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            GameObject parent = collider.gameObject;
            ArticulationCollisionDetection collisionDetection = 
                                           parent.GetComponent<ArticulationCollisionDetection>();
            if (collisionDetection == null)
                collisionDetection = parent.AddComponent<ArticulationCollisionDetection>();
            
            collisionDetection.setParent(robotRoot);
        }

        // To store collision information
        storageIndex = 0;
        storageLength = 5;
        collisionSelfNames = new string[storageLength];
        collisionOtherNames = new string[storageLength];
    }

    void Update()
    {
    }

    public void OnCollision(string self, string other)
    {
        if (!collisionAudio.isPlaying)
        {
            collisionAudio.Play();

            // Temporary
            collisionSelfNames[storageIndex] = self;
            collisionOtherNames[storageIndex] = other;
            storageIndex = (storageIndex+1) % storageLength;
        }
    }
}
