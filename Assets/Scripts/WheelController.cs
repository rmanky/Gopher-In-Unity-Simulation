using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public float speed = 100f;
    public float angularSpeed = 50f;
    public WheelCollider[] leftWheel;
    public WheelCollider[] rightWheel;

    private float xMove;
    private float zMove;
    private float targetLinearSpeed;
    private float targetAngularSpeed;

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
        float motor = targetLinearSpeed;
        float angularMotor = targetAngularSpeed;
        foreach (WheelCollider lw in leftWheel) 
            lw.motorTorque = motor + angularMotor;
        foreach (WheelCollider rw in rightWheel)
            rw.motorTorque = motor - angularMotor;
    }
}