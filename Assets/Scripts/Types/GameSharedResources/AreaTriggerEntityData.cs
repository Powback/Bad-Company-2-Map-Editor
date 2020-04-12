using UnityEngine;
using System.Collections;
using System.Globalization;
using BC2;

public class AreaTriggerEntityData : MonoBehaviour {
	public bool showSphere = true;
	GameObject radiusSphere;
	// Use this for initialization
	void Start () {
		showSphere = Util.GetMapload ().showHelpers;
		Inst inst = transform.GetComponent<BC2Instance> ().instance;
		string radiusString = Util.GetField ("Radius", inst).value;
		float radius = float.Parse (radiusString, new CultureInfo("en-US", false));
		radiusSphere = (GameObject)Instantiate (Util.GetMapload ().sphere, transform.position, transform.rotation);
		radiusSphere.transform.localScale = new Vector3 (radius, radius, radius);
		radiusSphere.name = inst.guid + " helper";
		radiusSphere.transform.parent = transform;
		radiusSphere.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		radiusSphere.GetComponent<MeshRenderer> ().receiveShadows = false;
		radiusSphere.GetComponent<MeshRenderer>().material = new Material (Util.GetMapload ().helperMaterial);
		if (!showSphere) { 	
			radiusSphere.transform.gameObject.SetActive (false);
		}
	}

	void ShowHelperSphere(bool active) {
		radiusSphere.SetActive (active);
	}
}
