using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private GameObject player;
    public bool lookForward;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerCar");
    }

    // Update is called once per frame
    void Update()
    {
        if (lookForward) {

            transform.LookAt(player.transform);
        }   
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
