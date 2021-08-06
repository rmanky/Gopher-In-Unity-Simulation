using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;

public class PoseStampedPublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;
    // Variables required for ROS communication
    public string poseStampedTopicName = "model_pose";
    public Transform publishedTransform;

    // Message info
    private PoseStampedMsg poseStamped;
    private string frameId = "Model Pose";

    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        // Initialize message
        poseStamped = new PoseStampedMsg
        {
            header = new HeaderMsg()
            {
                frame_id = frameId
            }
        };
    }

    private void FixedUpdate()
    {
        poseStamped.header.Update();

        poseStamped.pose.position = publishedTransform.position.To<FLU>();
        poseStamped.pose.orientation = publishedTransform.rotation.To<FLU>();

        ros.Send(poseStampedTopicName, poseStamped);
    }
}
