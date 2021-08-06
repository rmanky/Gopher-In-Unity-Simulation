using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticulationWheelController : MonoBehaviour
{
    public float speed = 1.5f;
    public float angularSpeed = 1.5f;
    public ArticulationBody leftWheel;
    public ArticulationBody rightWheel;
    public float wheelTrackLength;
    public float wheelRadius;

    private float xMove;
    private float zMove;
    private float targetLinearSpeed;
    private float targetAngularSpeed;

    private float vRight;
    private float vLeft;

    void Start()
    {
        targetLinearSpeed = 0f;
        targetAngularSpeed = 0f;
    }

    void Update()
    {
        // Get key input
        targetAngularSpeed = Input.GetAxisRaw("Horizontal") * angularSpeed;
        targetLinearSpeed = Input.GetAxisRaw("Vertical") * speed;
    }

    void FixedUpdate()
    {
        if (targetLinearSpeed == 0 && targetAngularSpeed == 0)
        {
            stopWheels();
            return;
        }

        vRight = -targetAngularSpeed*(wheelTrackLength/2) + targetLinearSpeed;
        vLeft = targetAngularSpeed*(wheelTrackLength/2) + targetLinearSpeed;

        setWheelVelocity(leftWheel, vLeft / wheelRadius * Mathf.Rad2Deg);
        setWheelVelocity(rightWheel, vRight / wheelRadius * Mathf.Rad2Deg);
    }

    // Control wheels
    void setWheelVelocity(ArticulationBody wheel, float jointVelocity)
    {
        ArticulationDrive drive = wheel.xDrive;
        drive.target = drive.target + jointVelocity*Time.fixedDeltaTime;
        wheel.xDrive = drive;
    }

    void stopWheels()
    {
        ArticulationDrive ldrive = leftWheel.xDrive;
        ldrive.target = leftWheel.jointPosition[0] * Mathf.Rad2Deg;
        leftWheel.xDrive = ldrive;

        ArticulationDrive rdrive = rightWheel.xDrive;
        rdrive.target = rightWheel.jointPosition[0] * Mathf.Rad2Deg;
        rightWheel.xDrive = rdrive;
    }
}
