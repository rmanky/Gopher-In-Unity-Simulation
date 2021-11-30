using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private ArticulationBody rBody;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotationSpeed;

    private float totalMass = 0;

    [SerializeField]
    private ArticulationBody[] fingers;

    [SerializeField]
    [Range(0f, 1f)]
    private float grip = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
        if (!rBody) {
            rBody = gameObject.GetComponent<ArticulationBody>();
        }

        foreach (ArticulationBody arBody in rBody.GetComponentsInChildren<ArticulationBody>()) {
            totalMass += arBody.mass;
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
            xDrive.target = grip * xDrive.upperLimit;
            arBody.xDrive = xDrive;
        }
    }
}
