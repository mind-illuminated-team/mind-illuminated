using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMovement : MonoBehaviour
{

    private RectTransform rectTransform;
    private float magnitude = 50;


    public int direction = 1;
    public float angle = 0;
    private float sinMagnitude = 200;
    private float angleIncrement = 10;
    private float sinFrequency = 1;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    private void Setup() {

        rectTransform = GetComponent<RectTransform>();

    }
    // Update is called once per frame
    void Update()
    {
        SinWaveMovement();
    }

    public void SetInitialAngle(float value) {
        angle = value;
    }
    private void SinWaveMovement() {


        Vector3 tempPosition = rectTransform.localPosition;

        float deltaX = sinMagnitude * (Mathf.Sin( sinFrequency * Mathf.Deg2Rad* angle ));
        Debug.Log(deltaX);

        tempPosition += direction*  Vector3.right * deltaX *Time.deltaTime;
        rectTransform.localPosition = tempPosition;

        angle += angleIncrement;
    }
}
