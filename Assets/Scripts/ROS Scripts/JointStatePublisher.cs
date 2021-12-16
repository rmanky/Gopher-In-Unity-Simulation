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
    public string frameId = "gopher/base_link";
    private UrdfJoint[] jointChain;
    private int jointStateLength;
    string[] names;
    float[] positions;
    float[] velocities;
    float[] forces;

    // Message
    private JointStateMsg jointState; 
    public float publishRate;

    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(jointStateTopicName);

        // Get joints
        jointChain = jointRoot.GetComponentsInChildren<UrdfJoint>();

        // This is broken, and had to be manually patch in UrdfJoint.cs
        jointChain = jointChain.Where(joint => joint.IsRevoluteOrContinuous).ToArray();

        jointStateLength = jointChain.Length;
        
        positions = new float[jointStateLength];
        velocities = new float[jointStateLength];
        forces = new float[jointStateLength];
        names = new string[jointStateLength];

        // Initialize message
        for (int i = 0; i < jointStateLength; ++i)
        {
            names[i] = jointChain[i].jointName;
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
            positions[i] = jointChain[i].GetPosition();
            velocities[i] = jointChain[i].GetVelocity();
            forces[i] = jointChain[i].GetEffort();
        }

        jointState.header = new HeaderMsg(Clock.GetCount(), new TimeStamp(Clock.time), frameId);
        jointState.position = Array.ConvertAll(positions, x => (double)x);
        jointState.velocity = Array.ConvertAll(velocities, x => (double)x);
        jointState.effort = Array.ConvertAll(forces, x => (double)x);

        ros.Publish(jointStateTopicName, jointState);
    }
}
