using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : MonoBehaviour
{
    [SerializeField]
    private ArticulationBody wheelLeft, wheelRight;
    private ArticulationDrive wheelDriveLeft, wheelDriveRight;

    [SerializeField]
    private float wheelRadius = 0.0625f;

    [SerializeField]
    private float targetVelocity = 2f;

    [SerializeField]
    private float forceLimit = 2500f;

    [SerializeField]
    private float damping = 100f;

    [SerializeField]
    private float stiffness = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        wheelDriveLeft = wheelLeft.xDrive;
        wheelDriveLeft.stiffness = stiffness;
        wheelDriveLeft.damping = damping;
        wheelDriveLeft.forceLimit = forceLimit;
        wheelLeft.xDrive = wheelDriveLeft;

        wheelDriveRight = wheelRight.xDrive;
        wheelDriveRight.stiffness = stiffness;
        wheelDriveRight.damping = damping;
        wheelDriveRight.forceLimit = forceLimit;
        wheelRight.xDrive = wheelDriveRight;
    }

    public void Drive(Vector2 driveVector) {
        float forwardSpeed = driveVector.y * targetVelocity;
        float rotationalSpeed = driveVector.x * targetVelocity;
        wheelLeft.xDrive = DriveWheel(wheelDriveLeft, forwardSpeed + rotationalSpeed);
        wheelRight.xDrive = DriveWheel(wheelDriveRight, forwardSpeed - rotationalSpeed);
    }

    ArticulationDrive DriveWheel(ArticulationDrive wheelDrive, float speed)
    {
        wheelDrive.targetVelocity = (360f * speed) / (2f * Mathf.PI * wheelRadius);
        return wheelDrive;
    }
}
