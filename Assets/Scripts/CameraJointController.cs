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
    private float yawOffsetDeg;
    private float pitchOffsetDeg;
    private float angleLimit = 60f;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yawOffsetDeg = yawOffset * Mathf.Rad2Deg;
        pitchOffsetDeg = pitchOffset * Mathf.Rad2Deg;
        yawRotation = yawOffsetDeg;
        pitchRotation = pitchOffsetDeg;
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
        yawRotation = Mathf.Clamp(yawRotation, 
                                  yawOffsetDeg - angleLimit, yawOffsetDeg + angleLimit);
        pitchRotation = Mathf.Clamp(pitchRotation, 
                                    pitchOffsetDeg - angleLimit, pitchOffsetDeg + angleLimit);

        setJointTarget(cameraYawJoint, yawRotation);
        setJointTarget(cameraPitchJoint, pitchRotation);
    }

    // Control joints
    void setJointTarget(ArticulationBody joint, float target)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.target = target;
        joint.xDrive = drive;
    }
}
