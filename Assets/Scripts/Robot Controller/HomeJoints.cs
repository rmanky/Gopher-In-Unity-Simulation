using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeJoints : MonoBehaviour
{
    [SerializeField]
    private bool autoUpdate = false;

    [SerializeField]
    private JointTargetEntry[] entries;

    [System.Serializable]
    public class JointTargetEntry
    {
        public ArticulationBody joint;
        public float target;
    }

    void SetJointTargets() {
        foreach (JointTargetEntry entry in entries)
        {
            ArticulationBody joint = entry.joint;
            float target = entry.target;
            if (joint.dofCount < 1)
            {
                Debug.LogError("The degrees of freedom is zero!");
                break;
            }
            ArticulationDrive drive = joint.xDrive;
            drive.target = target;
            joint.xDrive = drive;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SetJointTargets();
    }

    // Update is called once per frame
    void Update()
    {
        if (autoUpdate) {
            SetJointTargets();
        }
    }
}
