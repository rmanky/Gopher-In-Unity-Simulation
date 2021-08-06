using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;

public class JointStatePublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;
    // Variables required for ROS communication
    public string jointStateTopicName = "joint_states";

    // Joints
    public GameObject jointRoot;
    private ArticulationBody[] articulationChain;
    private int jointStateLength;
    string[] names;
    List<float> positions;
    List<float> velocities;
    List<float> forces;

    // Message info
    private JointStateMsg jointState; 
    private string frameId = "Joint State";

    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        // Get joints
        articulationChain = jointRoot.GetComponentsInChildren<ArticulationBody>();
        articulationChain = articulationChain.Where(joint => joint.jointType 
                                                    != ArticulationJointType.FixedJoint).ToArray();

        int jointStateLength = articulationChain.Length;

        positions = new List<float>();
        velocities = new List<float>();
        forces = new List<float>();
        names = new string[jointStateLength];
        for (int i = 0; i < jointStateLength; i++)
            names[i] = articulationChain[i].name;

        // Initialize message
        jointState = new JointStateMsg
        {
            header = new HeaderMsg { frame_id = frameId },
            name = new string[jointStateLength],
            position = new double[jointStateLength],
            velocity = new double[jointStateLength],
            effort = new double[jointStateLength]
        };
    }

    private void FixedUpdate()
    {
        jointState.header.Update();

        articulationChain[0].GetJointPositions(positions);
        articulationChain[0].GetJointVelocities(velocities);
        articulationChain[0].GetJointForces(forces);

        jointState.position = positions.Select(x => (double)x).ToArray();
        jointState.velocity = velocities.Select(x => (double)x).ToArray();
        jointState.effort = forces.Select(x => (double)x).ToArray();

        ros.Send(jointStateTopicName, jointState);
    }
}
