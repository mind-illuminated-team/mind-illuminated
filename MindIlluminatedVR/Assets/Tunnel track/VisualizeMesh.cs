using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisualizeMesh : MonoBehaviour
{
    public GameObject markerObject;
    public float scale;

    public List<Vector3> verticies;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        markerObject.transform.localScale = new Vector3(scale, scale, scale);


        markerObject.name = gameObject.name + "_Marker_Center";
        Instantiate(markerObject, transform.position, new Quaternion(0, 0, 0, 0));

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            // Position vector of the verticies after rotation, relative to the center of the object
            Vector3 dir = Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z);
            // Location in the wolrd, after scale is applied
            Vector3 vertex_pos = transform.position + Vector3.Scale(dir, transform.localScale);

            if (verticies == null)
                verticies = new List<Vector3>();
            verticies.Add(vertex_pos);

            markerObject.name = gameObject.name + "_Marker_" + i;
            Instantiate(markerObject, vertex_pos, Quaternion.identity);
            //Instantiate(markerObject, mesh.vertices[i], Quaternion.identity);
        }
    }
}

public class MI_MeshVertex
{
    private Vector3 vertex;
    // Index of the circle the vertex is on
    private int circleInd;
}
