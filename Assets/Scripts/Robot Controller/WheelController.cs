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
    private List<ArticulationBody> stabilizers = new List<ArticulationBody>();

    [SerializeField]
    private float maxSpeed = 100f;

    // Start is called before the first frame update
    private void Start()
    {
        wheelDriveLeft = wheelLeft.xDrive;
        wheelDriveLeft.stiffness = 0;
        wheelDriveLeft.damping = 10;
        wheelDriveLeft.forceLimit = 10;
        wheelLeft.xDrive = wheelDriveLeft;

        wheelDriveRight = wheelRight.xDrive;
        wheelDriveRight.stiffness = 0;
        wheelDriveRight.damping = 10;
        wheelDriveRight.forceLimit = 10;
        wheelRight.xDrive = wheelDriveRight;
    }

    public void Drive(Vector2 driveVector) {
        float forwardSpeed = driveVector.y * maxSpeed;
        float rotationalSpeed = driveVector.x * maxSpeed;
        wheelLeft.xDrive = DriveWheel(wheelDriveLeft, forwardSpeed + rotationalSpeed);
        wheelRight.xDrive = DriveWheel(wheelDriveRight, forwardSpeed - rotationalSpeed);
    }

    ArticulationDrive DriveWheel(ArticulationDrive wheelDrive, float speed)
    {
        wheelDrive.targetVelocity = speed;
        return wheelDrive;
    }
}
