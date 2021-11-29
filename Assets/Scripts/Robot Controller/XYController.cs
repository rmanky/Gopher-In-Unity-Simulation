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
    private bool inheritLimits = true;

    [SerializeField]
    private float verticalOffset;

    [SerializeField]
    private float horizontalOffset;

    private float _verticalTarget, _horizontalTarget;

    // Start is called before the first frame update
    private void Start()
    {
        if (autoSetOffsets) {
            verticalOffset = verticalJoint.xDrive.target;
            horizontalOffset = horizontalJoint.xDrive.target;    
        }

        _verticalTarget = verticalOffset;
        _horizontalTarget = horizontalOffset;
    }

    public void SetTarget(Vector2 lookDir)
    {
        float verticalLook = lookDir.y;
        ArticulationDrive verticalDrive = verticalJoint.xDrive;
        _verticalTarget += verticalLook;
        if (inheritLimits) {
            _verticalTarget = Mathf.Clamp(_verticalTarget, verticalDrive.lowerLimit, verticalDrive.upperLimit);
        }
        verticalDrive.target = _verticalTarget;
        verticalJoint.xDrive = verticalDrive;


        float horizontalLook = lookDir.x;
        ArticulationDrive horizontalDrive = horizontalJoint.xDrive;
        _horizontalTarget += horizontalLook;
        if (inheritLimits) {
            _horizontalTarget = Mathf.Clamp(_horizontalTarget, horizontalDrive.lowerLimit, horizontalDrive.upperLimit);
        }
        horizontalDrive.target = _horizontalTarget;
        horizontalJoint.xDrive = horizontalDrive;
    }
}
