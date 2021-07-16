using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;

public class CameraPublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;

    // Variables required for ROS communication
    public Camera mainCamera;
    public Camera rightArmCamera;
    public Camera leftArmCamera;

    public string mainCameraTopicName = "main_cam/color/image_raw/compressed";
    public string rightArmCameraTopicName = "right_arm_cam/color/image_raw/compressed";
    public string leftArmCameraTopicName = "left_arm_cam/color/image_raw/compressed";

    public int FieldOfView = 60;
    public int resolutionWidth = 1280;
    public int resolutionHeight = 720;
    public int qualityLevel = 50;
    
    private string mainFrameID = "Main Camera";
    private string rightFrameID = "Right Arm Camera";
    private string leftFrameID = "Left Arm Camera";

    private CompressedImageMsg mainCameraImage;
    private CompressedImageMsg rightArmCameraImage;
    private CompressedImageMsg leftArmCameraImage;
    private Texture2D texture2D;
    private Rect rect;


    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        // Setting FOV
        mainCamera.fieldOfView = FieldOfView;
        rightArmCamera.fieldOfView = FieldOfView;
        leftArmCamera.fieldOfView = FieldOfView;

        // Game Object
        texture2D = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.ARGB32, false);
        rect = new Rect(0, 0, resolutionWidth, resolutionHeight);
        RenderTexture renderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24, 
                                                        RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        mainCamera.targetTexture = renderTexture;
        rightArmCamera.targetTexture = renderTexture;
        leftArmCamera.targetTexture = renderTexture;
        
        // Messages
        mainCameraImage = new CompressedImageMsg();
        mainCameraImage.header.frame_id = mainFrameID;
        mainCameraImage.format = "jpeg";
        rightArmCameraImage = new CompressedImageMsg();
        rightArmCameraImage.header.frame_id = rightFrameID;
        rightArmCameraImage.format = "jpeg";
        leftArmCameraImage = new CompressedImageMsg();
        leftArmCameraImage.header.frame_id = leftFrameID;
        leftArmCameraImage.format = "jpeg";

        // Call back
        Camera.onPostRender += UpdateImage;
    }

    private void UpdateImage(Camera _camera)
    {
        if (texture2D != null && _camera == mainCamera)
        {
            texture2D.ReadPixels(rect, 0, 0);
            mainCameraImage.data = texture2D.EncodeToJPG(qualityLevel);
            ros.Send(mainCameraTopicName, mainCameraImage);
        }
        /*
        else if (texture2D != null && _camera == rightArmCamera)
        {
            texture2D.ReadPixels(rect, 0, 0);
            rightArmCameraImage.data = texture2D.EncodeToJPG(qualityLevel);
            ros.Send(rightArmCameraTopicName, rightArmCameraImage);
        }

        else if (texture2D != null && _camera == leftArmCamera)
        {
            texture2D.ReadPixels(rect, 0, 0);
            leftArmCameraImage.data = texture2D.EncodeToJPG(qualityLevel);
            ros.Send(leftArmCameraTopicName, leftArmCameraImage);
        }
        */
    }
}
