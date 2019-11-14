using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelGenerator : MonoBehaviour
{
    public GameObject tunnelPrefab;
    public int frequency = 1;
    public List<Vector3> waypoints;

    private GameObject baseTunnel;
    private GameObject newTunnel;
    private GameObject prevTunnel;

    private bool start;

    private float lastUpdate = 0;

    private Random random;

    // Start is called before the first frame update
    void Start()
    {
        if (tunnelPrefab == null)
        {
            baseTunnel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseTunnel.transform.localScale = new Vector3(8, 8, 8);
            baseTunnel.transform.rotation = Quaternion.Euler(60, 0, 0);
            baseTunnel.transform.position = new Vector3();
            //MeshRenderer meshRenderer = toSpawn.GetComponent<MeshRenderer>();
            //meshRenderer.material = 
            tunnelPrefab = baseTunnel;
        }
        else
        {
            baseTunnel = Instantiate(tunnelPrefab, tunnelPrefab.transform.position, tunnelPrefab.transform.rotation);
            //baseTunnel = Instantiate(tunnelPrefab, tunnelPrefab.transform.position, Quaternion.Euler(90, 0, 0));
        }
        waypoints = new List<Vector3>();

        //GameObject original = toSpawn;
        //toSpawn = Instantiate(toSpawn, toSpawn.transform.position+ Vector3.back *5, toSpawn.transform.rotation);

        if (baseTunnel.GetComponent<TransformCylinder>() == null)
            baseTunnel.AddComponent<TransformCylinder>();

        // for testing, TODO remove
        if (baseTunnel.GetComponent<VisualizeMesh>() != null)
            Destroy(baseTunnel.GetComponent<VisualizeMesh>());

        //if (toSpawn.GetComponent<VisualizeMesh>() == null)
        //{
        //    VisualizeMesh vis_script = toSpawn.AddComponent<VisualizeMesh>();
        //    vis_script.markerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    vis_script.markerObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //}

        start = true;
        prevTunnel = baseTunnel;
        random = new Random();
    }

    // Update is called once per frame
    void Update()
    {
        // Update every second only
        if (Time.time - lastUpdate < frequency)
            return;

        lastUpdate = Time.time;

        if (start)
        {
            Destroy(baseTunnel.GetComponent<TransformCylinder>());
            start = false;
        }

        CreateRandomTunnel();

        ConnectTunnels();
    }

    void CreateRandomTunnel()
    {
        Vector3 spawnPos = prevTunnel.transform.position + Vector3.forward * 20;
        newTunnel = Instantiate(baseTunnel, spawnPos, baseTunnel.transform.rotation);

        Mesh mesh = newTunnel.GetComponent<MeshFilter>().mesh;
        Vector3[] newVertices = new Vector3[mesh.vertices.Length];


        float shift = Random.Range(-1.5f, 1.5f);
        float scale = 1;
        //float scale = Random.Range(0.5f, 1.5f);

        for (int i = 0; i<mesh.vertices.Length;i++)
        {
            // Endpoints
            if (mesh.vertices[i].y == 1)
            {
                newVertices[i].x = mesh.vertices[i].x * scale;
                newVertices[i].y = mesh.vertices[i].y;
                newVertices[i].z = mesh.vertices[i].z * scale + shift;
            }
            // Other points
            else
                newVertices[i] = mesh.vertices[i];
            //Debug.Log(i+"_" + mesh.vertices[i]  + "_" + newVertices[i]);
            //Debug.Log(newVertices[i]);
        }

        mesh.vertices = newVertices;
    }

    // Set position of new tunnel by matching upper endpoint of original with upper startpoint of new
    // save centerNew - startpointNew vector; add it endpointPrev to get new centerNew
    void ConnectTunnels()
    {
        Mesh meshNew = newTunnel.GetComponent<MeshFilter>().mesh;
        Mesh meshPrev = prevTunnel.GetComponent<MeshFilter>().mesh;

        // Relative location on mesh after rotation
        // verticies[40], center of start circle correspond to verticies[41], center of end circle
        Vector3 dirNew = Quaternion.Euler(baseTunnel.transform.rotation.eulerAngles) * new Vector3(meshNew.vertices[40].x, meshNew.vertices[40].y, meshNew.vertices[40].z);
        Vector3 dirPrev = Quaternion.Euler(baseTunnel.transform.rotation.eulerAngles) * new Vector3(meshPrev.vertices[41].x, meshPrev.vertices[41].y, meshPrev.vertices[41].z);
        
        // Location in the wolrd, after scale is applied
        Vector3 reference_pos_new = newTunnel.transform.position + Vector3.Scale(dirNew, baseTunnel.transform.localScale);
        Vector3 reference_pos_prev = prevTunnel.transform.position + Vector3.Scale(dirPrev, baseTunnel.transform.localScale);

        //// Testing visualization
        GameObject markerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //Instantiate(markerObject, reference_pos_new, Quaternion.identity);
        Instantiate(markerObject, reference_pos_prev, Quaternion.identity);

        // Reposition the new tunnel according to the reference points
        Vector3 newCenter = reference_pos_prev + (newTunnel.transform.position - reference_pos_new);
        newTunnel.transform.position = newCenter;

        prevTunnel = newTunnel;
        waypoints.Add(reference_pos_prev);
    }

    //enum TunnelDirection
    //{
    //    Straight,
    //    Ascending,
    //    Descending
    //}

    //TunnelDirection RandomTunnelDirection()
    //{
    //    int rnd = Random.Range(1, 100);
    //    if (rnd <= 60)
    //        return TunnelDirection.Straight;
    //    else if (rnd <= 80)
    //        return TunnelDirection.Ascending;
    //    else
    //        return TunnelDirection.Descending;
    //}
}
