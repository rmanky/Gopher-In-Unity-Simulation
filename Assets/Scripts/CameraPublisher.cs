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
    public string mainCameraTopicName = "main_cam/color/image_raw";
    public string mainWideCameraTopicName = "main_cam_wide/color/image_raw";
    public string rightCameraTopicName = "right_arm_cam/color/image_raw";
    public string rightWideCameraTopicName = "right_arm_cam_wide/color/image_raw";
    public string leftCameraTopicName = "left_arm_cam/color/image_raw";
    public string leftWideCameraTopicName = "left_arm_cam_wide/color/image_raw";

    public Camera mainCamera;
    private string mainFrameID = "MainCamera";
    public int mainCameraFieldOfView = 60;
    public Camera rightCamera;
    private string rightFrameID = "RightCamera";
    public int rightCameraFieldOfView = 60;
    public Camera leftCamera;
    private string leftFrameID = "LeftCamera";
    public int leftCameraFieldOfView = 60;

    public int resolutionWidth = 1280;
    public int resolutionHeight = 720;
    public int qualityLevel = 50;
    
    private MCompressedImage mainCameraImage;
    private MCompressedImage rightCameraImage;
    private MCompressedImage leftCameraImage;
    private Texture2D texture2D;
    private Rect rect;


    /// <summary>
    /// Publish model state - pose and velocity
    /// </summary>
    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        // Setting FOV
        mainCamera.fieldOfView = mainCameraFieldOfView;
        rightCamera.fieldOfView = rightCameraFieldOfView;
        leftCamera.fieldOfView = leftCameraFieldOfView;

        // Game Object
        texture2D = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        rect = new Rect(0, 0, resolutionWidth, resolutionHeight);
        mainCamera.targetTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        rightCamera.targetTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        leftCamera.targetTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        
        // Messages
        mainCameraImage = new MCompressedImage();
        mainCameraImage.header.frame_id = mainFrameID;
        mainCameraImage.format = "jpeg";
        rightCameraImage = new MCompressedImage();
        rightCameraImage.header.frame_id = rightFrameID;
        rightCameraImage.format = "jpeg";
        leftCameraImage = new MCompressedImage();
        leftCameraImage.header.frame_id = leftFrameID;
        leftCameraImage.format = "jpeg";

        // Call back
        Camera.onPostRender += UpdateImage;
    }

    private void UpdateImage(Camera _camera)
    {
        if (texture2D != null && _camera == this.mainCamera)
        {
            texture2D.ReadPixels(rect, 0, 0);
            mainCameraImage.data = texture2D.EncodeToJPG(qualityLevel);
            ros.Send(mainCameraTopicName, mainCameraImage);
        }

        else if (texture2D != null && _camera == this.rightCamera)
        {
            texture2D.ReadPixels(rect, 0, 0);
            rightCameraImage.data = texture2D.EncodeToJPG(qualityLevel);
            ros.Send(mainCameraTopicName, rightCameraImage);
        }

        else if (texture2D != null && _camera == this.leftCamera)
        {
            texture2D.ReadPixels(rect, 0, 0);
            leftCameraImage.data = texture2D.EncodeToJPG(qualityLevel);
            ros.Send(mainCameraTopicName, leftCameraImage);
        }
    }
}
