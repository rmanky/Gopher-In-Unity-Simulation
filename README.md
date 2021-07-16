# Gopher In Unity Simulation
This package simulates Gopher nursing robot with [Unity](https://unity.com/), which could provide better graphic performance and interface design. Thanks to [Unity Robotics Hub](https://github.com/Unity-Technologies/Unity-Robotics-Hub), ROS-Unity connection is also provided in this repository for utilizing packages based on ROS.

![image](demo/Gopher_in_Unity.png)

## Dependencies
This repository has been developed and tested in Ubuntu 18.04 and ROS Melodic, with Unity 20.3.

Before running this package, it is recommended to try the [Unity Robotics Hub](https://github.com/Unity-Technologies/Unity-Robotics-Hub) demo first. They provided a very detailed instruction to set up robots in Unity. But you could still run this package by following the steps below.

---

Create a new Unity 3D project and name it **Gopher In Unity Simulation**. 

Change the physics and color settings. Open `Edit` -> `Project Settings` 

- In `Physics`, change `Solver Type` from `Projected Gauss Seidel` to `Temporal Gauss Seidel`. 
- In `Player`, change `Color Space` from `Gamma` to `Linear`.

The next step is to install ROS-Unity connection package. Open `Window` -> `Package Manager`, find and click the `+` in the left upper corner and switch to `Add package from git URL...`. 

- To install [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector), enter `https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector` and add it.
- To install [URDF-Importer](https://github.com/Unity-Technologies/URDF-Importer), enter `https://github.com/Unity-Technologies/URDF-Importer.git?path=/com.unity.robotics.urdf-importer` and add it.

You could `git clone` and download this repository now, then cut and paste to the folder where it stores your Unity projects, to merge with your **Gopher In Unity Simulation** folder. Switch back to Unity, and now it should load all the necessary files.

## Running


