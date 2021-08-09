using System;
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
    float[] positions;
    float[] velocities;
    float[] forces;

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

        jointStateLength = articulationChain.Length;
        
        positions = new float[jointStateLength];
        velocities = new float[jointStateLength];
        forces = new float[jointStateLength];
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

        for (int i = 0; i < jointStateLength; i++)
        {   
            positions[i] = articulationChain[i].jointPosition[0];
            velocities[i] = articulationChain[i].jointVelocity[0];
            forces[i] = articulationChain[i].jointForce[0];
            Debug.Log(articulationChain[i].name + " " + positions[i] + " " + velocities[i]);
        }

        jointState.position = Array.ConvertAll(positions, x => (double)x);
        jointState.velocity = Array.ConvertAll(velocities, x => (double)x);
        jointState.effort = Array.ConvertAll(forces, x => (double)x);

        //ros.Send(jointStateTopicName, jointState);
    }
}
