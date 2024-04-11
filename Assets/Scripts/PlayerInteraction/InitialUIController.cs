using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InitialUIController : MonoBehaviourSingleton<InitialUIController>
{
    public GameObject instructionPanel;
    public GameObject blackPanel;
    public GameObject instruction;
    public Animator animator;

    //退出游戏
    private void Awake(){
        DontDestroyOnLoad(this);
    }
    private void Start(){
        animator = blackPanel.GetComponent<Animator>();
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void Begin()
    {
        //堆屎山了，动画机不敢改，先只能这样
        ShitToFadePanel();
        MainUIController.mainUIControllerInstance.ShowBlackStartPanel();
    }
    public void Instruct()
    {
        instructionPanel.SetActive(true);
        instruction.SetActive(true);
        blackPanel.SetActive(true);
    }
    public void CloseInstruct()
    {
        instruction.SetActive(false);
        StartCoroutine(Black());
    }
    /// <summary>
    /// 严重的狮山，但是不敢改动画机只能这么做,不过可以优化一下这里的算法，晚上没脑子想了
    /// </summary>
    public void ShitToFadePanel(){
        transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFade(0, 2).OnComplete(()=>this.transform.GetChild(1).GetChild(0).gameObject.SetActive(false));
        transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(2).GetChild(0).GetChild(2).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(0).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(1).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(2).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(3).GetComponent<Image>().DOFade(0, 2);
    }
    public void ShitToShowPanel(){
        transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFade(1, 2).OnComplete(()=>this.transform.GetChild(1).GetChild(0).gameObject.SetActive(true));
        transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().DOFade(1, 2);
        transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>().DOFade(1, 2);
        transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<Image>().DOFade(1, 2);
        transform.GetChild(2).GetChild(0).GetChild(2).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(0).GetComponent<Image>().DOFade(1, 2);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(1).GetComponent<Image>().DOFade(1, 2);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(2).GetComponent<Image>().DOFade(1, 2);
        transform.GetChild(2).GetChild(0).GetChild(3).GetChild(3).GetComponent<Image>().DOFade(1, 2);
    }
    public void HideCanvas(){
        this.gameObject.SetActive(false);
    }
    IEnumerator Black()
    {
        animator.SetTrigger("Black2");
        yield return new WaitForSeconds(1);
        instructionPanel.SetActive(false);
    }
}
