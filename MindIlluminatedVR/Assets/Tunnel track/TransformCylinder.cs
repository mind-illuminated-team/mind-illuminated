using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TransformCylinder : MonoBehaviour
{
    private Mesh mesh;
    private int[] newTriangles;

    
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        ApplySlope();
        RenderTunnel();
    }

    // Transform cylinder into tunnel
    // - order of triangle coordinates are swapped, which makes unity render the "inside"
    private void RenderTunnel()
    {
        #region Approach #1
        // - second half of triangles array dropped, they correspond to the circle bases

        newTriangles = new int[mesh.triangles.Length / 2];

        for (int i = 0; i < mesh.triangles.Length / 2; i = i + 3)
        {
            newTriangles[i] = mesh.triangles[i];
            newTriangles[i + 1] = mesh.triangles[i + 2];
            newTriangles[i + 2] = mesh.triangles[i + 1];
        }

        mesh.triangles = newTriangles;
        #endregion

        #region Approach #2
        //// - drop triangles which form the circle base of the cylinders
        
        //newTriangles = new int[mesh.triangles.Length];
        //int droppedcount = 0;
        //for (int i = 0; i < mesh.triangles.Length; i = i + 3)
        //{
        //    // Vertex #40/41 = center of circle base
        //    if (mesh.triangles[i] != 40 && mesh.triangles[i] != 41 &&
        //        mesh.triangles[i + 1] != 40 && mesh.triangles[i + 1] != 41 &&
        //        mesh.triangles[i + 2] != 40 && mesh.triangles[i + 2] != 41)
        //    {
        //        newTriangles[i] = mesh.triangles[i];
        //        newTriangles[i + 1] = mesh.triangles[i + 2];
        //        newTriangles[i + 2] = mesh.triangles[i + 1];
        //    }
        //    else
        //    {
        //        droppedcount++;
        //    }
        //}

        //// For testing
        //Debug.Log("Original: " + mesh.triangles.Length);
        //Debug.Log("Dropped: " + droppedcount * 3);
        //for (int i = 0; i < newTriangles.Length; i = i + 3)
        //{
        //    string log = i + ": " + newTriangles[i] + "," + newTriangles[i + 1] + "," + newTriangles[i + 2];
        //    Debug.Log(log);
        //}

        //mesh.triangles = newTriangles;
        #endregion
    }

    private void ApplySlope()
    {

    }
}
