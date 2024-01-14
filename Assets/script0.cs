using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script0 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Update position based on key input 
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, 0.1f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -0.1f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-0.1f, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(0.1f, 0, 0);
        }
        // Handle vertical movement too
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += new Vector3(0, 0.1f, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += new Vector3(0, -0.1f, 0);
        }
    }
}
