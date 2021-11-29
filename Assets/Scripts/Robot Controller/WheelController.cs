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

    // Start is called before the first frame update
    private void Start()
    {
        wheelDriveLeft = wheelLeft.xDrive;
        wheelDriveLeft.stiffness = 0;
        wheelDriveLeft.damping = 100;
        wheelDriveLeft.forceLimit = 1000;
        wheelLeft.xDrive = wheelDriveLeft;

        wheelDriveRight = wheelRight.xDrive;
        wheelDriveRight.stiffness = 0;
        wheelDriveRight.damping = 100;
        wheelDriveRight.forceLimit = 1000;
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
        wheelDrive.targetVelocity = speed / wheelRadius;
        return wheelDrive;
    }
}
