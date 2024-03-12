using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbacusManager : MonoBehaviour
{
    [Tooltip("通过次数")]
    [SerializeField] private int count = 0;


    [Space]
    [Tooltip("是否开始")]
    [SerializeField] private bool isBegin = false;

    int target = 0;//真实结果
    float time = 0;//经过时间
    bool isPause = false;//UI开启判断

    public const float latestTime = 16f;
    public static int result = 0;
    public AbacusGameState state;//用于数据传递
    [Tooltip("计时UI显示")] public Text timerText;
    [Tooltip("算式UI显示")] public List<Text> texts;
    [Tooltip("胜利UI")] public GameObject wPanel;
    [Tooltip("失败UI")] public GameObject lPanel;
    [Tooltip("暂停UI")] public GameObject pPanel;
    [Tooltip("帮助UI")] public GameObject hPanel;
    [Tooltip("总UI")] public GameObject tCanvas;

    private void Awake()
    {
        Help();
        AddSubCal();
    }
    private void Update()
    {
        #region 计时器
        if (isBegin && !isPause)
        {
            time -= Time.deltaTime;
            if (timerText != null && texts[4] != null)
            {
                timerText.text = ((int)time).ToString();
                texts[4].text = result.ToString();
            }
#if UNITY_EDITOR
            else if (timerText == null) Debug.LogError("No TimerText in UI");
            else Debug.LogError("No Result in UI");
#endif
            if (time <= 0) Judge();
        }
        else if(!isBegin) time = latestTime;
        #endregion

        #region 进行UI操作
        if (Input.GetKey(KeyCode.Escape)) Pause();
        //else if (Input.GetKey(KeyCode.L)) Lose();
        //else if (Input.GetKey(KeyCode.W)) Win();
        #endregion
    }
    //出现算式
    private void AddSubCal()
    {
        int nums1 = Random.Range(0, 100);
        int nums2 = Random.Range(0, 100);

        bool isAdd = Mathf.FloorToInt(Random.value * 1.99f) == 0;//判断是加或减

#if UNITY_EDITOR
        if (texts.Count <= 0) Debug.LogError("No Calculation UI");
        Debug.Log(texts.Count);
#endif

        //场景初始化
        #region 算珠初始化
        Abacus abacus = FindObjectOfType<Abacus>();
        abacus.BeadClear();
        #endregion
        #region UI初始化
        if (isAdd)
        {
            //场景表示
            texts[0].text = nums1.ToString();
            texts[1].text = "+";
            texts[2].text = nums2.ToString();

            target = nums1 + nums2;
        }
        else
        {
            int temp = nums1;
            nums1 = Mathf.Max(nums1, nums2);
            nums2 = Mathf.Min(temp, nums2);

            //场景表示
            texts[0].text = nums1.ToString();
            texts[1].text = "-";
            texts[2].text = nums2.ToString();

            target = nums1 - nums2;
        }
        texts[3].text = "=";
        #endregion
        time = latestTime;
        result = 0;
        //开始游戏
        isBegin = true;
    }
    //判断是否相等或超时
    private bool isSatisfied()
    {
        if (result == target && time > 0) return true;
        return false;

    }
    //关于最终次数的计算,进行最终结果判断时调用
    public void Judge()
    {
        if (isBegin)
        {
            if (isSatisfied()) count++;
            else
            {
                Lose();
            }
        }
        isBegin = false;

        //清空UI显示
        timerText.text = "";
        foreach (var item in texts) item.text = "";

        //判断是否继续进行游戏
        if (count < 3 && !isBegin && !isPause)
        {
            AddSubCal();
        }
        else if (count == 3) Win();

#if UNITY_EDITOR
        Debug.Log("真实结果：" + target + " ，" + "经过时间：" + time);
#endif
    }
    //关于胜利
    private void Win()
    {
        //传递数据
        if (state != null) state.Count(count);
#if UNITY_EDITOR
        else Debug.LogError("No State Object!");
#endif
        //UI显示成功界面
        wPanel.SetActive(true);
        isPause = true;
    }
    //关于失败
    private void Lose()
    {
        //UI显示失败界面
        lPanel.SetActive(true);
        isPause = true;
    }
    //关于暂停
    private void Pause()
    {
        //UI显示暂停界面
        pPanel.SetActive(true);
        isPause = true;
    }
    //关于重新开始
    public void Restart()
    {
        count = 0;
        AddSubCal();
        lPanel.SetActive(false);
        isPause = false;
    }
    //关于继续
    public void Continue()
    {
        pPanel.SetActive(false);
        isPause = false;
    }
    //关于退出
    public void Exit()
    {
        tCanvas.SetActive(false);
        isPause = false;
    }
    //关于帮助
    public void Help()
    {
        hPanel.SetActive(true);
        isPause = true;
    }
    //关于跳过
    public void Skip()
    {
        hPanel.SetActive(false);
        isPause = false;
    }
}
