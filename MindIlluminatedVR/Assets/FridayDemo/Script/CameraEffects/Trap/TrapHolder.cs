using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapHolder : MonoBehaviour
{
    private int trapCount = 20;
    private float shiftFactor = 40;
    private float scaleFactor = 10;
    private float angleFactor = 20;
    private float initialShift = -200;

    public Transform leftTrapHolder;
    public Transform rightTrapHolder;

    public TrapMovement leftTrapPrefab;
    public TrapMovement rightTrapPrefab;



    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    private void Setup() {

        for (int i = 0; i < trapCount; i++) {

            GameObject tempLeftTrap = Instantiate(leftTrapPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
         
            tempLeftTrap.transform.parent = leftTrapHolder;
            tempLeftTrap.transform.SetAsFirstSibling();
           
            tempLeftTrap.GetComponent<RectTransform>().localScale = Vector3.one*2;
            Vector2 tempScale = tempLeftTrap.GetComponent<RectTransform>().sizeDelta;
            tempScale.x -= i * scaleFactor;

            tempLeftTrap.GetComponent<RectTransform>().sizeDelta= tempScale;

            Vector3 tempPosition = Vector3.zero;
            tempPosition.x = initialShift + i * shiftFactor;
            //tempPosition.y = initialShift + i * shiftFactor;

            tempLeftTrap.GetComponent<RectTransform>().localPosition = tempPosition;
            tempLeftTrap.GetComponent<TrapMovement>().SetInitialAngle(i*angleFactor);


        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
