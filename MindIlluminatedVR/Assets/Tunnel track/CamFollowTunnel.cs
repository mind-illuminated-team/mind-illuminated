using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowTunnel : MonoBehaviour
{
    public int cameraSpeed = 15;

    private List<Vector3> waypoints;
    private Vector3 currentDist;
    private Vector3 prevDist;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints == null)
        {
            waypoints = GameObject.Find("TunnelGenerator").GetComponent<TunnelGenerator>().waypoints;
            if (waypoints == null)
                return;
        }

        if (waypoints.Count != 0)
        {            
            currentDist = waypoints[0] - transform.position;

            // if the sign of any coordinate changes then we moved past the current waypoint)
            if (prevDist != null &&
                ((currentDist.x < 0 && prevDist.x > 0) ||
                (currentDist.x > 0 && prevDist.x < 0) ||
                (currentDist.y < 0 && prevDist.y > 0) ||
                (currentDist.y > 0 && prevDist.y < 0) ||
                (currentDist.z < 0 && prevDist.z > 0) ||
                (currentDist.z > 0 && prevDist.z < 0)))
            {
                //for (int i = 0; i < waypoints.Count; i++)
                //{
                //    Debug.Log(count+"_"+waypoints[i]);
                //}
                waypoints.RemoveAt(0);
                // need to make prevdist 0 since there will be a new distance vector for a new waypoint
                prevDist = Vector3.zero;
                return;
            }
            else
            {
                prevDist = currentDist;
            }

            Vector3 dir = currentDist / currentDist.magnitude;
            transform.Translate(dir * Time.deltaTime * cameraSpeed);
        }
    }
}
