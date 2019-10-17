using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    public float startPoint;
    public float endPoint;
    public float gravityAcceleration;
    float speed = 0;
    bool forward = true;
    Vector3 rotationVector = new Vector3(0, 180, 0);

    float distance;
    void Start() {
        distance = endPoint - startPoint;
    }

    void Update () {
        float y = transform.position.y;
        if (forward) {
            if (y <= endPoint) {
                transform.Translate(Vector3.up * Time.deltaTime * 2);
            } else {
                //transform.Rotate(rotationVector);
                forward = false;
            }
        } else {
            if (y >= startPoint) {
                speed += gravityAcceleration;
                transform.Translate(Vector3.down * Time.deltaTime * Time.deltaTime * speed);
            } else {
                //transform.Rotate(rotationVector);
                speed = 0;
                forward = true;
            }
        }
    }
}
