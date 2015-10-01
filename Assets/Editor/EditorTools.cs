using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using BC2;

[ExecuteInEditMode]
public class EditorTools
{
	Complex position;
	public void OnGUI() {
		Debug.Log("KEK");
		if (Selection.activeGameObject.gameObject.GetComponent<BC2Instance> ()) {
			GameObject selected = Selection.activeGameObject.gameObject;
			if(position.value != null) {
				foreach(Complex complex in selected.GetComponent<BC2Instance>().instance.complex) {
					if(complex.name == "Transform") {
						position = complex;
						Debug.Log("Found complex!");
					}
				}
			} else {

				Matrix4x4 m = Util.GenerateMatrix4x4String(position.value);
				if(m.GetColumn(3).x != selected.transform.position.z || m.GetColumn(3).z != selected.transform.position.x || m.GetColumn(3).y != selected.transform.position.y) { 
					Vector3 pos = selected.transform.position;
					m.SetColumn(3, new Vector4(pos.z,pos.y,pos.x,0));
					string zero = "/*nonzero*/";
					string newComplex = m.GetColumn(0) + zero + m.GetColumn(1) + zero + m.GetColumn(2) + zero + m.GetColumn(3) + zero;
					position.value = newComplex;
					Debug.Log("Updated Complex!");
				} else {
					Debug.Log("Shit's the same");
				}
			}
		}
	}
}
