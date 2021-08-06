using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This script initializes the articulation bodies by 
///     setting stiffness, damping and force limit of
///     the non-fixed ones.
/// </summary>
public class ArticulationBodyInitialization : MonoBehaviour
{
    private ArticulationBody[] articulationChain;

    public GameObject robotRoot;
    public bool assignToAllChildren = true;
    public int robotChainLength = 0;
    public float stiffness = 10000f;
    public float damping = 100f;
    public float forceLimit = 1000f;

    void Start()
    {
        // get non-fixed joints
        articulationChain = robotRoot.GetComponentsInChildren<ArticulationBody>();
        articulationChain = articulationChain.Where(joint => joint.jointType 
                                                    != ArticulationJointType.FixedJoint).ToArray();
        foreach (ArticulationBody joint in articulationChain)
        {
            Debug.Log(joint.name);
        }

        // joint length to assign
        int assignLength = articulationChain.Length;
        if (!assignToAllChildren)
            assignLength = robotChainLength;

        // setting stiffness, damping and force limit
        int defDyanmicVal = 100;
        for (int i = 0; i < assignLength; i++)
        {
            ArticulationBody joint = articulationChain[i];

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