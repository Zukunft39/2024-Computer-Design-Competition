using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Networking;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;
    [Header("AI")]
    [SerializeField] private InputField questionInputField;
    [SerializeField] private string deployId;
    [SerializeField] private string universalAIExtraPrompt;
    private string AIPrompt;

    [SerializeField] private float typingSpeed = 0.1f; // 每个字出现的速度

    private List<DialogueString> dialogueList;

    [Header("Player")]
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private Animator animator;
    [SerializeField] private CinemachineVirtualCamera dialogueCamera;
    [SerializeField] private AudioSource playerFootsteps;

    public int currentDialogueIndex = 0;//当前读取的对话序列
    private bool optionSelected = false;//选项被选中判定
    
    private void Start() {
        dialogueParent.SetActive(false);
        playerFootsteps = GameObject.FindWithTag("AudioManager").GetComponent<MainAudioManager>().sfxSource.GetComponent<AudioSource>();
    }
    /// <summary>
    /// 对话初始化设置
    /// </summary>
    /// <param name="textToPrint"> 对话详细设置内容 </param>
    /// <param name="NPC"> 摄像机转向的移动目标 </param>
    public void DialogueStart(List<DialogueString> textToPrint,Transform NPC,Transform Cam,String m_AIPrompt = "")
    {
        AIPrompt = m_AIPrompt;
        
        dialogueParent.SetActive(true);

        thirdPersonController.enabled = false;
        animator.SetFloat("Status",1f);
        animator.SetFloat("Move Speed",0f);
        animator.SetFloat("Turn Speed",0f);
        if(playerFootsteps) playerFootsteps.enabled = false;

        MainEventManager.Instance.ShowCursor();
        
        TurnCameraTowardsNPC(Cam);

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        DisableButtons();
        RotateToPlayer(NPC);
        StartCoroutine(PrintDialogue());

    }

    /// <summary>
    /// 禁用按钮
    /// </summary>
    private void DisableButtons(){
        if(option1Button){
            option1Button.interactable = false;//这里我们可以写成取消显示Button
            option1Button.gameObject.SetActive(false);
            option1Button.GetComponentInChildren<TMP_Text>().text = "No Option";
        }
        if(option2Button){
            option2Button.interactable = false;   
            option2Button.gameObject.SetActive(false);
            option2Button.GetComponentInChildren<TMP_Text>().text = "No Option";
        }  
    }
    private void RotateToPlayer(Transform NPC){
        
        Vector3 directionToNPC = NPC.position - transform.position;
        Vector3 directionToPlayer = transform.position - NPC.position;

        directionToNPC.y = 0;//忽略在y轴上的偏差
        directionToPlayer.y = 0;

        transform.rotation = Quaternion.LookRotation(directionToNPC);
        Debug.Log("物体名称" + transform.name);
        if(NPC.tag == "Girl"){
            Quaternion quaternionToPlayer = Quaternion.LookRotation(directionToPlayer);
            Quaternion additionalRotation = Quaternion.Euler(0, 55f, 0);
            NPC.rotation = quaternionToPlayer * additionalRotation;
        }
        else if(NPC.tag == "Untagged"){
            NPC.rotation = Quaternion.LookRotation(directionToPlayer);
        }
        else return; 
    }
    /// <summary>
    /// 旋转相机到NPC的位置
    /// </summary>
    /// <param name="NPC"> 相机旋转到目标的位置 </param>
    private void TurnCameraTowardsNPC(Transform NPC){
        dialogueCamera.LookAt = NPC;
        dialogueCamera.Priority = 100;
    }
    /// <summary>
    /// 打印输入对话文字
    /// </summary>
    /// <returns></returns>
    private IEnumerator PrintDialogue(){
        while(currentDialogueIndex < dialogueList.Count){
            DialogueString line = dialogueList[currentDialogueIndex];
            Debug.Log("跳转到第"+currentDialogueIndex+"个对话");
            line.startDialogueEvent?.Invoke();//启用调用当前处理事件
            if (line.isAIDialog)
            {
                yield return StartCoroutine(AIDialogCoroutine(questionInputField.text));
            }else
            {
                if (line.isQuestion)
                {
                    yield return StartCoroutine(TypeText(line.text));

                    option1Button.gameObject.SetActive(true);
                    option2Button.gameObject.SetActive(true);
                    option1Button.interactable = true;
                    option2Button.interactable = true;

                    option1Button.GetComponentInChildren<TMP_Text>().text = line.answerOption1;
                    option2Button.GetComponentInChildren<TMP_Text>().text = line.answerOption2;

                    option1Button.onClick.RemoveAllListeners();
                    option2Button.onClick.RemoveAllListeners();
                    option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1IndexJump));
                    option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2IndexJump));

                    yield return new WaitUntil(() => optionSelected); //直到当前状态为optionSelected时才推出
                }
                else
                {
                    yield return StartCoroutine(TypeText(line.text));
                }
            }

            line.endDialougueEvent?.Invoke();

            optionSelected = false;
        }
        DialogueStop();
    }
    
    //AI Dialog coroutine
    private IEnumerator AIDialogCoroutine(string questionText)
    {
        questionInputField.gameObject.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        questionInputField.gameObject.SetActive(false);
        questionInputField.text = "";
        string requestText = AIPrompt+" " + universalAIExtraPrompt+" "+questionText;
        string url = $"https://script.google.com/macros/s/{deployId}/exec?question=" + UnityWebRequest.EscapeURL(requestText);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return StartCoroutine(TypeText("正在思考中...",false));
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            yield return StartCoroutine(TypeText($"看来你问的问题因为{request.error}太高深了，我也不是很懂"));
        }
        else
        {
            string answerRawText = request.downloadHandler.text;
            AIAnswer answerClass = JsonUtility.FromJson<AIAnswer>(answerRawText);
            bool isEnd = answerClass.answer.Contains("结束对话");
            if(isEnd )
            {
                answerClass.answer = answerClass.answer.Replace("结束对话","");
            }
            Debug.Log(answerRawText+" "+answerClass.answer);
            yield return StartCoroutine(TypeText(answerClass.answer));
            if (!isEnd)
            { 
                dialogueText.text = "";
               yield return StartCoroutine(AIDialogCoroutine(questionText));
            }
        }
    }
  
    
    /// <summary>
    /// 跳转到对应的对话
    /// </summary>
    /// <param name="indexJump">跳转到得到对应目标</param>
    private void HandleOptionSelected(int indexJump){
        optionSelected = true;
        DisableButtons();

        currentDialogueIndex = indexJump;
    }
    
    /// <summary>
    /// 打印文字
    /// </summary>
    /// <param name="text"> Dialoguelist 中的文字 </param>
    /// <returns></returns>
    private IEnumerator TypeText(string text,bool isWaitForInput = true){
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
        playerFootsteps.enabled = true;

        MainEventManager.Instance.HideCursor();

        dialogueCamera.Priority = 0;
        dialogueCamera.LookAt = null;
        Debug.Log("对话结束");
    }
}
class AIAnswer
{
    public string answer;            
}