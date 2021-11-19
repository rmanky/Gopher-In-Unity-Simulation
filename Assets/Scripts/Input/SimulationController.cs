using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimulationController : MonoBehaviour
{
    private GopherInputActions gopherInputActions;
    private InputAction movement, rightArmGripper, rightArmMoveInput, rightArmRotInput;

    [SerializeField]
    private WheelController wheelController;

    [SerializeField]
    private JacobianIK rightArmIK;

    [SerializeField]
    private GripController gripController;

    [SerializeField]
    private bool gripperHold = true;

    private bool rightArmMoving, rightArmRotating;
    private Vector3 rightArmPos, rightArmRot = new Vector3(0f, 0f, 0f);

    private void Awake() {
        gopherInputActions = new GopherInputActions();
    }
    private void Moved(InputAction.CallbackContext context) {
        wheelController.Drive(context.ReadValue<Vector2>());
    }

    private void Stopped(InputAction.CallbackContext context) {
        wheelController.Drive(context.ReadValue<Vector2>());
    }

    /// MOVING

    private void StartRightArmMove(InputAction.CallbackContext context) {
        rightArmPos = Vector3.zero;
        rightArmMoving = true;
    }

    private void PerformRightArmMove(InputAction.CallbackContext context) {
        Vector3 val = context.ReadValue<Vector3>();
        if (val.magnitude < 0.1f) {
            rightArmPos = Vector3.zero;
            rightArmMoving = false;
        } else {
            rightArmPos = context.ReadValue<Vector3>();
            rightArmMoving = true;
        }
    }

    /// ROTATING

    private void StartRightArmRot(InputAction.CallbackContext context) {
        rightArmRot = Vector3.zero;
        rightArmRotating = true;
    }

    private void PerformRightArmRot(InputAction.CallbackContext context) {
        Vector3 val = context.ReadValue<Vector3>();
        if (val.magnitude < 0.1f) {
            rightArmRot = Vector3.zero;
            rightArmRotating = false;
        } else {
            rightArmRot = context.ReadValue<Vector3>();
            rightArmRotating = true;
        }
    }

    private void RightGripper(InputAction.CallbackContext context) {
        float val = Mathf.Max(0f, context.ReadValue<float>());
        gripController.SetGrippers(val);
    }

    private void OnEnable() {
        gopherInputActions.Enable();
        movement = gopherInputActions.Gopher.Movement;
        movement.performed += Moved;
        movement.canceled += Stopped;

        rightArmGripper = gopherInputActions.Gopher.RightGripper;
        rightArmGripper.performed += RightGripper;
        if (!gripperHold) {
            rightArmGripper.canceled += RightGripper;
        }

        rightArmMoveInput = gopherInputActions.Gopher.RightArmMove;
        rightArmMoveInput.started += StartRightArmMove;
        rightArmMoveInput.performed += PerformRightArmMove;

        rightArmRotInput = gopherInputActions.Gopher.RightArmRot;
        rightArmRotInput.started += StartRightArmRot;
        rightArmRotInput.performed += PerformRightArmRot;
    }

    private void OnDisable() {
        gopherInputActions.Disable();
        movement.performed -= Moved;
        movement.canceled -= Stopped;

        rightArmGripper.performed -= RightGripper;
        if (!gripperHold) {
            rightArmGripper.canceled -= RightGripper;
        }

        rightArmMoveInput.started -= StartRightArmMove;
        rightArmMoveInput.performed -= PerformRightArmMove;

        rightArmRotInput.started -= StartRightArmRot;
        rightArmRotInput.performed -= PerformRightArmRot;
    }

    private void FixedUpdate() {
        if(rightArmMoving || rightArmRotating) {
            Debug.Log(rightArmMoving + " " + rightArmRotating);
            rightArmIK.MoveDirection(rightArmPos, rightArmRot);
        }
    }
}
