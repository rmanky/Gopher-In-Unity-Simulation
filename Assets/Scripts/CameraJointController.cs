using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJointController : MonoBehaviour
{
    public float mouseSensitivity = 200f;

    public ArticulationBody cameraYawJoint;
    public ArticulationBody cameraPitchJoint;

    private float mouseX;
    private float mouseY;

    private float yawRotation = 0f;
    private float pitchRotation = 0f;

    public float yawOffset = 0f;
    public float pitchOffset = 0f;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yawRotation = yawOffset;
        pitchRotation = pitchOffset;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    void FixedUpdate()
    {
        if (mouseX == 0 && mouseY == 0)
            return;
        
        yawRotation -= mouseX * mouseSensitivity * Time.fixedDeltaTime;
        pitchRotation += mouseY * mouseSensitivity * Time.fixedDeltaTime;
        yawRotation = Mathf.Clamp(yawRotation, yawOffset-60f, yawOffset+60f);
        pitchRotation = Mathf.Clamp(pitchRotation, pitchOffset-60f, pitchOffset+60f);;

        setJointTarget(cameraYawJoint, yawRotation);
        setJointTarget(cameraPitchJoint, pitchRotation);
    }

    // Control joints
    void setJointTarget(ArticulationBody joint, float jointPosition)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.target = jointPosition;
        joint.xDrive = drive;
    }

}
