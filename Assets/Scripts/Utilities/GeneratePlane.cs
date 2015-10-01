using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMesh : MonoBehaviour {
	
	public Material mat;
	public List<GameObject> go;
	public List<Vector3> polyL;
	public Vector3[] poly;  // Initialized in the inspector
	
	
	void Start () {
		foreach (GameObject gam in go) {
			polyL.Add (gam.transform.position);
			Debug.Log(gam.transform.position);
			
		}
		poly = polyL.ToArray ();
		if (poly == null || poly.Length < 3) {
			Debug.Log ("Define 2D polygon in 'poly' in the the Inspector");
			return;
		}
		
		MeshFilter mf = gameObject.AddComponent<MeshFilter>();
		
		Mesh mesh = new Mesh();
		mf.mesh = mesh;
		
		Renderer rend = gameObject.AddComponent<MeshRenderer>();
		rend.material = mat;
		
		Vector3 center = FindCenter ();
		
		Vector3[] vertices = new Vector3[poly.Length+1];
		vertices[0] = Vector3.zero;
		
		for (int i = 0; i < poly.Length; i++) {
			//poly[i].z = 0.0f;
			vertices[i+1] = poly[i];
		}
		
		mesh.vertices = vertices;
		
		int[] triangles = new int[poly.Length*3];
		
		for (int i = 0; i < poly.Length-1; i++) {
			triangles[i*3] = i+2;
			triangles[i*3+1] = 0;
			triangles[i*3+2] = i + 1;
		}
		
		triangles[(poly.Length-1)*3] = 1;
		triangles[(poly.Length-1)*3+1] = 0;
		triangles[(poly.Length-1)*3+2] = poly.Length;
		
		mesh.triangles = triangles = triangles;
		mesh.uv = BuildUVs(vertices);
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		
	}
	
	Vector3 FindCenter() {
		Vector3 center = Vector3.zero;
		foreach (Vector3 v3 in poly) {
			center += v3;    
		}
		return center / poly.Length;
	}
	
	Vector2[] BuildUVs(Vector3[] vertices) {
		
		float xMin = Mathf.Infinity;
		float yMin = Mathf.Infinity;
		float xMax = -Mathf.Infinity;
		float yMax = -Mathf.Infinity;
		
		foreach (Vector3 v3 in vertices) {
			if (v3.x < xMin)
				xMin = v3.x;
			if (v3.y < yMin)
				yMin = v3.y;
			if (v3.x > xMax)
				xMax = v3.x;
			if (v3.y > yMax)
				yMax = v3.y;
		}
		
		float xRange = xMax - xMin;
		float yRange = yMax - yMin;
		
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			uvs[i].x = (vertices[i].x - xMin) / xRange;
			uvs[i].y = (vertices[i].y - yMin) / yRange;
			
		}
		return uvs;
	}
}