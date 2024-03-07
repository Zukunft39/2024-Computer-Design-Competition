using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    Transform playerTransform;
    Transform cameraTransform;
    Animator animator;
    CharacterController characterController;
    public enum PlayerState{
        Crouch,
        Stand,
        Midair
    }
    [HideInInspector]
    public PlayerState playerState = PlayerState.Stand;

/// <summary>
/// animator 状态机对应切换状态的阈值
/// </summary>
    float crouchThreshold = 0f;
    float standThresshold = 1f;
    float midairThreshold = 2f;
    public enum LocomotionState{
        Idle,
        Walk,
        Run
    }
    [HideInInspector]
    public LocomotionState locomotionState = LocomotionState.Idle;
    public enum ActionState{
        Normal,
        Gaming
    }
    [HideInInspector]
    public ActionState actionState = ActionState.Normal;

    float crouchSpeed = 1.5f;
    float walkSpeed = 2.5f;
    float runSpeed = 5.5f;

    Vector2 moveInput;// 输入Input System
    Vector3 playerMovement = Vector3.zero;// 玩家实际移动
    bool isRunning = false;
    bool isCrouching = false;
    bool isJumping = false;
    bool isGaming = false;

/// <summary>
/// animator 动画状态对应哈希值
/// </summary>
    int postureHash;
    int moveSpeedHash;
    int turnSpeedHash;
    void Start()
    {
        playerTransform = transform; // 提高运行效率
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        postureHash = Animator.StringToHash("Status");
        moveSpeedHash = Animator.StringToHash("Move Speed");
        turnSpeedHash = Animator.StringToHash("Turn Speed");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateInputDirection();
        SwitchPlayerState();
        SetupAnimator();
    }

    #region 读取玩家输入
    public void GetMoveInput(InputAction.CallbackContext ctx){
        moveInput = ctx.ReadValue<Vector2>();
    }
    public void GetRunInput(InputAction.CallbackContext ctx){
        isRunning = ctx.ReadValueAsButton();
    }
    public void GetCrouchInput(InputAction.CallbackContext ctx){
        isCrouching = ctx.ReadValueAsButton();
    }
    #endregion
    void SwitchPlayerState(){
        if(isCrouching){
            playerState = PlayerState.Crouch;
        }
        else{
            playerState = PlayerState.Stand;
        }

        if(moveInput.magnitude == 0){
            locomotionState = LocomotionState.Idle;
        }
        else if(!isRunning){
            locomotionState = LocomotionState.Walk;
        }
        else{
            locomotionState = LocomotionState.Run;
        }

        if(isGaming){
            actionState = ActionState.Gaming;
        }
        else{
            actionState = ActionState.Normal;
        }
    }
    
    /// <summary>
    /// 计算玩家位置相对相机的移动
    /// </summary>
    void CalculateInputDirection(){
        Vector3 camForwardProjection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        playerMovement = camForwardProjection * moveInput.y + cameraTransform.right * moveInput.x;
        playerMovement = playerTransform.InverseTransformVector(playerMovement);
    }

    void SetupAnimator(){
        //计算玩家移动速度
        if(playerState == PlayerState.Stand){
            animator.SetFloat(postureHash, standThresshold,0.1f,Time.deltaTime);
            switch(locomotionState){
                case LocomotionState.Idle:
                    animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        else if(playerState == PlayerState.Crouch){
            animator.SetFloat(postureHash, crouchThreshold, 0.1f, Time.deltaTime);
            switch(locomotionState){
                case LocomotionState.Idle:
                    animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        //计算玩家转向速度
        if(actionState == ActionState.Normal){
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
            playerTransform.Rotate(0,rad * 200 * Time.deltaTime,0);//转向速度慢，人为添加转向速度
        }
    }
    /// <summary>
    /// 使用Character Controller接管动画 控制玩家移动
    /// </summary>
    private void OnAnimatorMove() {
        characterController.Move(animator.deltaPosition);
    }
}
