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
    private float tempTimer;
    private int rabbit = 0;
    private int chicken = 0;

    public Text timerText;
    public Text headText;
    public Text footText;
    [Tooltip("兔子")]public Text currentRText;
    [Tooltip("鸡")]public Text currentCText;
    [Tooltip("轻重切换")]
    public Transform lightIma;
    public Transform heavyIma;
    public Transform arrowIma;

    ObjectPooler pooler;

    private void Start()
    {
        Init();
        #region 生成
        InvokeRepeating(nameof(InstantiateObjC), 0f, 5f);
        InvokeRepeating(nameof(InstantiateObjR), 0f, 2.5f);
        #endregion
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

        //对象池启动
        pooler = ObjectPooler.Instance;
    }
    private void Check()
    {
        if(currentChicken == chicken && currentRabbit == rabbit)
        {
            //UI显示

            isBegin = false;
        }
    }
    private void InstantiateObjC()
    {
        pooler.GetSpawnObj("Chicken");
    }
    private void InstantiateObjR()
    {
        pooler.GetSpawnObj("Rabbit");
    }
    public void AddNum(string tag)
    {
        switch (tag)
        {
            case "Chicken":
                currentChicken++;
                currentCText.text = currentChicken.ToString();
                break;
            case "Rabbit":
                currentRabbit++;
                currentRText.text = currentRabbit.ToString();
                break;
        }
    }
}
