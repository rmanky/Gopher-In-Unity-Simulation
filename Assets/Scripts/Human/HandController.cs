using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Rigidbody rBody;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotationSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        if (!rBody) {
            rBody = gameObject.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 direction = target.position - transform.position;
        float maxSpeed = Mathf.Min(speed, direction.magnitude * speed);

        // cancel out existing velocity
        rBody.AddForce(-rBody.velocity * rBody.mass, ForceMode.Impulse);
        // then add new force
        rBody.AddForce(direction.normalized * rBody.mass * maxSpeed);

        Vector3 axis = Vector3.Cross(transform.forward, target.forward);
        float maxRotation = Mathf.Min(rotationSpeed, axis.magnitude * rotationSpeed);
        rBody.AddTorque(-rBody.angularVelocity, ForceMode.VelocityChange);
        rBody.AddTorque(axis * rBody.mass * maxRotation);

        axis = Vector3.Cross(transform.up, target.up);
        rBody.AddTorque(axis * rBody.mass * maxRotation);
    }

    Quaternion FromToRot(Vector3 a, Vector3 b)
    {
        float ma = a.magnitude;
        float mb = b.magnitude;
        Vector3 mb_a = mb * a;
        Vector3 ma_b = ma * b;
        float den = 2 * ma * mb;
        float mba_mab = (mb_a + ma_b).magnitude;
        float c = mba_mab / den; // cosine of half angle
        // find the rotation axis scaled by the sine of the half angle (s)
        // using |a x b| = sin(angle) |a| |b| = 2 c s |a| |b|
        // where c and s are the cosine and sine of the half hangle
        // and mba_mab is 2 c |a| |b|
        // c is not 0 until 180 degrees (vectors are anti-parallel)
        Vector3 v = Vector3.Cross (a, b) / mba_mab;
        return new Quaternion(v.x, v.y, v.z, c);
    }
}
