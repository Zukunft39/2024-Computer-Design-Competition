using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridGameManager : MonoBehaviourSingleton<GridGameManager>
{
    public Button geneTriBtn;
    public Camera mainCam;
    public float smoothFactor;
    public GameObject triangleGO;
    public Transform genePoint;
    public GameObject dummy;
    public float maxPivotDistance;
    public Action approvedAct;
    public GameObject[] guideLines;
    public float generateRange = 2;
    public Text levelNameTxt;
   

    private bool clicked;
    private Triangle curTriangle;
    private (int,int) curTriangleIndex;
    private Vector2 offset;
    private Vector3 crossPoint;
    private List<Triangle> generatedTriangles;
    private bool isGaming;
    private int cur_id;

    public void GenerateTriangle(int _triType)
    {
        if(!isGaming)return;
        Triangle triangle = Instantiate(triangleGO, genePoint.position + new Vector3(UnityEngine.Random.Range(-generateRange,generateRange),
            UnityEngine.Random.Range(-generateRange,generateRange),0), Quaternion.identity).GetComponent<Triangle>();
        generatedTriangles.Add(triangle);
        switch (_triType)
        {
            case 0:
                triangle.CreateTriangle(3,90,4);
                break;
            case 1:
                triangle.CreateTriangle(3,36.87f,5,90);
                break;
        }
        triangle.triType = _triType;
        var pos = triangle.transform.position;
        pos.z = -0.5f;
        triangle.transform.position = pos; }

    void DragTri()
    {
        Ray alwaysOnRay = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit alwaysOnhit;
        if (Physics.Raycast(alwaysOnRay, out alwaysOnhit) && alwaysOnhit.collider.CompareTag("Triangle"))
        {
            dummy.SetActive(true);
            dummy.transform.position = alwaysOnhit.transform.position;
        }
        else
        {
            dummy.SetActive(false);
        }
        
        if (!clicked&& Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Triangle"))
            {
                clicked = true;
                curTriangle = hit.transform.gameObject.GetComponent<Triangle>();
                curTriangle.pivotedGrid = null;
                offset = curTriangle.transform.position - hit.point;
            }
        }

        if (clicked && Input.GetMouseButton(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,100,LayerMask.GetMask("Ground")) && hit.collider.CompareTag("Ground"))
            {
                var pos = hit.point;
                pos.z = curTriangle.transform.position.z;
                crossPoint = pos;
                curTriangle.transform.position = Vector3.Lerp(pos+(Vector3)offset+ (Vector3)curTriangle.rotateOffset, curTriangle.transform.position, smoothFactor);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    curTriangle.Rotate(pos);
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    curTriangle.Rotate(pos,false);
                }

                if (Input.GetKey(KeyCode.R))
                {
                    generatedTriangles.Remove(curTriangle);
                    curTriangle.DeleteTri();
                    curTriangle = null;
                    clicked = false;
                    if(Judger.Instance.JudgeIsApproved(generatedTriangles)) approvedAct();
                }
            }
        }

        if (clicked && Input.GetMouseButtonUp(0))
        {
            clicked = false;
            Grid pivotedGrid = GridMap.Instance.gridAttrs[curTriangleIndex.Item1, curTriangleIndex.Item2];
            curTriangle.Set2PivotPos(pivotedGrid);
            curTriangle.rotateOffset = Vector3.zero;
            curTriangle = null;
            if(Judger.Instance.JudgeIsApproved(generatedTriangles)) approvedAct();
        }
    }

    private void OnApproved()
    {
        isGaming = false;
        guideLines[cur_id]?.SetActive(false);
        if(cur_id!= GouguData.Instance.levels.Length-1) GridMap.Instance.DeconstructGrid();
        foreach (var i in generatedTriangles)
        {
            i.DeleteTri();
        }
        generatedTriangles.Clear();
        curTriangle = null;
        clicked = false;
        dummy.SetActive(false);
    }

    private void Awake()
    {
        // geneTriBtn.onClick.AddListener(()=>GenerateTriangle(0));
        generatedTriangles = new();
        approvedAct += OnApproved;
    }

    private void Update()
    {
       if(isGaming)
       {
           DragTri();
       }
    }

    private void FixedUpdate()
    {
        if(isGaming)
            if (clicked && Input.GetMouseButton(0))
            {
                var findRes = GridMap.Instance.FindNearestGridPivot(curTriangle.transform.position);
                var pos = findRes.Item1;
                curTriangleIndex =(findRes.Item2, findRes.Item3);
                pos.z -= 0.5f;
                curTriangle.ShowGhostTri(pos); 
            }
    }
    
    public class WaitForGameEnds : CustomYieldInstruction
    {
        public WaitForGameEnds(int _id)
        {
            level_id = _id;
            if (!GridGameManager.Instance.isGaming)
            {
                GridGameManager.Instance.StartGridGame(level_id);
            }
        }
        private int level_id;
        public override bool keepWaiting
        {
            get
            {
                return GridGameManager.Instance.isGaming;
            }
        }
    }

    public void StartGridGame(int level_id)
    {
        Judger.Instance.InitializeLevel(level_id);
        guideLines[level_id]?.SetActive(true);
        isGaming = true;
        cur_id = level_id;
        GridMap.Instance.ConstructGrid(GouguData.Instance.levels[level_id].levelData.gridMapSize);
        levelNameTxt.text = GouguData.Instance.levels[level_id].levelName;
    }
}


