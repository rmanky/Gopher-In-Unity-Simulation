using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    private enum HandType {Left, Right};

    [SerializeField]
    private HandType activeHand = HandType.Left;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private ArticulationBody rBody;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotationSpeed;

    private float totalMass = 0;

    private List<ArticulationBody> fingers;

    [SerializeField]
    [Range(0f, 1f)]
    private float grip = 0.0f;

    private GopherInputActions inputActions;

    private void Awake() {
        inputActions = new GopherInputActions();
    }

    private void Hand(InputAction.CallbackContext context) {
        grip = context.ReadValue<float>();
    }

    private void SetActions(InputAction action, Action<InputAction.CallbackContext> callback) {
        action.started += callback;
        action.performed += callback;
        action.canceled += callback;
    }

    private void RemoveActions(InputAction action, Action<InputAction.CallbackContext> callback) {
        action.started -= callback;
        action.performed -= callback;
        action.canceled -= callback;
    }

    private void OnEnable() {
        inputActions.Enable();
        if (activeHand == HandType.Left) {
            SetActions(inputActions.Human.LeftHand, Hand);
        } else {
            SetActions(inputActions.Human.RightHand, Hand);
        }
    }

    private void OnDisable() {
        inputActions.Disable();
        if (activeHand == HandType.Left) {
            RemoveActions(inputActions.Human.LeftHand, Hand);
        } else {
            RemoveActions(inputActions.Human.RightHand, Hand);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!rBody) {
            rBody = gameObject.GetComponent<ArticulationBody>();
        }

        fingers = new List<ArticulationBody>();
        foreach (ArticulationBody arBody in rBody.GetComponentsInChildren<ArticulationBody>()) {
            totalMass += arBody.mass;
            fingers.Add(arBody);
        }
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 direction = target.position - rBody.worldCenterOfMass;
        float maxSpeed = Mathf.Min(speed, direction.magnitude * speed);

        // cancel out existing velocity
        rBody.velocity = Vector3.zero;
        // rBody.AddForce(-rBody.velocity, ForceMode.VelocityChange);
        // then add new force
        rBody.AddForce(direction.normalized * totalMass * maxSpeed);

        Vector3 axis = Vector3.Cross(transform.forward, target.forward);
        float maxRotation = Mathf.Min(rotationSpeed, axis.magnitude * rotationSpeed);

        // cancel out existing angular velocity
        rBody.angularVelocity = Vector3.zero;
        // rBody.AddTorque(-rBody.angularVelocity, ForceMode.VelocityChange);
        // add new angular velocity
        rBody.AddTorque(axis * totalMass * maxRotation);

        axis = Vector3.Cross(transform.up, target.up);
        rBody.AddTorque(axis * totalMass * maxRotation);

        foreach (ArticulationBody arBody in fingers) {
            ArticulationDrive xDrive = arBody.xDrive;
            xDrive.target = Mathf.Lerp(xDrive.lowerLimit, xDrive.upperLimit, grip);
            arBody.xDrive = xDrive;
        }
    }
}
