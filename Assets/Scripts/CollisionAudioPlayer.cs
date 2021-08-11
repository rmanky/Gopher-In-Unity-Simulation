using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudioPlayer : MonoBehaviour
{
    public AudioClip collisionAudioClip;
    private AudioSource collisionAudio;

    private string selfName;
    private string otherName;
       

    // Start is called before the first frame update
    void Start()
    {
        collisionAudio = gameObject.AddComponent<AudioSource>();
        collisionAudio.clip = collisionAudioClip;

        ArticulationBody AB = GetComponents<Collider>()[0].attachedArticulationBody;
        if (AB != null)
            selfName = AB.gameObject.name;
        else
            selfName = gameObject.name;
    }

    public void SetAudioClip(AudioClip clip)
    {
        collisionAudio.clip = clip;
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.collider.attachedRigidbody;
        ArticulationBody otherAB = collision.collider.attachedArticulationBody;
        if (otherRb != null)
            otherName = otherRb.gameObject.name;
        else if (otherAB != null)
            otherName = otherAB.gameObject.name;
        else
            otherName = collision.collider.name;
        
        Debug.Log(selfName + " hits " + otherName);
        if(!collisionAudio.isPlaying)
        {
            collisionAudio.Play();
        }
    }
}
