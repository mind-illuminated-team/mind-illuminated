using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackWhite : MonoBehaviour
{

    public Material tileMaterial;
    public float speed;
    private float value = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        value += speed * Time.deltaTime;
        tileMaterial.SetTextureOffset("_MainTex", new Vector2(0,value));
    }
}
