using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Globalization;
using CsvHelper;

public class DataRecorder : MonoBehaviour
{
    public GameObject robot;
    private StateReader stateReader;
    private Laser laser;
    private CollisionReader collisionReader;
    private int collisionStorageIndex;

    public float recordRate = 10;
    public bool updateData;
    private bool isRecording;
    public float[] states;
    public string[] collisions;
    public float[] task;

    private float twoPI;

    void Start()
    {
        // Initialization
        states = new float[18];
        collisions = new string[2];
        task = new float[12]; 
        isRecording = false;
        updateData = false;
        if (robot != null)
            setRobot(robot);

        // Constant
        twoPI = 2 * Mathf.PI;

        InvokeRepeating("RecordData", 1f, 1/recordRate);
    }

    void Update()
    {
    }

    public void setRobot(GameObject robot)
    {
        this.robot = robot;
        stateReader = robot.GetComponentInChildren<StateReader>();
        laser = robot.GetComponentInChildren<Laser>();
        collisionReader = robot.GetComponentInChildren<CollisionReader>();
        collisionStorageIndex = 0;
        
        states = new float[18];
        collisions = new string[2];
        task = new float[12]; 

        isRecording = false;
        updateData = false;
    }

    public void StartRecording()
    {
        isRecording = true;
    }

    public void StopRecording()
    {

        isRecording = false;
    }

    private void RecordData()
    {
        if (robot == null)
            return;
        if(!isRecording && !updateData)
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
        states[10] = stateReader.positions[2];
        states[11] = ToFLUEuler(stateReader.positions[3]);
        states[12] = stateReader.velocities[2];
        states[13] = -stateReader.velocities[3];
        // arm camera joint
        states[14] = stateReader.positions[22];
        states[15] = ToFLUEuler(stateReader.positions[21]);
        states[16] = stateReader.velocities[22];
        states[17] = -stateReader.velocities[21];

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
