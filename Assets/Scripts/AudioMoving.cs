using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMoving : MonoBehaviour
{
    AudioSource motorSound;

    // Start is called before the first frame update
    void Start()
    {
        motorSound = GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey (KeyCode.W) && !motorSound.isPlaying) //有移动键按下并且声音并不是播放状态
        {
        motorSound.Play(); //播放声音
        }
        if(Input.GetKey (KeyCode.A) && !motorSound.isPlaying) //有移动键按下并且声音并不是播放状态
        {
        motorSound.Play(); //播放声音
        }
        if(Input.GetKey (KeyCode.S) && !motorSound.isPlaying) //有移动键按下并且声音并不是播放状态
        {
        motorSound.Play(); //播放声音
        }
        if(Input.GetKey (KeyCode.D) && !motorSound.isPlaying) //有移动键按下并且声音并不是播放状态
        {
        motorSound.Play(); //播放声音
        }
        if(!Input.anyKey && motorSound.isPlaying) //没有任何键按下并且声音是播放状态
        {
        motorSound.Stop(); //停止播放声音
        }
    }
}
