using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeJoints : MonoBehaviour
{
    [SerializeField]
    private bool autoUpdate = false;

    [SerializeField]
    private float smoothingDuration = 5f;

    [SerializeField]
    private JointTargetEntry[] entries;

    [System.Serializable]
    public class JointTargetEntry
    {
        public ArticulationBody joint;
        public float target;
    }

    void SetJointTargets(float lerp) {
        foreach (JointTargetEntry entry in entries)
        {
            ArticulationBody joint = entry.joint;
            if (joint.dofCount < 1)
            {
                Debug.LogError("The degrees of freedom is zero!");
                break;
            }
            ArticulationDrive drive = joint.xDrive;
            float smoothTarget = Mathf.Lerp(0f, entry.target, lerp);
            drive.target = smoothTarget;
            joint.xDrive = drive;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InterpelateRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // if (autoUpdate) {
        //     SetJointTargets();
        // }
    }

    private IEnumerator InterpelateRoutine()
    {
        float lerp = 0;
        while(lerp < 1)
        {
            lerp = Mathf.MoveTowards(lerp, 1.0f, Time.deltaTime / smoothingDuration);
            SetJointTargets(lerp);
    
            yield return null;
        }
    }
}
