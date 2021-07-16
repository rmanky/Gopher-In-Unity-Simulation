using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;

using RosSharp.RosBridgeClient;

public class LaserScanPublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;

    // Variables required for ROS communication
    public LaserScanReader laserScanReader;
    public string laserTopicName = "base_scan";
    private string FrameId = "Laser Scan";
    private float scanPeriod;
    private float previousScanTime = 0;
    private MLaserScan laserScan;


    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        // Messages
        scanPeriod = (float)laserScanReader.samples / (float)laserScanReader.update_rate;

        laserScan = new MLaserScan
        {
            header = new MHeader { frame_id = FrameId },
            angle_min       = laserScanReader.angle_min,
            angle_max       = laserScanReader.angle_max,
            angle_increment = laserScanReader.angle_increment,
            time_increment  = laserScanReader.time_increment,
            range_min       = laserScanReader.range_min,
            range_max       = laserScanReader.range_max,
            ranges          = laserScanReader.ranges,      
            intensities     = laserScanReader.intensities
        };
    }

    private void FixedUpdate()
    {
        if (Time.realtimeSinceStartup >= previousScanTime + scanPeriod)
        {
            UpdateLaserScan();
            previousScanTime = Time.realtimeSinceStartup;
        }
    }


    private void UpdateLaserScan()
    {
        laserScan.ranges = laserScanReader.Scan();
        ros.Send(laserTopicName, laserScan);
    }
}
