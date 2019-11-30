using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
public class SplineCart : MonoBehaviour
{

    private Spline currentSpline;
    private float distance = 0;

    public GameObject Follower;
    //public float DurationInSecond;
    [Tooltip("Speed along spline in meters/second.")]
    public float Speed;

    private int splineCount = 0;
    private float nextSplinejumpPoint = .99f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentSpline == null)
            currentSpline = SplineManager.instance.GetSplineComponent(0);

        distance += Time.deltaTime * Speed;
        if (distance >= currentSpline.Length)
            distance = 0;
        PlaceFollower();
    }

    //void Update()
    //{
    //    distance += Time.deltaTime / DurationInSecond;

    //    if (currentSpline != null)
    //    {
    //        if (distance > currentSpline.nodes.Count - 1)
    //        {
    //            distance = 0;

    //            // --- Beni - 11.20.
    //            // spline[0] has the complete track, so this logic is unneseccary and 
    //            // causes a bug once the cart reaches the end of the track
    //            // since at the end of spline[0] we should not go spline[1]
    //            // but to the beggining of spline[0], since it has the complete track

    //            //GetNextSpline();

    //            // --- END
    //        }
    //    }
    //    PlaceFollower();             
    //}

    private void PlaceFollower()
    {
        if (currentSpline != null)
        {
            if (Follower != null)
            {
                //// --- Beni 11.28.
                //// Changed to match contruder
                //// But it doesn't work because the spline length changes after generation
                //rate += Time.deltaTime / DurationInSecond;
                //if (rate > 1)
                //{
                //    rate--;
                //}
                //CurveSample sample = currentSpline.GetSampleAtDistance(currentSpline.Length * rate);
                //// --- END

                //CurveSample sample = currentSpline.GetSample(rate);
                CurveSample sample = currentSpline.GetSampleAtDistance(distance);
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
