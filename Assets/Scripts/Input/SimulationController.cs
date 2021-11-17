using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimulationController : MonoBehaviour
{
    private GopherInputActions gopherInputActions;
    private InputAction movement, rightArmMoveInput, rightArmRotInput;

    [SerializeField]
    private WheelController wheelController;

    [SerializeField]
    private JacobianIK rightArmIK;

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

    private void StartRightArmMove(InputAction.CallbackContext context) {
        rightArmMoving = true;
    }

    private void PerformRightArmMove(InputAction.CallbackContext context) {
        rightArmPos = context.ReadValue<Vector3>();
    }

    private void StopRightArmMove(InputAction.CallbackContext context) {
        rightArmMoving = false;
    }

    private void StartRightArmRot(InputAction.CallbackContext context) {
        rightArmMoving = true;
    }

    private void PerformRightArmRot(InputAction.CallbackContext context) {
        rightArmRot = context.ReadValue<Vector3>();
    }

    private void StopRightArmRot(InputAction.CallbackContext context) {
        rightArmMoving = false;
    }

    private void OnEnable() {
        gopherInputActions.Enable();
        movement = gopherInputActions.Gopher.Movement;
        movement.performed += Moved;
        movement.canceled += Stopped;

        rightArmMoveInput = gopherInputActions.Gopher.RightArmMove;
        rightArmMoveInput.started += StartRightArmMove;
        rightArmMoveInput.performed += PerformRightArmMove;
        rightArmMoveInput.canceled += StopRightArmMove;

        rightArmRotInput = gopherInputActions.Gopher.RightArmRot;
        rightArmRotInput.started += StartRightArmRot;
        rightArmRotInput.performed += PerformRightArmRot;
        rightArmRotInput.canceled += StopRightArmRot;
    }

    private void OnDisable() {
        gopherInputActions.Disable();
        movement.performed -= Moved;
        movement.canceled -= Stopped;

        rightArmMoveInput.started -= StartRightArmMove;
        rightArmMoveInput.performed -= PerformRightArmMove;
        rightArmMoveInput.canceled -= StopRightArmMove;
    }

    private void FixedUpdate() {
        if(rightArmMoving || rightArmRotating) {
            rightArmIK.MoveDirection(rightArmPos, rightArmRot);
        }
    }
}
