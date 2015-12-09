using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratePlane : MonoBehaviour {
	
	public Material mat;
	public List<Vector3> points = new List<Vector3>();
    private List<Vector2> point_pos = new List<Vector2>();

    public void Generate ()
    {

        // Create Vector2 vertices
        Vector2[] vertices2D = new Vector2[] { };
        foreach (Vector3 point in points)
        {
            point_pos.Add(new Vector2(point.x,point.z));
        }
        vertices2D = point_pos.ToArray();
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
		if(gameObject.GetComponent<MeshRenderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
			gameObject.AddComponent<MeshFilter>();
		}
		MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
        filter.mesh = msh;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] UVvertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(UVvertices[i].x / 10, UVvertices[i].z / 10);
        }
        mesh.uv = uvs;
    }

}