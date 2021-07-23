using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticulationController : MonoBehaviour
{
    public float speed = 1.0f;
    public float angularSpeed = 1.5f;

    private ArticulationBody ab;
    private float xMove;
    private float zMove;
    private Vector3 forwardDirection;

    // Start is called before the first frame update
    void Start()
    {
        ab = GetComponent<ArticulationBody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get key input
        xMove = Input.GetAxisRaw("Horizontal");

        zMove = Input.GetAxisRaw("Vertical");
        forwardDirection = transform.forward * zMove;
    }

    void FixedUpdate()
    {
        ab.velocity = speed * forwardDirection.normalized;
        ab.angularVelocity = xMove * angularSpeed * Vector3.up;
    }
}
