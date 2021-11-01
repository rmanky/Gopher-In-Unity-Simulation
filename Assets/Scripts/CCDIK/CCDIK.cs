using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CCDIK : MonoBehaviour
{
    [SerializeField]
    private Transform goal;

    [SerializeField]
    private ArticulationBody effector;

    [SerializeField]
    private ArticulationBody baseBody;

    [SerializeField]
    private float sqrDistError = 0.01f;

    [SerializeField][Range(0.0f, 1.0f)]
    private float weight = 1.0f;

    [SerializeField]
    private int maxIterationCount = 500;

    [SerializeField][Range(1.0f, 5.0f)]
    private float lerpSpeed = 1.0f;

    List<ArticulationBody> bodyList;

    List<Quaternion> axisList;

    void OnEnable ()
    {
        bodyList = new List<ArticulationBody>();
        axisList = new List<Quaternion>();
        ArticulationBody current = effector;

        while (current != null && current.gameObject != baseBody.gameObject)
        {
            bodyList.Add(current);
            axisList.Add(current.anchorRotation);
            current = current.transform.parent.GetComponent<ArticulationBody>();
        }
        if (current == null)
        {
            throw new UnityException("Base Bone is not ancestor of effector, IK will fail!");
        }
    }

    void Update() {
        Solve();
    }

    void Solve()
    {
        Vector3 goalPosition = goal.position;
        Vector3 effectorPosition = effector.transform.position;

        Vector3 targetPosition = Vector3.Lerp (effectorPosition, goalPosition, weight);
        float sqrDistance;

        int iterationCount = 0;
        do
        {
            int i = 0;
            foreach (ArticulationBody joint in bodyList) {
                Transform jointTransform = joint.transform;
                Vector3 directionToEffector = effector.transform.position - jointTransform.position;
                Vector3 directionToGoal = goal.position - jointTransform.position;
                Vector3 axisOfRotation = axisList[i] * Vector3.right;

                RotateFromTo(jointTransform, directionToEffector, directionToGoal);
                RotateFromTo(jointTransform, jointTransform.rotation * axisOfRotation, jointTransform.parent.rotation * axisOfRotation);

                sqrDistance = (effector.transform.position - targetPosition).sqrMagnitude;
                i++;

                if (sqrDistance <= sqrDistError)
                {
                    break;
                }
            }

            sqrDistance = (effector.transform.position - targetPosition).sqrMagnitude;
            iterationCount++;
        }
        while (sqrDistance > sqrDistError && iterationCount < maxIterationCount);
    }

    void RotateFromTo(Transform joint, Vector3 a, Vector3 b)
    {
        Quaternion fromToRotation = Quaternion.FromToRotation(a, b) * joint.rotation;
        joint.rotation = fromToRotation;
    }

    float ClampAngle(float angle, float from, float to)
     {
         // accepts e.g. -80, 80
         if (angle < 0f) angle = 360 + angle;
         if (angle > 180f) return Mathf.Max(angle, 360+from);
         return Mathf.Min(angle, to);
     }
}
