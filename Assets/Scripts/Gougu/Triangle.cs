using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public float e1,e2,angle1,angle2;
    public Material ghostTriMat;
    public GameObject ghostTriGO;
    public Vector2 rotateOffset;
    public Grid pivotedGrid;
    public TriDir dir=TriDir.LEFT;
    public int triType;
    public TriRepresentMethod triRepresentMethod;
    
    public enum TriDir
    {
        LEFT,UP,RIGHT,DOWN
    }

    public enum TriRepresentMethod
    {
        SAS,SASA
    }
    
    private Mesh triangleMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private MeshCollider meshCollider;
    private GameObject ghostTriGOInstance;
    private bool isPivoted;
    private LineRenderer _lineRenderer;

    /// <summary>
    /// Create triangle by 3 vertices
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public void CreateTriangle(Vector3 p1,Vector3 p2,Vector3 p3)
    {
        vertices=new[]{p1,p2,p3};
        triangles=new[]{0,1,2};
        uvs=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(0,1)};
        triangleMesh.vertices=vertices;
        triangleMesh.triangles=triangles;
        triangleMesh.uv=uvs;
        triangleMesh.normals=new[]{-Vector3.forward,-Vector3.forward,-Vector3.forward};
//        meshCollider.sharedMesh = triangleMesh;
        GetComponent<MeshFilter>().mesh=triangleMesh;
        _lineRenderer.positionCount = 4;
    }
    
    /// <summary>
    /// Create triangle by SAS
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="angle"></param>
    /// <param name="e2"></param>
    public void CreateTriangle(float e1,float angle,float e2)
    {
        Vector3 p1 =Vector3.zero;
        Vector3 p2 = new Vector3(p1.x + e1*GouguData.Instance.gridSize, p1.y, p1.z);
        Vector3 p3 = new Vector3(p1.x+Mathf.Cos(Mathf.Deg2Rad* angle)*e2*GouguData.Instance.gridSize, p1.y+Mathf.Sin(Mathf.Deg2Rad*angle)*e2*GouguData.Instance.gridSize, p1.z);
        CreateTriangle(p1,p2,p3);
    }
    
    public void CreateTriangle(float e1,float e1Angle,float e2,float e2Angle)
    {
        Vector3 p1 =Vector3.zero;
        Vector3 p2 = new Vector3(p1.x+Mathf.Cos(Mathf.Deg2Rad* e1Angle)*e1*GouguData.Instance.gridSize, p1.y+Mathf.Sin(Mathf.Deg2Rad*e1Angle)*e1*GouguData.Instance.gridSize, p1.z);
        Vector3 p3 = new Vector3(p1.x+Mathf.Cos(Mathf.Deg2Rad* e2Angle)*e2*GouguData.Instance.gridSize, p1.y+Mathf.Sin(Mathf.Deg2Rad*e2Angle)*e2*GouguData.Instance.gridSize, p1.z);
        CreateTriangle(p1,p2,p3);
    }

    void InitializeTriangleMesh()
    {
        triangleMesh = new();
        meshCollider = GetComponent<MeshCollider>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void GeneGhostTri()
    {
        ghostTriGOInstance = Instantiate(ghostTriGO, transform.position, quaternion.identity);
        ghostTriGOInstance.GetComponent<MeshRenderer>().material = ghostTriMat;
        ghostTriGOInstance.GetComponent<LineRenderer>().enabled = false;
        ghostTriGOInstance.SetActive(false);
    }

    public void ShowGhostTri(Vector3 pos)
    {
        ghostTriGOInstance.SetActive(true);
        ghostTriGOInstance.transform.position = pos;
        isPivoted = true;
    }
    
    public void HideGhostTri()
    {
        ghostTriGOInstance.SetActive(false);
        isPivoted = false;
    }

    public void Set2PivotPos(Grid _pivotedGrid)
    {
        if (isPivoted)
        {
            HideGhostTri();
            var pos = ghostTriGOInstance.transform.position;
            pos.z = transform.position.z;
            transform.position=pos;
            pivotedGrid = _pivotedGrid;
        }
    }

    public void Rotate(Vector3 dragPos,bool isRight = true)
    {
        dir=(TriDir)(((int)dir + (isRight ? -1 : 1))%4);
        float deg = isRight ? -90 : 90;
        dragPos.z = transform.position.z;
        Vector3 startVec = dragPos - transform.position;
        Vector3 afterRotateVec = Quaternion.AngleAxis(deg, Vector3.forward) * startVec;
        transform.Rotate(Vector3.forward,deg);
        rotateOffset += (Vector2)(startVec - afterRotateVec);
        ghostTriGOInstance.transform.Rotate(Vector3.forward,deg);
    }

    public void DeleteTri()
    {
        Destroy(ghostTriGOInstance);
        Destroy(gameObject);
    }

    public void UpdateLineRenderer()
    {
        Vector3[] positions = new Vector3[4];
        for (int i=0;i<positions.Length-1;i++)
            positions[i] = transform.position+transform.rotation * vertices[i];
        positions[3] = positions[0];
        _lineRenderer.SetPositions(positions);
    }
    
    private void Awake()
    {
       InitializeTriangleMesh();
    }

    private void Start()
    {
       //CreateTriangle(e1,angle,e2);
       GeneGhostTri();
    }

    private void Update()
    {
        UpdateLineRenderer();
    }

    private void OnValidate()
    {
        InitializeTriangleMesh();
        switch (triRepresentMethod)
        {
            case TriRepresentMethod.SAS:
                CreateTriangle(e1,angle1,e2);
                break;
            case TriRepresentMethod.SASA:
                CreateTriangle(e1,angle1,e2,angle2);
                break;
        }
        UpdateLineRenderer();
    }
}
