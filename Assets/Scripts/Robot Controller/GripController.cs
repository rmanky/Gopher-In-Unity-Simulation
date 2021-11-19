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

    public void SetGrippers(float closeValue) {
        SetTarget(leftFinger, closeValue * 50);
        SetTarget(leftFingerTip, closeValue * -30);
        SetTarget(rightFinger, closeValue * 50);
        SetTarget(rightFingerTip, closeValue * -30);
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
