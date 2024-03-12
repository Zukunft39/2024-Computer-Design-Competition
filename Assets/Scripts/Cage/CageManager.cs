using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CageGameState : ScriptableObject
{
    private bool isOver = false;
    public void Over()
    {
        isOver = true;
    }
}
public class CageManager : MonoBehaviour
{
    [SerializeField] private bool isBegin = false;
    [Tooltip("头")]
    [SerializeField] private int head;
    [Tooltip("脚")]
    [SerializeField] private int foot;
    [Space]
    [Tooltip("当前数据")]
    [SerializeField] private int currentRabbit = 0;
    [SerializeField] private int currentChicken = 0;
    private float timer;
    private int rabbit = 0;
    private int chicken = 0;

    public Text timerText;
    public Text headText;
    public Text footText;
    [Tooltip("兔子")]public Text currentRText;
    [Tooltip("鸡")]public Text currentCText;

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        #region 计时器
        if (isBegin && timerText != null)
        {
            timer -= Time.deltaTime;
            timerText.text = ((int)timer).ToString() + "s";
        }
#if UNITY_EDITOR
        else if (timerText == null) Debug.LogError("No TimerText!");
#endif
        #endregion
        #region 检查
        Check();
        #endregion
    }
    //初始化
    private void Init()
    {
        timer = 60f;
        timerText.text = "60s";

        //feet>=2head并且feet是偶数
        head = Random.Range(1, 15);
        int temp = Mathf.FloorToInt(Random.value * 2.99f);
        if (temp == 0) foot = 2 * Random.Range(head, 15);
        else foot = 2 * Random.Range(head, 20);
        if (foot > 4 * head) foot = 4 * head;
        headText.text = head.ToString();
        footText.text = foot.ToString();

        rabbit = foot / 2 - head;
        chicken = head - rabbit;
    }
    private void Check()
    {
        if(currentChicken == chicken && currentRabbit == rabbit)
        {
            //UI显示

            isBegin = false;
        }
    }
    public void AddNum(string tag)
    {
        //type=1->rabbit, type=0->chicken
        switch (tag)
        {
            case "Rabbit":
                currentChicken++;
                currentCText.text = currentChicken.ToString();
                break;
            case "Chicken":
                currentRabbit++;
                currentRText.text = currentRabbit.ToString();
                break;
        }
    }
}
