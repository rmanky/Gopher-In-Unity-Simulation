using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticulationBodyInitialization : MonoBehaviour
{
    private ArticulationBody[] articulationChain;

    public GameObject robot;
    public float stiffness = 10000f;
    public float damping = 100f;
    public float forceLimit = 1000f;

    void Start()
    {
        articulationChain = robot.GetComponentsInChildren<ArticulationBody>();
        articulationChain = articulationChain.Where(joint => joint.jointType 
                                                    != ArticulationJointType.FixedJoint).ToArray();

        int defDyanmicVal = 100;
        foreach (ArticulationBody joint in articulationChain)
        {
            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;

            ArticulationDrive currentDrive = joint.xDrive;
            currentDrive.stiffness = stiffness;
            currentDrive.damping = damping;
            currentDrive.forceLimit = forceLimit;
            joint.xDrive = currentDrive;
        }
    }

    void Update()
    {
    }
}
