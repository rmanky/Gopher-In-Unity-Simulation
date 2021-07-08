using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

public class ModelStatePublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;

    // Variables required for ROS communication
    public string poseTopicName = "model_pose";
    public string velocityTopicName = "model_vel";

    public GameObject model;
    private MPose mModelPose;


    /// <summary>
    /// Publish model state - pose and velocity
    /// </summary>
    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;
        
        mModelPose = new MPose();
    }

    void Update()
    {
        mModelPose.position = model.transform.position.To<FLU>();
        mModelPose.orientation = model.transform.rotation.To<FLU>();

        //Geometry.MTwist mModelQuaternion = new Geometry.MTwist
        //{
        //    linear = mModelTwistLinear
        //    angular = mModelTwistAngular
        //};

        ros.Send(poseTopicName, mModelPose);
        //ros.Send(velTopicName, mModelVelocity);
    }
}
