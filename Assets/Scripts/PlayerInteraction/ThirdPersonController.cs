using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
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
        //Midair
    }
    [HideInInspector]
    public PlayerState playerState = PlayerState.Stand;

/// <summary>
/// animator 状态机对应切换状态的阈值
/// </summary>
    float crouchThreshold = 0f;
    float standThresshold = 1f;
    //float midairThreshold = 2f;

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
    int statusHash;
    int moveSpeedHash;
    int turnSpeedHash;
    int verticalSpeedHash;

/// <summary>
/// 角色重力手动添加
/// </summary>
    private float gravity = -9.81f;
    private float verticalVelocity = 0.0f;
    //private float jumpVelocity = 5.0f;

/// <summary>
/// 选用目标前几帧的平均值,作为空中水平移动速度进行截取
/// CACHE_FRAME 为选取的固定帧数,velocityCache作为速度的缓冲池,averageVelocity作为平均速度的记录,currentCacheIndex作为池中当前的索引,
/// </summary>
    public static readonly int CACHE_SIZE = 4;
    Vector3[] velocityCache = new Vector3[CACHE_SIZE];
    Vector3 averageVelocity;
    int currentCacheIndex = 0;

/// <summary>
/// 脚步状态的选择
/// </summary>

    void Start()
    {
        playerTransform = transform; // 提高运行效率
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        statusHash = Animator.StringToHash("Status");
        moveSpeedHash = Animator.StringToHash("Move Speed");
        turnSpeedHash = Animator.StringToHash("Turn Speed");
        verticalSpeedHash = Animator.StringToHash("Vertical Speed");
    }

    // Update is called once per frame
    void Update()
    {
        WalkSound();
        CalculateGravity();
        CalculateInputDirection();
        //Jump();
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
    public void GetJumpInput(InputAction.CallbackContext ctx){
        isJumping = ctx.ReadValueAsButton();
    }
    #endregion
    void SwitchPlayerState(){
        /*if(!characterController.isGrounded){
            playerState = PlayerState.Midair;
        }
        else*/ if(isCrouching){
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
        if(playerTransform == null || cameraTransform == null) return;
        Vector3 camForwardProjection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        playerMovement = camForwardProjection * moveInput.y + cameraTransform.right * moveInput.x;
        playerMovement = playerTransform.InverseTransformVector(playerMovement);
    }

    void SetupAnimator(){
        //计算玩家移动速度
        if(playerState == PlayerState.Stand){
            animator.SetFloat(statusHash, standThresshold,0.1f,Time.deltaTime);
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
            animator.SetFloat(statusHash, crouchThreshold, 0.1f, Time.deltaTime);
            switch(locomotionState){
                case LocomotionState.Idle:
                    animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        /*else if(playerState == PlayerState.Midair){
            animator.SetFloat(statusHash, midairThreshold, 0.1f, Time.deltaTime);
            animator.SetFloat(verticalSpeedHash, verticalVelocity, 0.1f, Time.deltaTime);
        }*/
        //计算玩家转向速度
        if(actionState == ActionState.Normal){
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
            playerTransform.Rotate(0,rad * 200 * Time.deltaTime,0);//转向速度慢,人为添加转向速度
        }
    }
    /// <summary>
    /// 计算平均速度并及时清除数组元素
    /// </summary>
    /// <param name="newVelocity"></param>
    /// <returns></returns>
    Vector3 AverageVelocity(Vector3 newVelocity){
        velocityCache[currentCacheIndex] = newVelocity;
        currentCacheIndex++;
        currentCacheIndex %= CACHE_SIZE;
        Vector3 averageVelocity = Vector3.zero;
        foreach(var v in velocityCache){
            averageVelocity += v;
        }
        return averageVelocity / CACHE_SIZE;
    }

    /// <summary>
    /// 使用Character Controller接管动画 控制玩家移动(Character不自带重力需手动添加)
    /// </summary>
    void OnAnimatorMove() {
        //if(playerState != PlayerState.Midair){
            Vector3 playerDeltaMovement = animator.deltaPosition;
            playerDeltaMovement.y = verticalVelocity * Time.deltaTime;
            characterController.Move(playerDeltaMovement);
            averageVelocity = AverageVelocity(animator.velocity);
        //}
        /*else{
            // todo 沿用地面平均速度代替空中的移动速度
            Vector3 playerDeltaMovement = averageVelocity * Time.deltaTime;
            playerDeltaMovement.y = verticalVelocity * Time.deltaTime;
            characterController.Move(playerDeltaMovement);
        }*/
    }
    void CalculateGravity(){
        if(characterController.isGrounded){
            verticalVelocity = gravity * Time.deltaTime;
            return;
        }
        else{
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
    /*void Jump(){
        if(characterController.isGrounded && isJumping){
            verticalVelocity = jumpVelocity;  
        }
    }*/
    
    /// <summary>
    /// 处理人物脚步声
    /// </summary>
    void WalkSound(){
        if(locomotionState == LocomotionState.Walk){
            PlayerFootstepListen.TriggerFootStep(PlayerFootstepListen.SoundType.WalkSound);
        }
        else if(locomotionState == LocomotionState.Run){
            PlayerFootstepListen.TriggerFootStep(PlayerFootstepListen.SoundType.RunSound);
        }
        else if(locomotionState == LocomotionState.Idle){
            PlayerFootstepListen.TriggerFootStep(PlayerFootstepListen.SoundType.NoSound);
        }
    }
}

