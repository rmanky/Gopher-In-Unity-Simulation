using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointPositionInitialization : MonoBehaviour
{
    // Robot object
    public GameObject jointRoot;
    public float speed = 10f;
    // Articulation Bodies
    public float[] homePosition = {0f, 0f, 0f, 0f, 0f, 0f, 0f};
    private ArticulationBody[] articulationChain;

    // home flag
    private bool isHomed = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get joints
        articulationChain = jointRoot.GetComponentsInChildren<ArticulationBody>();
        articulationChain = articulationChain.Where(joint => joint.jointType 
                                                    != ArticulationJointType.FixedJoint).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        // Initialize robot position
        if (!isHomed)
            HomeRobot();
    }

    public void HomeRobot()
    {
        int count = 0;
        for (int i = 0; i < homePosition.Length; i++)
        {
            float targetPosition = homePosition[i] * Mathf.Rad2Deg;
            if (articulationChain[i].xDrive.target != targetPosition)
                moveJoint(i, targetPosition);
            else
                count += 1;
        }

        if (count == 7)
        {
            isHomed = true;
            Debug.Log("All joints homed.");
        }
    }

    public void moveJoint(int jointNum, float target)
    {
        ArticulationDrive drive = articulationChain[jointNum].xDrive;

        float currentTarget = drive.target;
        float deltaPosition = speed * Mathf.Rad2Deg * Time.fixedDeltaTime;
        if (Mathf.Abs(currentTarget - target) > deltaPosition)
        {
            if (target > currentTarget)
                drive.target = currentTarget + deltaPosition;
            else
                drive.target = currentTarget - deltaPosition;
        }
        else
        {
            drive.target = target;
        }

        articulationChain[jointNum].xDrive = drive;
    }
}
