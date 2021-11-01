using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantOrbit : MonoBehaviour
{
    [SerializeField]
    private Vector3 origin;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float height;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = origin + radius * new Vector3(Mathf.Cos(Time.time * speed), 0, Mathf.Sin(Time.time * speed));
        transform.position += Vector3.up * height * Mathf.Sin(Time.time * speed / 2.0f);
    }
}
