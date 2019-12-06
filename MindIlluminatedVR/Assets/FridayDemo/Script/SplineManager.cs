using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using System.Threading.Tasks;
using System.Threading;

using Sensors;

public class SplineManager : MonoBehaviour
{

    public static SplineManager instance;

    // Fear level parameters
    public float fearLevelTresholdPercantage = 10;
    public float degreeOfSum = 1;
    [HideInInspector]
    public int currentFearLevel = 2;
    private float lastAverage = 0;
    // Generation parameters
    public float generationDistance = 20;

    public GameObject[] patternsPrefab;

    [Tooltip("Fear level of the spline: 0-3. Level 0 will be the first spline, only 1 level 0 patterns should be added")]
    // Level 0 patter is just for taking measurments in the beggining
    public int[] patternsPrefabFearLevels;



    public GameObject connectorPatternsPrefab;
    private List<GameObject> splines;

    private Spline baseSpline;
    private GameObject caveMeshGenerated;

    public SplineMeshTiling splineMeshTiling;
    private Thread managerThread;

    private float lastGenerationTime = 0;
    private int generationIndex = 1;

    #region DemoVariables

    public GameObject[] portalPrefab;
    private int portalCallAtSegment = 1;
    #endregion

    #region MaterialChange

    private TrackContortion trackContortion;
    public Material[] tunnelMaterial;

    #endregion

    private bool inGeneration = false;



    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        /*managerThread = new Thread(new ThreadStart(Setup));
        managerThread.Start();*/

        currentFearLevel = 2;

        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        Spline spline = GetSplineComponent(0);

        //Extend spline if we are close to the end
        if (!inGeneration && spline.Length - SplineCart.instance.distance < generationDistance)
        {
            inGeneration = true;
            SetFearLevel();

            int nextSplineIndex = ChooseNextSpline();
            ExtendSpline(nextSplineIndex);
        }
    }

        //if (Time.time - lastGenerationTime > 2 && generationIndex < splineCount)
        //{
        //    lastGenerationTime = Time.time;

        //    ExtendSpline(generationIndex);

        //    generationIndex++;
        //}
    //}

    void SetFearLevel()
    {
        float? average = SensorDataProvider.Instance.GetAverageSinceLastCheckPoint(degreeOfSum);
        if (!average.HasValue)
        {
            Debug.Log("Average was NULL from SensorDataProvider");
            return;
        }
        float currentAverage = average.Value;

        // current average is smaller -> increase fear level
        if (lastAverage / currentAverage >= (1f + fearLevelTresholdPercantage / 100))
        {
            // 3 = max fear level for now
            if (currentFearLevel < 3)
                currentFearLevel++;
        }

        // Current average is larger -> decrease fear level
        if (currentAverage / lastAverage >= (1f + fearLevelTresholdPercantage / 100))
        {
            // 1 = min fear level for now
            if (currentFearLevel > 1)
                currentFearLevel--;
        }

        lastAverage = currentAverage;
        
        // change speed according to speed level
        //SplineCart.instance.speed = SplineCart.instance.baseSpeed * (1f + 0.2f * (currentFearLevel-1));

        Debug.Log("Average: " + currentAverage);
        Debug.Log("Determined fear level: " + currentFearLevel);
    }

    int ChooseNextSpline()
    {
        if (patternsPrefab.Length != patternsPrefabFearLevels.Length)
        {
            Debug.Log("(patternsPrefab.Length != patternsPrefabFearLevels.Length");
            return 0;
        }

        List<int> matchingFearLevelIndexes = new List<int>();

        for (int i=0; i< patternsPrefabFearLevels.Length; i++)
        {
            if (patternsPrefabFearLevels[i] == currentFearLevel)
            {
                matchingFearLevelIndexes.Add(i);
            }
        }

        int randInd = Random.Range(0, matchingFearLevelIndexes.Count);

        Debug.Log("# of possible splines: " + matchingFearLevelIndexes.Count);
        Debug.Log("Chosen spline index: " + matchingFearLevelIndexes[randInd]);        

        return matchingFearLevelIndexes[randInd];
    }

    private void Setup() {
        splines = new List<GameObject>();

        for (int i = 0; i < patternsPrefab.Length; i++) {

            //int rand = GetRandomIndex(patternsPrefab.Length);
            //GameObject tempSpline = Instantiate(patternsPrefab[rand], Vector3.zero, Quaternion.identity) as GameObject;
            //tempSpline.name= "spline " + i + "_" +patternsPrefab[rand].name;

            GameObject tempSpline = Instantiate(patternsPrefab[i], Vector3.zero, Quaternion.identity) as GameObject;
            tempSpline.name = "spline " + i + "_" + patternsPrefab[i].name;

            // --- Beni - 11.20.
            // Remove stuff from "background?" splines
            if (i != 0)
            {
                Spline tmpSplineComponent = tempSpline.transform.GetChild(0).GetComponent<Spline>();

                // Remove contortion
                TrackContortion contComponent = tmpSplineComponent.GetComponent<TrackContortion>();
                if (contComponent != null)
                    contComponent.gameObject.SetActive(false);

                // Remove tunnel
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





            if (i == 0)
            {

                baseSpline = GetSplineComponent(0);
                if (baseSpline.GetComponent<SplineMeshTiling>() != null)
                    baseSpline.GetComponent<SplineMeshTiling>().enabled = true;
                
                // --- Beni - 11.30.
                // Set generated cave object to be disabled later
                if (baseSpline.GetComponent<TrackContortion>() != null)
                {
                    trackContortion = baseSpline.GetComponent<TrackContortion>();
                    caveMeshGenerated = baseSpline.GetComponent<TrackContortion>().transform.GetChild(0).gameObject;
                }
                else
                {
                    if (baseSpline.transform.GetChild(0).GetComponent<SplineMeshTiling>() != null)
                        caveMeshGenerated = baseSpline.transform.GetChild(0).GetComponent<SplineMeshTiling>().gameObject;
                }
                // --- END
            }

            if (i == 0)
            {
                Vector3 firstPortalPos = baseSpline.WorldPosition(baseSpline.nodes[baseSpline.nodes.Count - 2].Position);
                Instantiate(portalPrefab[Random.Range(0, portalPrefab.Length)], firstPortalPos, Quaternion.identity);
            }
}
        // baseSpline.GetComponent<SplineMeshTiling>().CreateMeshOnTheRun(0);        
        //StartCoroutine(SplineConnector());

    }

    #region Demo

    public void DisableTunnelGenerator() {

        Invoke("DisableTunnel", .5f);

    }

    int tunnelIndex = 0;

    private void DisableTunnel() {
        // Doesn't actually disable generation, only hides generated gameObject
        if (caveMeshGenerated.active)
        {
            caveMeshGenerated.SetActive(false);

       
           
        }
        else {
            caveMeshGenerated.SetActive(true);
            Material currentTunnelMaterial = tunnelMaterial[tunnelIndex];
            tunnelIndex++;
            tunnelIndex = tunnelIndex % tunnelMaterial.Length;
            trackContortion.SetMaterial(currentTunnelMaterial);
        }
    }

    #endregion
    //IEnumerator SplineConnector() {

    //    for (int i = 1; i < splineCount; i++)
    //    {
    //        yield return new WaitForSeconds(9);

    //        // not a good approach, cannot change speed dynamically, might have to use this if the other approach does not work
    //        //yield return new WaitForSeconds(time_it_takes_to_reach_end_of_next_spline);

    //        ExtendSpline(i);            
    //    }
    //}

    private int GetRandomIndex(int max) {
        return Random.Range(0,max);
    }
    private void PositionSpline(int index) {
        Spline currentSpline = GetSplineComponent(0);

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

        PositionSpline(index);
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
        //if (index == portalCallAtSegment)
        if (index != 0)
        {
            Instantiate(portalPrefab[Random.Range(0, portalPrefab.Length)], nodesPosition[nodeCount - 4], Quaternion.identity);
            portalCallAtSegment += 1;
        }

        new Thread(() =>
        {
            int LastNodeIndexPerm = lastNodeIndex;
            baseSpline.RemoveNode(baseSpline.nodes[lastNodeIndex]);
            for (int i = 0; i < nodeCount - 1; i++)
            {

                SplineNode node = new SplineNode(nodesPosition[i], nodesDirection[i]);
                baseSpline.AddNode(node);
                lastNodeIndex++;
                //yield return new WaitForSeconds(nodeExtendDelay);
            }

            inGeneration = false;
        }).Start();


        //StartCoroutine(ExtendNode(nodesPosition, nodesDirection, nodesUp, nodesRoll, lastNodeIndex, nodeCount));

    }    

    private float nodeExtendDelay = 1f;
    private IEnumerator ExtendNode(List<Vector3> nodesPosition, List<Vector3> nodesDirection, List<Vector3> nodesUp,List<float> nodesRoll, int lastnodeIndex, int nodeCount) {
        int LastNodeIndexPerm = lastnodeIndex;
        baseSpline.RemoveNode(baseSpline.nodes[lastnodeIndex]);
        List<SplineNode> tempNodes = new List<SplineNode>();
        List<SplineNode> SNCollector = new List<SplineNode>();
        for (int i = 0; i < nodeCount-1; i++) {
            
            SplineNode node = new SplineNode(nodesPosition[i], nodesDirection[i]);
            node.Up = nodesUp[i];
            
            tempNodes.Add(node);
            SNCollector.Add(node);
            baseSpline.AddNode(node);
            lastnodeIndex++;
            yield return new WaitForSeconds(nodeExtendDelay);
        }
        //baseSpline.AddMultipleNodes(SNCollector);
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
