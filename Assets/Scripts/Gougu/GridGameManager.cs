using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class GridGameManager : MonoBehaviourSingleton<GridGameManager>
{
    //生成三角形按钮
    public Button geneTriBtn;
    //主相机
    public Camera mainCam;
    //拖动平滑因子
    public float smoothFactor;
    //三角形预制体
    public GameObject triangleGO;
    //三角形生成点
    public Transform genePoint;
    //标识
    public GameObject dummy;
    //三角形最大锚定距离
    public float maxPivotDistance;
    //过关事件
    public Action approvedAct;
    //引导线
    public GameObject[] guideLines;
    //三角形生成范围
    public float generateRange = 2;
    //关卡名文本
    public Text levelNameTxt;
    //三角形生成音效音量
    [Range(0,1f)]
    public float placeSfxVolume = 0.5f;
    //三角形放置音效音量
    [Range(0,1f)]
    public float fetchSfxVolume = 0.5f;
    //过关音效音量
    [Range(0,1f)]
    public float approvedSfxVolume = 0.8f;
    
    //是否按下
    private bool clicked;
    //当前三角形
    private Triangle curTriangle;
    //当前三角形坐标
    private (int,int) curTriangleIndex;
    //拖动偏移
    private Vector2 offset;
    //交点
    private Vector3 crossPoint;
    //生成的三角形列表
    private List<Triangle> generatedTriangles;
    //是否游戏中
    private bool isGaming;
    //当前关卡id
    private int cur_id;
    
    /// <summary>
    ///  生成三角形
    /// </summary>
    /// <param name="_triType">三角形类型</param>
    public void GenerateTriangle(int _triType)
    {
        if(!isGaming)return;
        Triangle triangle = Instantiate(triangleGO, genePoint.position + new Vector3(UnityEngine.Random.Range(-generateRange,generateRange),
            UnityEngine.Random.Range(-generateRange,generateRange),0), Quaternion.identity).GetComponent<Triangle>();
        generatedTriangles.Add(triangle);
        //根据三角形类型分布生成对应的三角形
        switch (_triType)
        {
            case 0:
                triangle.CreateTriangle(3,90,4);
                break;
            case 1:
                triangle.CreateTriangle(3,36.87f,5,90);
                break;
        }
        //设置三角形对象属性
        triangle.triType = _triType;
        var pos = triangle.transform.position;
        pos.z = -0.5f;
        triangle.transform.position = pos;
        //播放生成音效
        SoundManager.Instance.PlaySFX(GouguData.Instance.audioDict["FetchWood"],0.15f,fetchSfxVolume);
    }
    
    /// <summary>
    ///  拖动三角形
    /// </summary>
    void DragTri()
    {
        //获取鼠标射线
        Ray alwaysOnRay = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit alwaysOnhit;
        //若碰撞，显示标识
        if (Physics.Raycast(alwaysOnRay, out alwaysOnhit) && alwaysOnhit.collider.CompareTag("Triangle"))
        {
            dummy.SetActive(true);
            dummy.transform.position = alwaysOnhit.transform.position;
        }
        else
        {
            dummy.SetActive(false);
        }
        
        //若未按下鼠标左键，检测是否点击三角形
        if (!clicked&& Input.GetMouseButtonDown(0))
        {
            //获取鼠标射线
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //若碰撞，获取三角形对象
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Triangle"))
            {
                //设置点击状态
                clicked = true;
                //获取三角形对象
                curTriangle = hit.transform.gameObject.GetComponent<Triangle>();
                //重置三角形锚定点
                curTriangle.pivotedGrid = null;
                //计算拖动偏移
                offset = curTriangle.transform.position - hit.point;
            }
        }
    
        //若点击状态，检测是否拖动
        if (clicked && Input.GetMouseButton(0))
        {
            //获取鼠标射线
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //若碰撞，获取交点
            if (Physics.Raycast(ray, out hit,100,LayerMask.GetMask("Ground")) && hit.collider.CompareTag("Ground"))
            {
                //设置三角形位置
                var pos = hit.point;
                pos.z = curTriangle.transform.position.z;
                crossPoint = pos;
                //插值进行平滑拖拽
                curTriangle.transform.position = Vector3.Lerp(pos+(Vector3)offset+ (Vector3)curTriangle.rotateOffset, curTriangle.transform.position, smoothFactor);
                
                //若按下E键，顺时针旋转
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SoundManager.Instance.PlaySFX("cut",0.1f,fetchSfxVolume);
                    curTriangle.Rotate(pos);
                }
                
                //若按下Q键，逆时针旋转
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SoundManager.Instance.PlaySFX("cut",0.1f,fetchSfxVolume);
                    curTriangle.Rotate(pos,false);
                }
                
                //若按下R键，删除三角形
                if (Input.GetKey(KeyCode.R))
                {
                    generatedTriangles.Remove(curTriangle);
                    curTriangle.DeleteTri();
                    curTriangle = null;
                    clicked = false;
                    SoundManager.Instance.PlaySFX("cut",0.1f,fetchSfxVolume);
                    if(Judger.Instance.JudgeIsApproved(generatedTriangles)) approvedAct();
                }
            }
        }
        
         //若松开鼠标左键，放置三角形
        if (clicked && Input.GetMouseButtonUp(0))
        {
            //重置点击状态
            clicked = false;
            //获取三角形锚定点
            Grid pivotedGrid = GridMap.Instance.gridAttrs[curTriangleIndex.Item1, curTriangleIndex.Item2];
            //设置三角形锚定点
            curTriangle.Set2PivotPos(pivotedGrid);
            //重置旋转偏移
            curTriangle.rotateOffset = Vector3.zero;
            //重置当前选中三角形对象
            curTriangle = null;
            //播放放置音效
            SoundManager.Instance.PlaySFX(GouguData.Instance.audioDict["PlaceWood"],0.15f,placeSfxVolume);
            //若通过过关判断，触发过关事件
            if(Judger.Instance.JudgeIsApproved(generatedTriangles)) approvedAct();
        }
    }
    
    /// <summary>
    ///  通过关卡
    /// </summary>
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
        SoundManager.Instance.PlaySFX(cur_id!=GouguData.Instance.levels.Length-1? 
            GouguData.Instance.audioDict[$"Approved{cur_id+1}"]:
            GouguData.Instance.audioDict["Final"],
            volume:approvedSfxVolume,
            isWeakenBGM:true);
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
        //若游戏中，显示提示三角形
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
    
    // 重载异步等待，使gameplay协程在游戏结束时继续执行
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
        //重载keepWaiting属性
        public override bool keepWaiting
        {
            get
            {
                //返回游戏状态
                return GridGameManager.Instance.isGaming;
            }
        }
    }
    
    /// <summary>
    ///  开始游戏
    /// </summary>
    /// <param name="level_id">关卡id</param>
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


