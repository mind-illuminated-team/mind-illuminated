using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipImageCameraEffect : MonoBehaviour
{
    public Vector3 flipDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FlipCamera();
    }


    private void FlipCamera() {

        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat *= Matrix4x4.Scale(flipDirection);
        Camera.main.projectionMatrix = mat;

    }
}
