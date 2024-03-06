using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    Transform playerTransform;
    Animator animator;

    public enum PlayerState{
        Crouch,
        Stand,
        Midair
    }
    public PlayerState playerState = PlayerState.Stand;

    float crouchThreshold = 0f;
    float standThresshold = 1f;
    float midairThreshold = 2f;
    public enum LocomotionState{
        Idle,
        Walk,
        Run
    }
    public LocomotionState locomotionState = LocomotionState.Idle;
    public enum ActionState{
        Normal,
        Gaming
    }
    public ActionState actionState = ActionState.Normal;

    float crouchSpeed = 1.5f;
    float walkSpeed = 2.5f;
    float runSpeed = 5.5f;

    Vector2 moveInput;
    bool isRunning;
    bool isCrouching;
    bool isJumping;
    bool isGaming;

    int postureHash;
    int moveSpeedHash;
    int turnSpeedHash;
    void Start()
    {
        playerTransform = transform; // 提高运行效率
        animator = GetComponent<Animator>();

        postureHash = Animator.StringToHash("Status");
        moveSpeedHash = Animator.StringToHash("Move Speed");
        turnSpeedHash = Animator.StringToHash("Turn Speed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region InputAction
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
}
