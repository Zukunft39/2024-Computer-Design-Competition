using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public float e1,e2,angle;
    
    private Mesh triangleMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    /// <summary>
    /// Create triangle by 3 vertices
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    void CreateTriangle(Vector3 p1,Vector3 p2,Vector3 p3)
    {
        vertices=new[]{p1,p2,p3};
        triangles=new[]{0,1,2};
        uvs=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(0,1)};
        triangleMesh.vertices=vertices;
        triangleMesh.triangles=triangles;
        triangleMesh.uv=uvs;
        triangleMesh.normals=new[]{-Vector3.forward,-Vector3.forward,-Vector3.forward};
    }
    
    /// <summary>
    /// Create triangle by SAS
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="angle"></param>
    /// <param name="e2"></param>
    void CreateTriangle(float e1,float angle,float e2)
    {
        Vector3 p1 =Vector3.zero;
        Vector3 p2 = new Vector3(p1.x+e1, p1.y, p1.z);
        Vector3 p3 = new Vector3(p1.x+Mathf.Cos(Mathf.Deg2Rad* angle)*e2, p1.y+Mathf.Sin(Mathf.Deg2Rad*angle)*e2, p1.z);
        CreateTriangle(p1,p2,p3);
    }

    Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        CreateTriangle(p1,p2,p3);
    }
    
    Triangle(float e1,float angle,float e2)
    {
        CreateTriangle(e1,angle,e2);
    }

    private void Awake()
    {
        triangleMesh = new();
        GetComponent<MeshFilter>().mesh=triangleMesh;
        
    }

    private void Start()
    {
       CreateTriangle(e1,angle,e2);
    }

    private void OnValidate()
    {
        Awake();
        CreateTriangle(e1,angle,e2);
    }
}
