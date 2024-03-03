using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbacusManager : MonoBehaviour
{
    [Tooltip("通过次数")]
    [SerializeField] private int count = 0;

    [Space]

    [Tooltip("结果")]
    [SerializeField] private int result = 0;

    [Space]
    [Tooltip("是否开始")]
    [SerializeField] private bool isBegin = false;

    int target = 0;//真实结果
    float time = 0;//经过时间

    public const float latestTime = 15f;
    [Tooltip("计时UI显示")] public Text timerText;
    [Tooltip("算式UI显示")] public List<Text> texts;
    private void Awake()
    {
        time = latestTime;
        texts = new List<Text>();
    }
    private void Update()
    {
        //计时器
        if (isBegin)
        {
            time -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = time.ToString();
            }
#if UNITY_EDITOR
            else Debug.LogError("No TimerText in UI");
#endif
        }
        else time = latestTime;

        //进行算盘的具体操作

        //判断是否继续进行游戏
        if (count < 3)
        {
            AddSubCal();
        }
        else Win();
    }
    //出现算式
    private void AddSubCal()
    {
        int nums1 = Random.Range(0, 100);
        int nums2 = Random.Range(0, 100);

        bool isAdd = Mathf.FloorToInt(Random.value * 1.99f) == 0;//判断是加或减

#if UNITY_EDITOR
        if (texts.Count <= 0) Debug.LogError("No Calculation UI");
#endif

        //场景表示

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
    private void Judge()
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

#if UNITY_EDITOR
        Debug.Log("真实结果：" + target + " ，" + "经过时间：" + time);
#endif
    }
    //关于胜利
    private void Win()
    {
        //UI显示成功界面
    }
    //关于失败
    private void Lose()
    {
        //UI显示失败界面
    }
}
