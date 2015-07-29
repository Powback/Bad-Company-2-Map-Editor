using UnityEngine;
using System.Collections;

public class ChangeUV : MonoBehaviour
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public float multiplier = 1;
    void FixedUpdate()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        uvs = new Vector2[vertices.Length];
			
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x * multiplier, vertices[i].z * multiplier);
        }
        mesh.uv = uvs;


    }
}
