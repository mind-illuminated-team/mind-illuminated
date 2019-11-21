using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
public class SplineCart : MonoBehaviour
{

    private Spline currentSpline;
    private float rate = 0;

    public GameObject Follower;
    public float DurationInSecond;
    public float ViewOffset = 0.1f;

    private int splineCount = 0;
    private float nextSplinejumpPoint = .99f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {



        rate += Time.deltaTime / DurationInSecond;

        if (currentSpline != null)
        {
            if (rate > currentSpline.nodes.Count - 1)
            {
                rate = 0;


                // --- Beni - 11.20
                // spline[0] has the complete track, so this logic is unneseccary and 
                // causes a bug once the cart reaches the end of the track
                // since at the end of spline[0] we should not go spline[1]
                // but to the beggining of spline[0], since it has the complete track

                //GetNextSpline();

                // --- END
            }
        }
        PlaceFollower();
        
       

    }

    private void PlaceFollower()
    {
        if (currentSpline != null)
        {
            if (Follower != null)
            {
                CurveSample sample = currentSpline.GetSample(rate);
                Follower.transform.localPosition = currentSpline.WorldPosition(sample.location);
                Follower.transform.localRotation = sample.Rotation;

              
            }
        }
        else {
            currentSpline = SplineManager.instance.GetSplineComponent(splineCount);
        }

    }

    private void GetNextSpline() {
        splineCount++;

        currentSpline = SplineManager.instance.GetSplineComponent(splineCount);
    }
}
