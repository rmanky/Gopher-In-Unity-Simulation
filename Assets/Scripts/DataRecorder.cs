using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRecorder : MonoBehaviour
{
    public float recordRate;

    public GameObject robot;
    private StateReader stateReader;
    private Laser laser;
    private CollisionReader collisionReader;
    private int collisionStorageIndex;

    private bool isRecording;
    public float[] states;
    public string[] collisions;
    public float[] task;

    private float twoPI;

    void Start()
    {
        stateReader = robot.GetComponentInChildren<StateReader>();
        laser = robot.GetComponentInChildren<Laser>();
        collisionReader = robot.GetComponentInChildren<CollisionReader>();
        collisionStorageIndex = 0;

        states = new float[18];
        collisions = new string[2];
        task = new float[12];
        twoPI = 2 * Mathf.PI;

        isRecording = false;
        InvokeRepeating("RecordData", 1f, 1/recordRate);
    }
    
    void Update()
    {
    }

    private void StartRecording()
    {
        isRecording = true;
    }

    private void StopRecording()
    {
        isRecording = false;
    }

    private void RecordData()
    {
        if(!isRecording)
            return;
        
        // Record state
        // t
        states[0] = Time.time;
        // pose
        states[1] = stateReader.position[2];
        states[2] = -stateReader.position[0];
        states[3] = ToFLUEuler(stateReader.eulerRotation[1] * Mathf.Deg2Rad);
        // vel
        states[4] = stateReader.linearVelocity[2];
        states[5] = -stateReader.angularVelocity[1];
        // obs dis
        int obsMinI = GetLaserMinIndex(laser.ranges);
        int humMinI = GetLaserMinIndex(laser.ranges);
        states[6] = laser.ranges[obsMinI];
        states[7] = laser.directions[obsMinI];
        states[8] = laser.ranges[humMinI]; // Human
        states[9] = laser.directions[humMinI]; // Human
        // main camera joint
        states[10] = stateReader.positions[28];
        states[11] = ToFLUEuler(stateReader.positions[29]);
        states[12] = stateReader.velocities[28];
        states[13] = -stateReader.velocities[29];
        // arm camera joint
        states[14] = stateReader.positions[20];
        states[15] = ToFLUEuler(stateReader.positions[19]);
        states[16] = stateReader.velocities[20];
        states[17] = -stateReader.velocities[19];

        // Record collision
        if (collisionStorageIndex != collisionReader.storageIndex)
        {
            // collision
            collisions[0] = collisionReader.collisionSelfNames[collisionStorageIndex];
            collisions[1] = collisionReader.collisionOtherNames[collisionStorageIndex];

            collisionStorageIndex = (collisionStorageIndex+1) % collisionReader.storageLength;
        }
    }   
    private float ToFLUEuler(float angle)
    {
        // Change direction
        angle = twoPI - angle;

        // Wrap to [-pi to pi]
        angle =  angle % twoPI; 
        // positive remainder, 0 <= angle < 2pi  
        angle = (angle + twoPI) % twoPI;
        // -180 < angle <= 180  
        if (angle > Mathf.PI)  
            angle -= twoPI; 
        return angle;
    }
    private int GetLaserMinIndex(float[] ranges)
    {
        int index = 0;
        float min = 100f;
        for(int i = 0; i < ranges.Length; ++i)
            if (ranges[i] != 0f && ranges[i] < min)
            {
                min = ranges[i];
                index = i;
            }
        return index;
    }
}
