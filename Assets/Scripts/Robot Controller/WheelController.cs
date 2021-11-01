using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField]
    private ArticulationBody wheelLeft;

    [SerializeField]
    private ArticulationBody wheelRight;

    [SerializeField]
    private List<ArticulationBody> stabilizers = new List<ArticulationBody>();

    [SerializeField]
    private float maxSpeed = 100f;

    private ArticulationDrive wheelDriveLeft;

    private ArticulationDrive wheelDriveRight;


    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        float forwardSpeed = Input.GetAxis("Vertical") * maxSpeed;
        float rotationalSpeed = Input.GetAxis("Horizontal") * maxSpeed;
        wheelLeft.xDrive = DriveWheel(wheelDriveLeft, forwardSpeed + rotationalSpeed);
        wheelRight.xDrive = DriveWheel(wheelDriveRight, forwardSpeed - rotationalSpeed);
    }

    ArticulationDrive DriveWheel(ArticulationDrive wheelDrive, float speed)
    {
        wheelDrive.targetVelocity = speed;
        return wheelDrive;
    }
}
