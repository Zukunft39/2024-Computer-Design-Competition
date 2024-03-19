using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CageData", menuName = "GamesData")]
public class CageGameState : ScriptableObject
{
    private bool isOver = false;
    public void Over()
    {
        isOver = true;
    }
}
public enum PanelState
{
    Instruction,
    Pause,
    Lose,
    Win,
    None
}
public class CageManager : MonoBehaviour
{
    [SerializeField] private bool isBegin = false;
    [SerializeField] private bool isPause = false;
    [Tooltip("头")]
    [SerializeField] private int head = 0;
    [Tooltip("脚")]
    [SerializeField] private int foot = 0;
    [Space]
    [Header("当前数据")]
    [Tooltip("兔子")]
    [SerializeField] private int currentRabbit = 0;
    [Tooltip("鸡")]
    [SerializeField] private int currentChicken = 0;
    [Space]
    private float timer;
    private float tempTimer;
    private int rabbit = 0;
    private int chicken = 0;
    [Header("UI面板")]
    [SerializeField] private GameObject[] panels;//0、1、2、3对应说明、暂停、失败、成功
    [Space]

    public Text timerText;
    public Text headText;
    public Text footText;
    [Space]
    [Tooltip("兔子")]public Text currentRText;
    [Tooltip("鸡")]public Text currentCText;
    [Space]
    [Header("轻重切换")]
    public Transform lightIma;
    public Transform heavyIma;
    public Transform arrowIma;

    ObjectPooler pooler;
    CageGameState state;
    GameObject currentPanel;
    PanelState panelState = new PanelState();

    private void Awake()
    {
        isBegin = false;
        //Init();
        //对象池启动
        pooler = ObjectPooler.Instance;
        //Initiall();
    }
    private void Start()
    {
        //说明UI显示
        panelState = PanelState.Instruction;
        PanelChange(panelState.ToString());
        PanelDisplay();
    }
    private void Update()
    {
        #region 计时器
        if (isBegin && timerText != null && !isPause)
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
        timer = 61f;
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
        if (currentChicken == chicken && currentRabbit == rabbit && timer > 0)
        {
            //UI显示
            WinGame();

            isBegin = false;

            if (state != null) state.Over();
        }
        else if(timer <= 0 && isBegin)
        {
            LoseGame();

            isBegin = false;
        }
    }
    private void InstantiateObjC()
    {
        if(!isPause) pooler.GetSpawnObj("Chicken");
    }
    private void InstantiateObjR()
    {
        if(!isPause) pooler.GetSpawnObj("Rabbit");
    }
    //UI面板切换
    private GameObject PanelChange(string panelState)
    {
        switch (panelState)
        {
            case "Instruction":
                currentPanel = panels[0];
                break;
            case "Pause":
                currentPanel = panels[1];
                break;
            case "Lose":
                currentPanel = panels[2];
                break;
            case "Win":
                currentPanel = panels[3];
                break;
            case "None":
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError("No Panel:" + panelState);
#endif
                break;
        }
        if (currentPanel != null)
        {
            return currentPanel;
        }
        Debug.LogWarning("CurrentPanel is Null!");
        return null;
    }
    //面板显示
    public void PanelDisplay()
    {
        if (currentPanel != null && !currentPanel.activeSelf)
        {
            
            isPause = true;
            currentPanel.SetActive(true);
        }
#if UNITY_EDITOR
        else if (currentPanel == null) Debug.LogError("No Panel!");
        else if (currentPanel.activeSelf) Debug.LogWarning("Panel(" + panelState + ") is exist!");
#endif
    }
    //失败
    private void LoseGame()
    {
        panelState = PanelState.Lose;
        PanelChange(panelState.ToString());
        PanelDisplay();
    }
    //成功
    private void WinGame()
    {
        panelState = PanelState.Win;
        PanelChange(panelState.ToString());
        PanelDisplay();
    }
    //暂停
    public void PauseGame()
    {
        panelState = PanelState.Pause;
        PanelChange(panelState.ToString());
        PanelDisplay();
    }
    //初始化包装
    public void Initiall()
    {
        Init();
        #region 生成
        InvokeRepeating(nameof(InstantiateObjC), 0f, 5f);
        InvokeRepeating(nameof(InstantiateObjR), 0f, 2.5f);
        #endregion
        currentPanel.SetActive(false);
        isPause = false;
        isBegin = true;
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
    public void Exit()
    {
#if UNITY_EDITOR
        Debug.Log("退出");
#endif
        panelState = PanelState.None;
        //跳转
    }
    public void Continue()
    {
#if UNITY_EDITOR
        Debug.Log("继续");
#endif
        currentPanel.SetActive(false);
        isPause = false;
    }
    public void Restrat()
    {
#if UNITY_EDITOR
        Debug.Log("重来");
#endif
        currentPanel.SetActive(false);
    }
}
