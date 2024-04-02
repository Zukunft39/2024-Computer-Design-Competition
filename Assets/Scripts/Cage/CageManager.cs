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
    //private bool isAnim = false;//判断动画是否开始播放
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
    private int rabbit = 0;
    private int chicken = 0;
    [Header("UI面板")]
    [SerializeField] private GameObject[] panels;//0、1、2、3、4对应说明、暂停、失败、成功、黑幕
    [Space]
    [SerializeField] private Transform[] transforms;//0-Chicken，1-Rabbit，2-cage
    [Space]
    [Header("笼子数据")]
    [SerializeField] private GameObject cage;
    [Tooltip("笼子横向半径")][SerializeField] private float cageOffestX;
    [Tooltip("笼子竖向半径")][SerializeField] private float cageOffestY;
    [Tooltip("笼子点位置信息")][SerializeField] private int[] poses;//0代表没有占用，1代表占用
    private int currentNum;//确定当前列表里第一个未被占有的数据
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
    [Header("UI动画信息")]
    public float topOffest;
    public float animDuration;

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

        //黑幕
        if(currentPanel.activeSelf) panels[4].SetActive(true);
        else
        {
            StartCoroutine(Black());
        }
        #endregion
        //if (Input.GetKey(KeyCode.Alpha1)) LoseGame();
    }

    #region 条件判断与初始化
    //初始化
    private void Init()
    {
        timer = 61f;
        timerText.text = "60s";
        currentChicken = 0;
        currentRabbit = 0;

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

        currentNum = 0;
        poses = new int[28];
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

    //初始化包装
    public void Initiall()
    {
        Init();
        #region 生成
        InvokeRepeating(nameof(InstantiateObjC), 0f, 5f);
        InvokeRepeating(nameof(InstantiateObjR), 2.5f, 4f);
        #endregion
        currentPanel.SetActive(false);
        isPause = false;
        isBegin = true;
    }
    #endregion

    #region UI相关信息
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
            Anim();
        }
#if UNITY_EDITOR
        else if (currentPanel == null) Debug.LogError("No Panel!");
        else if (currentPanel.activeSelf) Debug.LogWarning("Panel(" + panelState + ") is not exist!");
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
        Time.timeScale = 0;
        panelState = PanelState.Pause;
        PanelChange(panelState.ToString());
        PanelDisplay();
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
        Time.timeScale = 1;
#if UNITY_EDITOR
        Debug.Log("继续");
#endif
        currentPanel.SetActive(false);
        isPause = false;
    }
    public void Restart()
    {
#if UNITY_EDITOR
        Debug.Log("重来");
#endif
        Clear(transforms[0]);
        Clear(transforms[1]);
        Clear(transforms[2]);
        isPause = false;
        currentPanel.SetActive(false);
        Init();
    }
    #endregion

    #region 其他
    //获取数据
    public bool GetPause()
    {
        return isPause;
    }
    //清空
    private void Clear(Transform trs)
    {
        int count = trs.childCount;
        if(count > 0)
        {
            for(int i = 0; i < count; i++)
            {
                if (trs.GetChild(i).gameObject.activeSelf)
                {
                    pooler.Recover(trs.GetChild(i).gameObject, trs.GetChild(i).gameObject.tag);
                }
            }
        }
    }
    //动画播放
    private void Anim()
    {
#if UNITY_EDITOR
        if(currentPanel == null || !currentPanel.GetComponent<RectTransform>())
        {
            Debug.LogError("No Panel!");
            return;
        }
#endif
        RectTransform panelTransform = currentPanel.GetComponent<RectTransform>();
        Vector2 start = new Vector2(panelTransform.anchoredPosition.x, topOffest);
        Vector2 end = new Vector2(panelTransform.anchoredPosition.x, 0);
        StartCoroutine(AnimSlide(start, end, panelTransform));

    }
    IEnumerator AnimSlide(Vector2 start, Vector2 end, RectTransform transform)
    {
        float startTime = Time.time;
        while (Time.time < startTime + animDuration)
        {
            float t = (Time.time - startTime) / animDuration;
            Vector2 temp = Vector2.Lerp(start, end, t);
            transform.anchoredPosition = temp;

            yield return null;
        }
        transform.anchoredPosition = end;
    }
    IEnumerator Black()
    {
        Color color = panels[4].transform.GetChild(0).GetComponent<Image>().color;
        Animator animator = panels[4].GetComponent<Animator>();
        animator.SetTrigger("Black");
        yield return new WaitForSeconds(1);
        panels[4].SetActive(false);
        panels[4].transform.GetChild(0).GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1f);
    }
    //笼子抓取
    public void Catch(string tag)
    {
        if (tag == "Chicken")
        {
            pooler.GetSpawnObj("ChickenHead", FindPos(currentNum));
        }
        else if (tag == "Rabbit")
        {
            pooler.GetSpawnObj("RabbitHead", FindPos(currentNum));
        }
#if UNITY_EDITOR
        else Debug.LogError("No this tag(" + tag + ")!");
#endif
    }
    //寻找抓取后位置
    private Vector3 FindPos(int i)
    {
        if (poses[i] == 0)
        {
            poses[i] = 1;
            int tempX = i % 5;
            int tempY = (int)i / 5;
            Vector3 target = new Vector3(cageOffestX * (tempX - 2) / 2, cageOffestY * (tempY - 1), 0f);
#if UNITY_EDITOR
            if (cageOffestX == 0 || cageOffestY == 0) Debug.LogError("The cage info is null!");
            Debug.Log("Target Pos:" + target);
#endif
            return target;
        }
        i++;
        return FindPos(i);
    }
    #endregion
}
