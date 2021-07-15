using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    public float speed = 1.0f;
    public float angularSpeed = 1.5f;

    private Rigidbody rb;
    private float xMove;
    private float zMove;
    private Vector3 forwardDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        rb.velocity = speed * forwardDirection.normalized;
        rb.angularVelocity = xMove * angularSpeed * Vector3.up;
    }
}
