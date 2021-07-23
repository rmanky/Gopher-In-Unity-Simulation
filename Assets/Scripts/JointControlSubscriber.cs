using System.Collections;
using System.Linq;

using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;


public class JointControlSubscriber : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;

    // Robot object
    public GameObject gopher;
    // Hardcoded variables 
    private int numRobotJoints = 7;

    // Assures that the gripper is always positioned above the target cube before grasping.
    private readonly Quaternion pickOrientation = Quaternion.Euler(90, 90, 0);

    // Articulation Bodies
    private ArticulationBody[] jointArticulationBodies;
    private ArticulationBody leftGripper;
    private ArticulationBody rightGripper;

    private Transform gripperBase;
    private Transform leftGripperGameObject;
    private Transform rightGripperGameObject;

    private enum Poses
    {
        PreGrasp,
        Grasp,
        PickUp,
        Place
    };

    // Start is called before the first frame update
    void Start(){
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        jointArticulationBodies = new ArticulationBody[numRobotJoints];
        
        string shoulder_link = "world/base_link/chassis_link/torso/left_shoulder_link/left_arm_base_link/left_arm_shoulder_link";
        jointArticulationBodies[0] = gopher.transform.Find(shoulder_link).GetComponent<ArticulationBody>();

        string arm_link = shoulder_link + "/left_arm_half_arm_1_link";
        jointArticulationBodies[1] = gopher.transform.Find(arm_link).GetComponent<ArticulationBody>();

        string elbow_link = arm_link + "/left_arm_half_arm_2_link";
        jointArticulationBodies[2] = gopher.transform.Find(elbow_link).GetComponent<ArticulationBody>();

        string forearm_link = elbow_link + "/left_arm_forearm_link";
        jointArticulationBodies[3] = gopher.transform.Find(forearm_link).GetComponent<ArticulationBody>();

        string wrist_link = forearm_link + "/left_arm_spherical_wrist_1_link";
        jointArticulationBodies[4] = gopher.transform.Find(wrist_link).GetComponent<ArticulationBody>();

        string hand_link = wrist_link + "/left_arm_spherical_wrist_2_link";
        jointArticulationBodies[5] = gopher.transform.Find(hand_link).GetComponent<ArticulationBody>();

        string end_link = hand_link + "left_arm_bracelet_link";
        jointArticulationBodies[6] = gopher.transform.Find(end_link).GetComponent<ArticulationBody>();

        /*
        // Find left and right fingers
        string right_gripper = hand_link + "/tool_link/gripper_base/servo_head/control_rod_right/right_gripper";
        string left_gripper = hand_link + "/tool_link/gripper_base/servo_head/control_rod_left/left_gripper";
        string gripper_base = hand_link + "/tool_link/gripper_base/Collisions/unnamed";

        gripperBase = niryoOne.transform.Find(gripper_base);
        leftGripperGameObject = niryoOne.transform.Find(left_gripper);
        rightGripperGameObject = niryoOne.transform.Find(right_gripper);

        rightGripper = rightGripperGameObject.GetComponent<ArticulationBody>();
        leftGripper = leftGripperGameObject.GetComponent<ArticulationBody>();
        */

        /*
        for (int joint = 0; joint < jointArticulationBodies.Length; joint++)
        {
            var joint1XDrive = jointArticulationBodies[joint].xDrive;
            joint1XDrive.target = result[joint];
            jointArticulationBodies[joint].xDrive = joint1XDrive;
        }
        */
    }

    public void MoveRobot()
    {
        var joint1XDrive = jointArticulationBodies[0].xDrive;
        joint1XDrive.target = 45f;
        jointArticulationBodies[0].xDrive = joint1XDrive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
