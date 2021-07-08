using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    public GameObject gameObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))  
        {  
            gameObject.transform.Translate(2f * Vector3.forward * Time.deltaTime);  
        }  
         
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))  
        {  
            gameObject.transform.Translate(2f * Vector3.back * Time.deltaTime);  
        }  
         
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {  
            gameObject.transform.Rotate(Vector3.up, -0.5f);  
        }  
        
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) 
        {  
            gameObject.transform.Rotate(Vector3.up, 0.5f);  
        }  
    }
}
