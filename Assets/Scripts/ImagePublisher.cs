using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class ImagePublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;
    // Variables required for ROS communication
    public string cameraTopicName = "camera/color/image_raw/compressed";

    // Sensor
    public Camera imageCamera;
    public int FieldOfView = 60;
    public int resolutionWidth = 1280;
    public int resolutionHeight = 720;
    public int qualityLevel = 50;
    
    // Message info
    private CompressedImageMsg compressedImage;
    private string frameID = "Camera";

    private Texture2D texture2D;
    private Rect rect;

    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        // Setting FOV
        imageCamera.fieldOfView = FieldOfView;

        // Render image
        texture2D = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.ARGB32, false);
        rect = new Rect(0, 0, resolutionWidth, resolutionHeight);
        RenderTexture renderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24, 
                                                        RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        imageCamera.targetTexture = renderTexture;
        
        // Messages
        compressedImage = new CompressedImageMsg();
        compressedImage.header.frame_id = frameID;
        compressedImage.format = "jpeg";

        // Call back
        Camera.onPostRender += UpdateImage;
    }

    private void UpdateImage(Camera _camera)
    {
        if (texture2D != null && _camera == imageCamera)
        {
            compressedImage.header.Update();
            texture2D.ReadPixels(rect, 0, 0);
            compressedImage.data = texture2D.EncodeToJPG(qualityLevel);
            ros.Send(cameraTopicName, compressedImage);
        }
    }
}
