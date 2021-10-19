using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
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
        SetTarget(leftFinger, Input.GetAxis("Fire1") * 45);
        SetTarget(leftFingerTip, Input.GetAxis("Fire1") * -20);
        SetTarget(rightFinger, Input.GetAxis("Fire1") * 45);
        SetTarget(rightFingerTip, Input.GetAxis("Fire1") * -20);
    }

    void SetTarget(ArticulationBody joint, float target)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.target = target;
        drive.forceLimit = 100;
        drive.damping = 100;
        joint.xDrive = drive;
    }
}
