using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float movespeed = 10;
    public float rotspeed = 3;

    void Update()
    {
        CameraControl();
    }

    void CameraControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * movespeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * movespeed);
        }
        if (Input.GetKey(KeyCode.R))
        {
            transform.Translate(Vector3.up * Time.deltaTime * movespeed);
        }
        if (Input.GetKey(KeyCode.F))
        {
            transform.Translate(Vector3.down * Time.deltaTime * movespeed); ;
        }
        // Rotate camera
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -1 * rotspeed, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 1 * rotspeed, 0));
        }
    }
}
