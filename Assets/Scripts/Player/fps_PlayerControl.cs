using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    None,
    Idle,
    Crouch,
    Walk,
    Run,
}

[RequireComponent(typeof(AudioSource))]
public class fps_PlayerControl : MonoBehaviour {

    private PlayerState state;
    public PlayerState State
    {
        get
        {
            if (crouching)
                state = PlayerState.Crouch;
            else if (walking)
                state = PlayerState.Walk;
            else if (running)
                state = PlayerState.Run;
            else
                state = PlayerState.Idle;
            return state;
        }
    }

    public float crouchSpeed = 2;
    public float crouchJumpSpeed = 5;
    public float normalSpeed = 6;
    public float normalJumpSpeed = 7;
    public float sprintSpeed = 10;
    public float sprintJumpSpeed = 8;

    public float crouchDeltaHeight = 0.5f;  // 蹲下时相机偏移高度
    public float cameraMoveSpeed = 8;
    public float gravity = 20;
    public AudioClip jumpClip;              // 跳跃音频

    private Transform mainCamera;
    private float standardCamHeight;        // 正常相机高度
    private float crouchingCamHeight;

    private float speed;
    private float jumpSpeed;

    // 状态
    private bool crouching = false;
    private bool walking = false;
    private bool running = false;
    private bool grounded = false;
    private Vector3 moveDirection = Vector3.zero;

    private Vector3 normalControllerCenter = Vector3.zero;  // CharacterController中心位置
    private float normalControllerHeight = 0;   // CharacterController高度

    private AudioSource audioSource;
    private CharacterController controller;
    private fps_PlayerParameter parameter;

    private void Start()
    {
        speed = normalSpeed;
        jumpSpeed = normalJumpSpeed;

        mainCamera = GameObject.FindGameObjectWithTag(Tags.mainCamera).transform;
        standardCamHeight = mainCamera.localPosition.y;
        crouchingCamHeight = standardCamHeight - crouchDeltaHeight;

        audioSource = this.GetComponent<AudioSource>();
        controller = this.GetComponent<CharacterController>();
        parameter = this.GetComponent<fps_PlayerParameter>();

        normalControllerCenter = controller.center;
        normalControllerHeight = controller.height;
    }

    private void Update()
    {
        UpdateMove();
        AudioManagement();
    }

    private void UpdateMove()
    {
        if (grounded)
        {
            // 获取玩家输入的方向
            moveDirection = new Vector3(parameter.inputMoveVector.x, 0, parameter.inputMoveVector.y);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.Normalize();
            moveDirection *= speed;

            if (parameter.inputJump)
            {
                moveDirection.y = jumpSpeed;
                AudioSource.PlayClipAtPoint(jumpClip, this.transform.position);
                CurrentSpeed();
            }
        }
        // 每帧y移动距离
        moveDirection.y -= gravity * Time.deltaTime;
        // 移动并检测是否着地
        CollisionFlags flag = controller.Move(moveDirection * Time.deltaTime);
        grounded = (flag & CollisionFlags.Below) != 0;

        if (grounded && (Mathf.Abs(moveDirection.x) > 0 || Mathf.Abs(moveDirection.z) > 0))
        {
            // 着地且有方向输入
            if (parameter.inputSprint)
            {
                running = true;
                walking = false;
                crouching = false;
            }
            else if (parameter.inputCrouch)
            {
                running = false;
                walking = false;
                crouching = true;
            }
            else
            {
                running = false;
                walking = true;
                crouching = false;
            }
        }
        else
        {
            // 不着地或无方向输入
            if (walking) walking = false;
            if (running) running = false;
            if (parameter.inputCrouch)
            {
                crouching = true;
            }
            else
                crouching = false;
        }

        if (crouching)
        {
            // 改变碰撞器
            controller.height = normalControllerHeight - crouchDeltaHeight;
            controller.center = normalControllerCenter - new Vector3(0, crouchDeltaHeight / 2.0f, 0);
        }
        else
        {
            controller.height = normalControllerHeight;
            controller.center = normalControllerCenter;
        }
        UpdateCrouch();
        CurrentSpeed();
    }

    private void CurrentSpeed()
    {
        switch (State)
        {
            case PlayerState.Idle:

            case PlayerState.Walk:
                speed = normalSpeed;
                jumpSpeed = normalJumpSpeed;
                break;

            case PlayerState.Crouch:
                speed = crouchSpeed;
                jumpSpeed = crouchJumpSpeed;
                break;

            case PlayerState.Run:
                speed = sprintSpeed;
                jumpSpeed = sprintJumpSpeed;
                break;
        }
    }

    private void AudioManagement()
    {
        if (State == PlayerState.Walk || State == PlayerState.Run)
        {
            audioSource.pitch = State == PlayerState.Walk ? 1.0f : 1.3f;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
            audioSource.Stop();
    }

    private void UpdateCrouch()
    {
        if (crouching)
        {
            if(mainCamera.localPosition.y > crouchingCamHeight)
            {
                // 降低相机高度
                if (mainCamera.localPosition.y - cameraMoveSpeed * Time.deltaTime < crouchingCamHeight)
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, crouchingCamHeight, mainCamera.localPosition.z);
                else
                    mainCamera.Translate(0, -cameraMoveSpeed * Time.deltaTime, 0, Space.World);
            }
        }
        else
        {
            if (mainCamera.localPosition.y < standardCamHeight)
            {
                if (mainCamera.localPosition.y + cameraMoveSpeed * Time.deltaTime > standardCamHeight)
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, standardCamHeight, mainCamera.localPosition.z);
                else
                    mainCamera.Translate(0, cameraMoveSpeed * Time.deltaTime, 0, Space.World);
            }
        }
    }
}
