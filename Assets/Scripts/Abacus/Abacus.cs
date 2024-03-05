using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Abacus : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [Space]
    [Tooltip("误差X")]
    [SerializeField] private int offestX;
    [Tooltip("误差Y")]
    [Space]
    [SerializeField] private int offestY;
    private AbacusBead[,] beads;
    private int row = 5;//5行
    private int col = 3;//3列


    private void Awake()
    {
        #region 算盘初始化
        beads = new AbacusBead[col, row];
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                GameObject obj = Instantiate(prefab);
                obj.transform.SetParent(transform, false);
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2((x - 1) * 2 * offestX, (y - row + 1) * offestY);

                if (y == row - 1)
                {
                    rect.anchoredPosition = new Vector2((x - 1) * 2 * offestX, y * offestY);
                    beads[x, y] = new AbacusBead(rect, true);
                }
                else beads[x, y] = new AbacusBead(rect, false);

                beads[x, y].obj = obj;

#if UNITY_EDITOR
                if (beads[x, y].obj == null) Debug.LogWarning("obj is null:" + x + "&" + y);
                else if (prefab == null) Debug.LogWarning("prefab is null");
#endif
            }
        }
        #endregion
    }
    private void Update()
    {
        //射线检测，算盘得分操作
        if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                List<RaycastResult> raycasts = new List<RaycastResult>();
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                EventSystem.current.RaycastAll(eventData, raycasts);
                foreach(var r in raycasts)
                {
                    GameObject clickObj = r.gameObject;
                    for(int x = 0; x < col; x++)
                    {
                        for(int y = 0; y < row; y++)
                        {
                            if(clickObj == beads[x, y].obj)
                            {
                                if (beads[x, y].GetCount() > 0) BeadClose(x, y, y);
                                else if (beads[x, y].GetCount() == 0) BeadOpen(x, y, row - 2 - y);
                            }
                        }
                    }
                }
            }
        }
    }
    //算珠被启用
    private void BeadOpen(int currentX, int currentY, int nextY)
    {
        //current指被点击的顺序，next指其上还有多少
        if (beads[currentX, currentY].GetCount() == 0)
        {
            AbacusManager.result += beads[currentX, currentY].GetValue() * (int)Mathf.Pow(10, col - 1 - currentX);
#if UNITY_EDITOR
            Debug.Log("beads[" + currentX + "," + currentY + "]" + "value:" + beads[currentX, currentY].GetValue());
#endif
        }
        //上珠操作
        if (currentY < row - 1 && beads[currentX, currentY].GetCount() == 0)
        {
            beads[currentX, currentY].DragBead(new Vector2(0, (row - 3) * offestY));
            beads[currentX, currentY].SetCount(1);
            nextY--;
            if (currentY < row - 2) currentY++;
            if (nextY >= 0) BeadOpen(currentX, currentY, nextY);
        }
        //下珠操作
        else if (currentY == row - 1)
        {
            beads[currentX, currentY].DragBead(new Vector2(0, -1 * offestY));
            beads[currentX, currentY].SetCount(1);
        }
    }
    //算珠被停用
    private void BeadClose(int currentX, int currentY, int nextY)
    {
        //同上
        if (beads[currentX,currentY].GetCount() == 1)
            AbacusManager.result -= beads[currentX, currentY].GetValue() * (int)Mathf.Pow(10, col - 1 - currentX);
        //上珠操作
        if (currentY < row - 1 && beads[currentX, currentY].GetCount() == 1)
        {
            beads[currentX, currentY].ReturnBead();
            beads[currentX, currentY].SetCount(-1);
            nextY--;
            if (currentY > 0) currentY--;
            if (nextY > 0) BeadClose(currentX, currentY, nextY);
        }
        //下珠操作
        else if(currentY == row - 1)
        {
            beads[currentX, currentY].ReturnBead();
            beads[currentX, currentY].SetCount(-1);
        }
    }
    //算珠初始化（用于被调用）
    public void BeadClear()
    {
        foreach (var bead in beads)
        {
            bead.ReturnBead();
            if (bead.GetCount() == 1) bead.SetCount(-1);
        }
    }
}