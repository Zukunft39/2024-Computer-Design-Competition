using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    //三角形边长
    public float e1,e2,angle1,angle2;
    //提示三角形材质
    public Material ghostTriMat;
    //提示三角形预制体
    public GameObject ghostTriGO;
    //旋转偏移
    public Vector2 rotateOffset;
    //锚定格子
    public Grid pivotedGrid;
    //三角形方向
    public TriDir dir=TriDir.LEFT;
    //三角形类型
    public int triType;
    //三角形数学表示方法
    public TriRepresentMethod triRepresentMethod;
    //是否在OnValidate时启用
    public bool enableOnValidate = true;
    
    public enum TriDir
    {
        LEFT,UP,RIGHT,DOWN
    }
    
    public enum TriRepresentMethod
    {
        //两边夹一角
        SAS,
        //极坐标确定其余两顶点相对坐标
        SASA
    }
    
    //三角形网格
    private Mesh triangleMesh;
    //三角形顶点
    private Vector3[] vertices;
    //网格中的三角形定义
    private int[] triangles;
    //三角形UV
    private Vector2[] uvs;
    //三角形碰撞体
    private MeshCollider meshCollider;
    //提示三角形实例
    private GameObject ghostTriGOInstance;
    //是否已锚定
    private bool isPivoted;
    //边框线渲染器
    private LineRenderer _lineRenderer;

    /// <summary>
    /// 根据顶点坐标生成三角形
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public void CreateTriangle(Vector3 p1,Vector3 p2,Vector3 p3)
    {
       // if (GouguData.Instance is null) return;
        vertices=new[]{p1,p2,p3};
        triangles=new[]{0,1,2};
        uvs=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(0,1)};
        triangleMesh.vertices=vertices;
        triangleMesh.triangles=triangles;
        triangleMesh.uv=uvs;
        triangleMesh.normals=new[]{-Vector3.forward,-Vector3.forward,-Vector3.forward};
        GetComponent<MeshFilter>().mesh=triangleMesh;
        _lineRenderer.positionCount = 4;
    }
    
    /// <summary>
    /// 根据SAS法则生成三角形
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
    
    /// <summary>
    /// 根据极坐标确定其余二顶点，生成三角形
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e1Angle"></param>
    /// <param name="e2"></param>
    /// <param name="e2Angle"></param>
    public void CreateTriangle(float e1,float e1Angle,float e2,float e2Angle)
    {
        Vector3 p1 =Vector3.zero;
        Vector3 p2 = new Vector3(p1.x+Mathf.Cos(Mathf.Deg2Rad* e1Angle)*e1*GouguData.Instance.gridSize, p1.y+Mathf.Sin(Mathf.Deg2Rad*e1Angle)*e1*GouguData.Instance.gridSize, p1.z);
        Vector3 p3 = new Vector3(p1.x+Mathf.Cos(Mathf.Deg2Rad* e2Angle)*e2*GouguData.Instance.gridSize, p1.y+Mathf.Sin(Mathf.Deg2Rad*e2Angle)*e2*GouguData.Instance.gridSize, p1.z);
        CreateTriangle(p1,p2,p3);
    }

    // 初始化三角形网格
    void InitializeTriangleMesh()
    {
        triangleMesh = new();
        meshCollider = GetComponent<MeshCollider>();
        _lineRenderer = GetComponent<LineRenderer>();
    }
    
    // 初始化提示三角形
    void GeneGhostTri()
    {
        ghostTriGOInstance = Instantiate(ghostTriGO, transform.position, quaternion.identity);
        ghostTriGOInstance.GetComponent<MeshRenderer>().material = ghostTriMat;
        ghostTriGOInstance.GetComponent<LineRenderer>().enabled = false;
        ghostTriGOInstance.SetActive(false);
    }
    
    /// <summary>
    ///  显示提示三角形
    /// </summary>
    /// <param name="pos"></param>
    public void ShowGhostTri(Vector3 pos)
    {
        ghostTriGOInstance.SetActive(true);
        ghostTriGOInstance.transform.position = pos;
        isPivoted = true;
    }
    
    /// <summary>
    ///  隐藏提示三角形
    /// </summary>
    public void HideGhostTri()
    {
        ghostTriGOInstance.SetActive(false);
        isPivoted = false;
    }

    /// <summary>
    /// 设置到锚定位置
    /// </summary>
    /// <param name="_pivotedGrid"></param>
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

    /// <summary>
    ///  旋转三角形
    /// </summary>
    /// <param name="dragPos"></param>
    /// <param name="isRight"></param>
    public void Rotate(Vector3 dragPos,bool isRight = true)
    {
        dir=(TriDir)((((byte)dir + (isRight ? -1 : 1)) & 3));
        float deg = isRight ? -90 : 90;
        dragPos.z = transform.position.z;
        Vector3 startVec = dragPos - transform.position;
        Vector3 afterRotateVec = Quaternion.AngleAxis(deg, Vector3.forward) * startVec;
        transform.Rotate(Vector3.forward,deg);
        rotateOffset += (Vector2)(startVec - afterRotateVec);
        ghostTriGOInstance.transform.Rotate(Vector3.forward,deg);
    }

    /// <summary>
    ///  删除三角形
    /// </summary>
    public void DeleteTri()
    {
        Destroy(ghostTriGOInstance);
        Destroy(gameObject);
    }
    
    /// <summary>
    ///  更新边框线渲染器
    /// </summary>
    public void UpdateLineRenderer()
    {
        Vector3[] positions = new Vector3[4];
        for (int i = 0; i < positions.Length - 1; i++)
            try
            {
                positions[i] = vertices[i];
            }
            catch {}
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
        if(!enableOnValidate)return;
        //在编辑器scene窗口显示三角形mesh
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
