using System;
using System.Collections.Generic;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.Tf2;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.SlamExample;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

/// <summary>
///     This script publishes all the 
///     non-fixed joint states - position, 
///     velocity and effort
/// </summary>
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
    float[] positions;
    float[] velocities;
    float[] forces;

    // Message
    private JointStateMsg jointState; 
    private string frameId = "";
    public float publishRate;

    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(jointStateTopicName);

        // Get joints
        articulationChain = jointRoot.GetComponentsInChildren<ArticulationBody>();
        articulationChain = articulationChain.Where(joint => joint.jointType 
                                                    != ArticulationJointType.FixedJoint).ToArray();

        jointStateLength = articulationChain.Length;
        
        positions = new float[jointStateLength];
        velocities = new float[jointStateLength];
        forces = new float[jointStateLength];
        names = new string[jointStateLength];

        // Initialize message
        for (int i = 0; i < jointStateLength; ++i)
            if (articulationChain[i].GetComponent<UrdfJoint>()) {
                names[i] = articulationChain[i].GetComponent<UrdfJoint>().jointName;
            } else {
                names[i] = "blah";
            }
        
        jointState = new JointStateMsg
        {
            header = new HeaderMsg(Clock.GetCount(), new TimeStamp(Clock.time), frameId),
            name = names,
            position = new double[jointStateLength],
            velocity = new double[jointStateLength],
            effort = new double[jointStateLength]
        };

        InvokeRepeating("PublishJointStates", 1f, 1f/publishRate);
    }

    void Update()
    {
    }

    private void PublishJointStates()
    {
        jointState.header.Update();

        for (int i = 0; i < jointStateLength; ++i)
        {   
            positions[i] = articulationChain[i].jointPosition[0];
            velocities[i] = articulationChain[i].jointVelocity[0];
            forces[i] = articulationChain[i].jointForce[0];
        }

        jointState.position = Array.ConvertAll(positions, x => (double)x);
        jointState.velocity = Array.ConvertAll(velocities, x => (double)x);
        jointState.effort = Array.ConvertAll(forces, x => (double)x);

        ros.Publish(jointStateTopicName, jointState);
    }
}
