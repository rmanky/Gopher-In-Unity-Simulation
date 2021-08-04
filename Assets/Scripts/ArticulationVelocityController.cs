using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticulationVelocityController : MonoBehaviour
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
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");
        targetLinearSpeed = zMove * speed;
        targetAngularSpeed = xMove * angularSpeed;
    }

    void FixedUpdate()
    {
        vRight = -targetAngularSpeed*(wheelTrackLength/2) + targetLinearSpeed;
        vLeft = targetAngularSpeed*(wheelTrackLength/2) + targetLinearSpeed;

        if (vRight != 0f && vLeft != 0f)
        {
            setLeftWheelVelocity(vLeft / wheelRadius * Mathf.Rad2Deg);
            setRightWheelVelocity(vRight / wheelRadius * Mathf.Rad2Deg);
        }
    }

    // Control wheels
    void setLeftWheelVelocity(float jointVelocity)
    {
        ArticulationDrive drive = leftWheel.xDrive;
        drive.target = drive.target + jointVelocity*Time.fixedDeltaTime;
        leftWheel.xDrive = drive;
    }

    void setRightWheelVelocity(float jointVelocity)
    {
        ArticulationDrive drive = rightWheel.xDrive;
        drive.target = drive.target + jointVelocity*Time.fixedDeltaTime;
        rightWheel.xDrive = drive;
    }
}
