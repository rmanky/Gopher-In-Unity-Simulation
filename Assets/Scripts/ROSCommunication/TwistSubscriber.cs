using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

public class TwistSubscriber : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;

    // Variables required for ROS communication
    public string twistTopicName = "base_controller/cmd_vel";

    // Transform
    public GameObject controlledObject;
    public float linearSpeed = 1.5f;
    public float angularSpeed = 1.0f;
    private Rigidbody rb;

    private Vector3 forwardDirection;
    private Vector3 linearVelocity;
    private Vector3 angularVelocity;
    private bool isMessageReceived;


    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        // Transform of the controlled object
        rb = controlledObject.GetComponent<Rigidbody>();

        ros.Subscribe<TwistMsg>(twistTopicName, updateVelocity);
        isMessageReceived = false;
    }

    private void FixedUpdate()
    {
        if (isMessageReceived)
        {
            // Linear velocity
            forwardDirection = transform.forward * linearVelocity.z;
            rb.velocity = linearSpeed * forwardDirection.normalized;
            // Angular velocity
            rb.angularVelocity = - angularVelocity.y * angularSpeed * Vector3.up;

            isMessageReceived = false;
        }
    }

    private void updateVelocity(TwistMsg twist)
    {
        linearVelocity = twist.linear.From<FLU>();
        angularVelocity = twist.angular.From<FLU>();

        isMessageReceived = true;
    }
}
