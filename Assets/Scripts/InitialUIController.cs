using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialUIController : MonoBehaviour
{
    public GameObject instructionPanel;
    public GameObject blackPanel;
    public GameObject instruction;

    Animator animator;

    private void Start()
    {
        animator = blackPanel.GetComponent<Animator>();
    }
    //退出游戏
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    //开始游戏
    public void Begin()
    {

    }
    //制作人界面展示
    public void Instruct()
    {
        instructionPanel.SetActive(true);
        instruction.SetActive(true);
        blackPanel.SetActive(true);
    }
    //制作人界面展示关闭
    public void CloseInstruct()
    {
        instruction.SetActive(false);
        StartCoroutine(Black());
    }
    //黑幕
    IEnumerator Black()
    {
        animator.SetTrigger("Black2");
        yield return new WaitForSeconds(1);
        instructionPanel.SetActive(false);
    }
}
