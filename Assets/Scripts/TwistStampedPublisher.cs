using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;

public class TwistStampedPublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;
    // Variables required for ROS communication
    public string twistStampedTopicName = "model_twist";
    public Transform publishedTransform;
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 linearVelocity;
    private Vector3 angularVelocity;

    // Message info
    private TwistStampedMsg twistStamped;
    private string frameId = "Model Twist";

    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        previousPosition = publishedTransform.position;
        previousRotation = publishedTransform.rotation;

        // Initialize message
        twistStamped = new TwistStampedMsg
        {
            header = new HeaderMsg()
            {
                frame_id = frameId
            }
        };
    }

    private void FixedUpdate()
    {
        twistStamped.header.Update();

        linearVelocity = (publishedTransform.position - previousPosition)
                         /Time.fixedDeltaTime;
        angularVelocity = (publishedTransform.rotation.eulerAngles - previousRotation.eulerAngles)
                          /Time.fixedDeltaTime;
        previousPosition = publishedTransform.position;
        previousRotation = publishedTransform.rotation;

        twistStamped.twist.linear = linearVelocity.To<FLU>();
        twistStamped.twist.angular = angularVelocity.To<FLU>();

        ros.Send(twistStampedTopicName, twistStamped);
    }
}
