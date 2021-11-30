using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimulationController : MonoBehaviour
{
    private enum SelectedControl { Base, Left, Right, Both };

    private SelectedControl selectedControl = SelectedControl.Base;

    private struct ActionCallback {
        public InputAction action;
        public Action<InputAction.CallbackContext> callback;

        public ActionCallback(InputAction a1, Action<InputAction.CallbackContext> c1) // Constructor.
        {
            action = a1;
            callback = c1;
        }
    }

    private GopherInputActions gopherInputActions;

    private List<ActionCallback> actionCallbacks = new List<ActionCallback>();

    [SerializeField]
    private WheelController wheelController;

    [SerializeField]
    private JacobianIK rightArmIK;

    [SerializeField]
    private JacobianIK leftArmIK;

    [SerializeField]
    private GripController rightGripController;

    [SerializeField]
    private GripController leftGripController;

    [SerializeField]
    private CameraSwitcher cameraSwitcher;

    [SerializeField]
    private XYController cameraController;

    [SerializeField]
    private bool gripperHold = true;

    private Vector3 rightArmMoveVec, rightArmRotVec, leftArmMoveVec, leftArmRotVec = Vector3.zero;
    private Vector2 cameraRotVec, wheelRotVec = Vector2.zero;

    private void Awake() {
        gopherInputActions = new GopherInputActions();
    }

    // WHEELS

    private void MoveWheels(InputAction.CallbackContext context) {
        Vector3 moveVec3D = context.ReadValue<Vector3>();
        wheelRotVec.x = moveVec3D.x;
        wheelRotVec.y = moveVec3D.z;
    }

    // CAMERA

    private void CameraRot(InputAction.CallbackContext context) {
        Vector3 rotVec3D = context.ReadValue<Vector3>();
        // Only need 2D values, and invert X
        cameraRotVec.x = -rotVec3D.x;
        cameraRotVec.y = rotVec3D.y;
    }

    // RIGHT ARM

    private void RightMove(InputAction.CallbackContext context) {
        if (selectedControl == SelectedControl.Right || selectedControl == SelectedControl.Both) {
            rightArmMoveVec = context.ReadValue<Vector3>();
        }
    }

    private void RightRot(InputAction.CallbackContext context) {
        if (selectedControl == SelectedControl.Right || selectedControl == SelectedControl.Both) {
            rightArmRotVec = context.ReadValue<Vector3>();
        }
    }

    private void RightGripper(InputAction.CallbackContext context) {
        if (selectedControl == SelectedControl.Right || selectedControl == SelectedControl.Both) {
            if (context.ReadValue<float>() != 0f || !gripperHold) {
                float val = Mathf.Max(0f, context.ReadValue<float>());
                rightGripController.SetGrippers(val);
            }
        }
    }

    // LEFT ARM

    private void LeftMove(InputAction.CallbackContext context) {
        if (selectedControl == SelectedControl.Left || selectedControl == SelectedControl.Both) {
            leftArmMoveVec = context.ReadValue<Vector3>();
        }
    }

    private void LeftRot(InputAction.CallbackContext context) {
        if (selectedControl == SelectedControl.Left || selectedControl == SelectedControl.Both) {
            leftArmRotVec = context.ReadValue<Vector3>();
        }
    }

    private void LeftGripper(InputAction.CallbackContext context) {
        if (selectedControl == SelectedControl.Left || selectedControl == SelectedControl.Both) {
            if (context.ReadValue<float>() != 0f || !gripperHold) {
                float val = Mathf.Max(0f, context.ReadValue<float>());
                leftGripController.SetGrippers(val);
            }
        }
    }

    // HELPERS

    private void SetActions(InputAction action, Action<InputAction.CallbackContext> callback) {
        ActionCallback actionCallback = new ActionCallback(action, callback);
        actionCallbacks.Add(actionCallback);
        // action.started += callback;
        action.performed += callback;
        action.canceled += callback;
    }

    private void RemoveAllActions() {
        foreach (ActionCallback actionCallback in actionCallbacks) {
            // actionCallback.action.started -= actionCallback.callback;
            actionCallback.action.performed -= actionCallback.callback;
            actionCallback.action.canceled -= actionCallback.callback;
        }
    }

    private void OnEnable() {
        gopherInputActions.Enable();
        InputAction gripperInput = gopherInputActions.Gopher.Gripper;
        SetActions(gripperInput, RightGripper);
        SetActions(gripperInput, LeftGripper);

        InputAction move3D = gopherInputActions.Gopher.Move3D;
        SetActions(move3D, RightMove);
        SetActions(move3D, LeftMove);
        SetActions(move3D, MoveWheels);

        InputAction rot3D = gopherInputActions.Gopher.Rotate3D;
        SetActions(rot3D, CameraRot);
        SetActions(rot3D, RightRot);
        SetActions(rot3D, LeftRot);

        SetActions(gopherInputActions.Gopher.CameraHead, (InputAction.CallbackContext context) => {
            cameraSwitcher.SetCamera(CameraSwitcher.CameraLocation.Head);
            Debug.Log("Switching to camera HEAD");
        });

        SetActions(gopherInputActions.Gopher.CameraLeft, (InputAction.CallbackContext context) => {
            cameraSwitcher.SetCamera(CameraSwitcher.CameraLocation.Left);
            Debug.Log("Switching to camera LEFT ARM");
        });

        SetActions(gopherInputActions.Gopher.CameraRight, (InputAction.CallbackContext context) => {
            cameraSwitcher.SetCamera(CameraSwitcher.CameraLocation.Right);
            Debug.Log("Switching to camera RIGHT ARM");
        });

        SetActions(gopherInputActions.Gopher.SelectBase, (InputAction.CallbackContext context) => {
            selectedControl = SelectedControl.Base;
            Debug.Log("Now controlling BASE");
        });

        SetActions(gopherInputActions.Gopher.SelectRightArm, (InputAction.CallbackContext context) => {
            selectedControl = SelectedControl.Right;
            Debug.Log("Now controlling RIGHT ARM");
        });

        SetActions(gopherInputActions.Gopher.SelectLeftArm, (InputAction.CallbackContext context) => {
            selectedControl = SelectedControl.Left;
            Debug.Log("Now controlling LEFT ARM");
        });

        SetActions(gopherInputActions.Gopher.SelectBoth, (InputAction.CallbackContext context) => {
            selectedControl = SelectedControl.Both;
            Debug.Log("Now controlling BOTH ARMS");
        });

    }

    private void OnDisable() {
        gopherInputActions.Disable();
        RemoveAllActions();
    }

    private void FixedUpdate() {
        // if statements are hard :(
        if (selectedControl == SelectedControl.Base) {
            if (cameraRotVec != Vector2.zero) {
                cameraController.SetTarget(cameraRotVec);
            }

            wheelController.Drive(wheelRotVec);
        } else {
            // reset to avoid weirdness
            wheelController.Drive(Vector2.zero);

            if (selectedControl == SelectedControl.Right || selectedControl == SelectedControl.Both) {
                if (rightArmMoveVec != Vector3.zero || rightArmRotVec != Vector3.zero) {
                    rightArmIK.MoveDirection(rightArmMoveVec, rightArmRotVec);
                }
            }
            
            if (selectedControl == SelectedControl.Left || selectedControl == SelectedControl.Both) {
                if (leftArmMoveVec != Vector3.zero || leftArmRotVec != Vector3.zero) {
                    leftArmIK.MoveDirection(leftArmMoveVec, leftArmRotVec);
                }
            }
        }
    }
}
