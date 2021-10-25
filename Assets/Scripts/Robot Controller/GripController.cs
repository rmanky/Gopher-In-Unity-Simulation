using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripController : MonoBehaviour
{
    [SerializeField]
    private ArticulationBody leftFinger;

    [SerializeField]
    private ArticulationBody leftFingerTip;

    [SerializeField]
    private ArticulationBody rightFinger;

    [SerializeField]
    private ArticulationBody rightFingerTip;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetTarget(leftFinger, Input.GetAxis("Fire1") * 50);
        SetTarget(leftFingerTip, Input.GetAxis("Fire1") * -40);
        SetTarget(rightFinger, Input.GetAxis("Fire1") * 50);
        SetTarget(rightFingerTip, Input.GetAxis("Fire1") * -40);
    }

    void SetTarget(ArticulationBody joint, float target)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.target = target;
        drive.stiffness = 5000;
        drive.forceLimit = 200;
        drive.damping = 500;
        joint.xDrive = drive;
    }
}
