using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XYController : MonoBehaviour
{
    [SerializeField]
    private ArticulationBody verticalJoint;

    [SerializeField]
    private ArticulationBody horizontalJoint;

    [SerializeField]
    private float sensitivity = 1f;

    [SerializeField]
    private bool autoSetOffsets = true;

    [SerializeField]
    private float verticalOffset;

    [SerializeField]
    private float horizontalOffset;

    private float _verticalTarget, _horizontalTarget;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (autoSetOffsets) {
            verticalOffset = verticalJoint.xDrive.target;
            horizontalOffset = horizontalJoint.xDrive.target;    
        }

        _verticalTarget = verticalOffset;
        _horizontalTarget = horizontalOffset;
    }

    // Update is called once per frame
    void Update()
    {
        float verticalLook = Input.GetAxis("Mouse Y") * sensitivity;
        ArticulationDrive verticalDrive = verticalJoint.xDrive;
        _verticalTarget -= verticalLook;
        verticalDrive.target = _verticalTarget;
        verticalDrive.damping = 1000;
        verticalDrive.forceLimit = 0.5f;
        verticalJoint.xDrive = verticalDrive;


        float horizontalLook = Input.GetAxis("Mouse X") * sensitivity;
        ArticulationDrive horizontalDrive = horizontalJoint.xDrive;
        _horizontalTarget += horizontalLook;
        horizontalDrive.target = _horizontalTarget;
        horizontalDrive.damping = 1000;
        horizontalDrive.forceLimit = 0.5f;
        horizontalJoint.xDrive = horizontalDrive;
    }
}
