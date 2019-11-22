using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger ");
        if (other.CompareTag("Player")) {

            EffectManager.instance.InvokeBloom();
            
            SplineManager.instance.DisableTunnelGenerator();
        }

    }
}
