using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BC2;

public class TerrainSplineData : MonoBehaviour {

    public Inst inst;
    public List<Inst> points = new List<Inst>();
    public List<Vector3> pointPos = new List<Vector3>();
    public MapLoad ml;


    public void Start()
    {
        inst = this.gameObject.GetComponent<BC2Instance>().instance;
        BC2Array array = Util.SelectArray("Points", inst);
        foreach (Item Item in array.item)
        {
            ml = Util.SelectMapload();
            Inst refPoint = Util.SelectByGUID(Item.reference, ml.partition);
            Debug.Log(refPoint.guid);
            points.Add(refPoint);
        }
        foreach(Inst point in points)
        {
            Complex posString = Util.SelectComplex("Position", point);
            Vector3 pos = Util.CalculatePositionFromString(posString.value);
            pointPos.Add(pos);
        }

      
        LineRenderer LR = gameObject.AddComponent<LineRenderer>();
        LR.SetVertexCount(pointPos.Count);
        for(int i = 0; i < pointPos.Count; i++)
        {
            LR.SetPosition(i, pointPos[i]);
        }

        BC2Array planes = Util.SelectArray("Planes", inst);
        Inst plane = Util.SelectByGUID(planes.item[0].reference, ml.partition);
        if(Util.SelectField("PlaneType", plane).value == "Lake")
        {
            GeneratePlane gp = transform.gameObject.AddComponent<GeneratePlane>();
            foreach(Vector3 v3 in pointPos)
            {
                gp.points.Add(v3);
            }
            Vector3 startpos = new Vector3();
            startpos.y = pointPos[0].y;
            transform.position = startpos;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            gp.mat = ml.material_white;
            gp.Generate();
        }

    }
}
