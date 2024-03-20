using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;

    [SerializeField] private float typingSpeed = 0.05f; // 每个字出现的速度
    [SerializeField] private float turnSpeed = 2f;

    private List<DialogueString> dialogueList;

    [Header("Player")]
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private Animator animator;
    // todo 这里切换到Cinemachine, 使用Frame Transposer实现固定相机轨道
    private Transform playerCamera;

    public int currentDialogueIndex = 0;//当前读取的对话序列
    private void Start() {
        dialogueParent.SetActive(false);
        playerCamera = Camera.main.transform;
    }

    /// <summary>
    /// 对话初始化设置
    /// </summary>
    /// <param name="textToPrint"> 对话详细设置内容 </param>
    /// <param name="NPC"> 摄像机转向的移动目标 </param>
    public void DialogueStart(List<DialogueString> textToPrint,Transform NPC){
        Debug.Log("开始对话");
        dialogueParent.SetActive(true);
        thirdPersonController.enabled = false;
        animator.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("已解锁鼠标位置并弹出对话界面");
        StartCoroutine(TurnCameraTowardsNPC(NPC));

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        DisableButtons();
        StartCoroutine(PrintDialogue());
    }

    /// <summary>
    /// 禁用按钮
    /// </summary>
    private void DisableButtons(){
        Debug.Log("禁用按钮");
        option1Button.interactable = false;//这里我们可以写成取消显示Button
        option2Button.interactable = false;

        option1Button.GetComponentInChildren<TMP_Text>().text = "No Option";
        option2Button.GetComponentInChildren<TMP_Text>().text = "No Option";
    }

    /// <summary>
    /// 旋转相机到NPC的位置
    /// </summary>
    /// <param name="NPC"> 相机旋转到目标的位置 </param>
    /// <returns> 返回一个协程 </returns>
    // todo 这里修改使用Cinemachine
    private IEnumerator TurnCameraTowardsNPC(Transform NPC){
        Quaternion startRotation = playerCamera.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(NPC.position - playerCamera.position);

        float elapsedTime = 0;
        while (elapsedTime < 1f){
            playerCamera.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime + turnSpeed;
            yield return null;
        }
        playerCamera.rotation = targetRotation;
        Debug.Log("相机旋转到NPC位置完成");
    }

    private bool optionSelected = false;
    /// <summary>
    /// 打印输入对话文字
    /// </summary>
    /// <returns></returns>
    private IEnumerator PrintDialogue(){
        while(currentDialogueIndex < dialogueList.Count){
            DialogueString line = dialogueList[currentDialogueIndex];

            line.startDialogueEvent?.Invoke();//启用调用当前处理事件

            if (line.isQuestion)
            {
                yield return StartCoroutine(TypeText(line.text));

                Debug.Log("当前为问句");

                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<TMP_Text>().text = line.answerOption1;
                option2Button.GetComponentInChildren<TMP_Text>().text = line.answerOption2;

                option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1IndexJump));
                option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2IndexJump));

                yield return new WaitUntil(() => optionSelected);
            }
            else{
                yield return StartCoroutine(TypeText(line.text));
            }

            line.endDialougueEvent?.Invoke();

            optionSelected = false;
        }
        DialogueStop();
    }
    /// <summary>
    /// 跳转到对应的对话
    /// </summary>
    /// <param name="indexJump">跳转到得到对应目标</param>
    private void HandleOptionSelected(int indexJump){
        Debug.Log("选项被选中了");
        optionSelected = false;
        DisableButtons();

        currentDialogueIndex = indexJump;
    }
    
    /// <summary>
    /// 打印文字
    /// </summary>
    /// <param name="text"> Dialoguelist 中的文字 </param>
    /// <returns></returns>
    private IEnumerator TypeText(string text){
        Debug.Log("正在打字");
        dialogueText.text = "";
        foreach(char letter in text.ToCharArray()){
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if(!dialogueList[currentDialogueIndex].isQuestion){
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        if(dialogueList[currentDialogueIndex].isEnd){
            DialogueStop();
        }

        currentDialogueIndex ++;
    }

    /// <summary>
    /// 停止对话返回原场景
    /// </summary>
    private void DialogueStop(){
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);

        thirdPersonController.enabled = true;
        animator.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("对话结束");
    }
}
