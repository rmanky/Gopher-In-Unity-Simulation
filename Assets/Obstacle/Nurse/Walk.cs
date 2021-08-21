using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    public Transform currentTarget;
    private Quaternion currentTargetRotation;
    
    public Transform[] targetTrajectory;
    public bool loop;

    private Rigidbody rb;
    public float speed = 1f;
    public float angularSpeed = 1f;
    
    private float timeCount;
    private int current;
    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
        rb = gameObject.GetComponent<Rigidbody>();
        myAnimator = gameObject.GetComponent<Animator>();

        if (currentTarget != null)
            MoveTo(currentTarget);
        if (targetTrajectory != null)
            MoveTrajectory(targetTrajectory);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentTarget != null && transform.position != currentTarget.position)
        {
            // Moving
            myAnimator.enabled = true;

            Vector3 pos = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(pos);

            rb.rotation = Quaternion.Slerp(transform.rotation, currentTargetRotation, timeCount);
            timeCount += Time.deltaTime;
        }
        else
        {
            // Arrive
            myAnimator.enabled = false;

            if (targetTrajectory != null)
            {
                current += 1;
                if (current == targetTrajectory.Length)
                    if (loop)
                    {
                        current = 0;
                        MoveTo(targetTrajectory[current]);
                    }
                    else
                    {
                        current -= 1;
                    }
                else
                    MoveTo(targetTrajectory[current]);
                
            }
            timeCount = 0.0f;
        }
    }

    public void MoveTo(Transform target)
    {
        currentTarget = target;
        currentTargetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        timeCount = 0.0f;
    }

    public void MoveTrajectory(Transform[] trajectory)
    {
        targetTrajectory = trajectory; 
        current = 0;
        currentTarget = targetTrajectory[current];
    }
}