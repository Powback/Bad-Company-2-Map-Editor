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
        BC2Array array = Util.GetArray("Points", inst);
        foreach (Item Item in array.item)
        {
            ml = Util.GetMapload();
            Inst refPoint = Util.GetInst(Item.reference, ml.partition);
            points.Add(refPoint);
        }
        foreach(Inst point in points)
        {
            Complex posString = Util.GetComplex("Position", point);
            Vector3 pos = Util.GetPositionFromString(posString.value);
            pointPos.Add(pos);
        }

      
        LineRenderer LR = gameObject.AddComponent<LineRenderer>();
        LR.SetVertexCount(pointPos.Count);
        for(int i = 0; i < pointPos.Count; i++)
        {
            LR.SetPosition(i, pointPos[i]);
        }

        BC2Array planes = Util.GetArray("Planes", inst);
        Inst plane = Util.GetInst(planes.item[0].reference, ml.partition);
        if(Util.GetField("PlaneType", plane).value == "Lake")
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
            
            gp.Generate();
            transform.GetComponent<MeshRenderer>().material = ml.waterMaterial;
        }

    }
}
