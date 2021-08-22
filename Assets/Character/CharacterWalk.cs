using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalk : MonoBehaviour
{
    public GameObject character;
    private Rigidbody rb;

    public float speed = 1.0f;
    public float angularSpeed = 1.0f;
    private float angularSpeedDegree;

    public bool loop;
    public Vector3[] targetTrajectory;
    private int currentIndex;
    public Vector3 currentTarget;
    private Quaternion currentTargetRotation;
    
    private Animator animator;

    void Start()
    {
        rb = character.GetComponent<Rigidbody>();
        animator = character.GetComponentInChildren<Animator>();

        currentTarget = new Vector3(0f, -1f, 0f);
        if (targetTrajectory.Length != 0)
            MoveTrajectory(targetTrajectory);
       
        angularSpeedDegree = angularSpeed * Mathf.Rad2Deg;
    }

    void FixedUpdate()
    {
        if (currentTarget.y != -1f && (rb.position - currentTarget).magnitude > 0.5)
        {
            // Moving
            animator.enabled = true;

            Vector3 position = Vector3.MoveTowards(rb.position, currentTarget, 
                                                   speed * Time.fixedDeltaTime);
            rb.MovePosition(position);
            Quaternion rotation = Quaternion.RotateTowards(rb.rotation, currentTargetRotation, 
                                                   angularSpeedDegree * Time.fixedDeltaTime);
            rb.MoveRotation(rotation);
        }
        else
        {
            // Arrive
            animator.enabled = false;

            if (targetTrajectory.Length == 0)
                return;

            if (loop)
            {
                currentIndex = (currentIndex+1) % targetTrajectory.Length;
                MoveTo(targetTrajectory[currentIndex]);
            }
            else
            {
                if (currentIndex+1 != targetTrajectory.Length)
                {
                    currentIndex += 1;
                    MoveTo(targetTrajectory[currentIndex]);
                }
            }
        }
    }

    public void MoveTo(Vector3 target)
    {
        currentTarget = target;
        currentTargetRotation = Quaternion.LookRotation(currentTarget - rb.position);
    }

    public void MoveTrajectory(Vector3[] trajectory)
    {   
        targetTrajectory = trajectory; 
        currentIndex = 0;
        currentTarget = targetTrajectory[currentIndex];
    }
}