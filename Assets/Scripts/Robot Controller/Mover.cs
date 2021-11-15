using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // lock the mouse
        Cursor.lockState = CursorLockMode.Locked;   
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime;
        
        // Rotate the object with the mouse X and Y delta
        transform.rotation *= Quaternion.Euler(Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y"));
    }
}
