using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Transform[] cameraLocations;

    void Start()
    {
        Transform origin = cameraLocations[0];
        camera.transform.parent = origin;
        camera.transform.position = origin.position;
        camera.transform.rotation = origin.rotation;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
