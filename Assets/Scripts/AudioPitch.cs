using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPitch : MonoBehaviour
{

    AudioSource audioSource;

    public float minimumPitch = 0.5f;
    public float maximumPitch = 1f;

    public Rigidbody Robot;

    // Start is called before the first frame update
    void Start()
    {
        Robot = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = minimumPitch;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Robot.velocity.magnitude * 300000;
        if (speed < minimumPitch)
        {
            audioSource.pitch = minimumPitch;
        }
        else if(speed > maximumPitch)
        {
            audioSource.pitch = maximumPitch;
        }else
        {
            audioSource.pitch = speed;
        }
        print (speed);
        print (0);
    }
}
