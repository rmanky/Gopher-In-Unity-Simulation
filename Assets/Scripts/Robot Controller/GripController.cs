using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripController : MonoBehaviour
{
    [System.Serializable]
    private class FingerJoint {
        public ArticulationBody body;
        public float target;
    }

    [SerializeField]
    private FingerJoint[] fingerJoints;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetGrippers(float closeValue) {
        foreach (FingerJoint fj in fingerJoints) {
            SetTarget(fj.body, closeValue * fj.target);
        }
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
