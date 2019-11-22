using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
public class SplineManager : MonoBehaviour
{

    public static SplineManager instance;

    public int splineCount = 3;
    public GameObject[] patternsPrefab;
    public GameObject connectorPatternsPrefab;
    private List<GameObject> splines;

    private Spline baseSpline;
    private SplineMeshTiling caveMeshGenerator;
    public SplineMeshTiling splineMeshTiling;

    #region DemoVariables

    public GameObject portalPrefab;
    private int portalCallAtSegment = 1;
    #endregion


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Setup() {
        splines = new List<GameObject>();

        for (int i = 0; i < splineCount; i++) {

            int rand = GetRandomIndex(patternsPrefab.Length);
            GameObject tempSpline = Instantiate(patternsPrefab[rand], Vector3.zero, Quaternion.identity) as GameObject;
            tempSpline.name= "spline " + i;

            // --- Beni - 11.20.
            // Remove tunnel from "background?" splines
            if (i != 0)
            {
                Spline tmpSplineComponent = tempSpline.transform.GetChild(0).GetComponent<Spline>();

                for (int j = 0; j < tmpSplineComponent.transform.childCount; j++)
                {
                    Transform child = tmpSplineComponent.transform.GetChild(j);
                    if (child.name == "Tunnel")
                    {
                        child.parent = null;
                        Destroy(child.gameObject);
                    }
                }
            }
            // --- END

            splines.Add(tempSpline);



            if (i > 0)
            {
                PositionSpline(i);

            }
            else {

                baseSpline = GetSplineComponent(0);
                baseSpline.GetComponent<SplineMeshTiling>().enabled = true;
                caveMeshGenerator = baseSpline.transform.GetChild(0).GetComponent<SplineMeshTiling>();
                
            }
          



        }


       // baseSpline.GetComponent<SplineMeshTiling>().CreateMeshOnTheRun(0);
        StartCoroutine(SplineConnector());

    }

    #region Demo

    public void DisableTunnelGenerator() {

        Invoke("DisableTunnel", .5f);

    }

    private void DisableTunnel() {
        caveMeshGenerator.gameObject.SetActive(false);
    }

    #endregion
    IEnumerator SplineConnector() {

        for (int i = 1; i < splineCount; i++)
        {

           

            ExtendSpline(i);
            yield return new WaitForSeconds(9);
        }



    }

    private int GetRandomIndex(int max) {
        return Random.Range(0,max);
    }
    private void PositionSpline(int index) {

        Spline currentSpline = GetSplineComponent(index - 1);

        //Debug.Log(currentSpline.transform.parent.name);
        int lastNodeIndex = currentSpline.nodes.Count - 1;
        //Debug.Log(currentSpline.nodes[lastNodeIndex].WorldPosition(currentSpline.transform));
        splines[index].transform.position = currentSpline.nodes[lastNodeIndex].WorldPosition(currentSpline.transform);
       
    }

    public Spline GetSplineComponent(int index) {

        return splines[index].transform.GetChild(0).GetComponent<Spline>();
    }


    private void ExtendSplineTest() {

        int lastNodeIndex = baseSpline.nodes.Count - 1;
        SplineNode node = new SplineNode(new Vector3(15, 0, 0), new Vector3(1, 0, 0));
       

    }
    private void ExtendSpline(int index) {

        Spline nextSpline = GetSplineComponent(index);
        List<Vector3> nodesPosition = new List<Vector3>();
        List<Vector3> nodesDirection = new List<Vector3>();
        List<Vector3> nodesUp = new List<Vector3>();
        List<float> nodesRoll = new List<float>();
        int nodeCount = nextSpline.nodes.Count;

        for (int i = 1; i < nodeCount; i++) {

            nodesPosition.Add(nextSpline.WorldPosition(nextSpline.nodes[i].Position));
            //nodesDirection.Add(new Vector3(nextSpline.nodes[i].Direction.x, nextSpline.nodes[i].Direction.y, nextSpline.nodes[i].Direction.z));

           // Debug.Log("direction" + nextSpline.nodes[i].Direction);
            nodesDirection.Add(nextSpline.WorldPosition(nextSpline.nodes[i].Direction));
            nodesUp.Add(nextSpline.WorldPosition(nextSpline.nodes[i].Up));
            nodesRoll.Add(nextSpline.nodes[i].Roll);
        }

        

        int lastNodeIndex = baseSpline.nodes.Count - 1;


        //For DEMO
        if (index == portalCallAtSegment) {
            Instantiate(portalPrefab, nodesPosition[nodeCount - 4]+Vector3.up, Quaternion.identity);
        }

        StartCoroutine(ExtendNode(nodesPosition,nodesDirection, nodesUp,nodesRoll,lastNodeIndex, nodeCount)) ;

    }

    private float nodeExtendDelay = 1f;
    private IEnumerator ExtendNode(List<Vector3> nodesPosition, List<Vector3> nodesDirection, List<Vector3> nodesUp,List<float> nodesRoll, int lastnodeIndex, int nodeCount) {

        //Vector3 positionShift= baseSpline.WorldPosition()
        //Vector3 positionShift= baseSpline.WorldPosition()
        int LastNodeIndexPerm = lastnodeIndex;
        SplineNode nodeToBeRemoved = baseSpline.nodes[lastnodeIndex];
        baseSpline.RemoveNode(nodeToBeRemoved);
        List<SplineNode> tempNodes = new List<SplineNode>();
        for (int i = 0; i < nodeCount-1; i++) {

            //baseSpline.nodes.Add(new SplineNode(nodesPosition[i], nodesDirection[i]));
            SplineNode node = new SplineNode(nodesPosition[i], nodesDirection[i]);
            tempNodes.Add(node);
            //node.Up = nodesUp[i];
            //node.Roll = nodesRoll[i];
            baseSpline.AddNode(node);
            lastnodeIndex++;
            yield return new WaitForSeconds(nodeExtendDelay);
        }

       





        //baseSpline.GetComponent<SplineMeshTiling>().CreateMeshOnTheRun(LastNodeIndexPerm);

    }

    private void Connect(int index) {
        Spline previousSpline = GetSplineComponent(index - 1);
        int lastNodeIndexPrevious = previousSpline.nodes.Count - 1;

        Spline currentSpline = GetSplineComponent(index);
        int lastNodeIndexCurrent = previousSpline.nodes.Count - 1;

        

        currentSpline.nodes[0].Direction = previousSpline.nodes[lastNodeIndexPrevious].Direction;

        //currentSpline.GetComponent<SplineMeshTiling>().enabled = true;
        if (index == 1) {
            previousSpline.GetComponent<SplineMeshTiling>().enabled = true;
        }
        
    }
}
