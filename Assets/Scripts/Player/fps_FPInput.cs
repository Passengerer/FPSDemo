using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_FPInput : MonoBehaviour {

    // 锁定光标
	public bool LockCursor
    {
        get { return Cursor.lockState == CursorLockMode.Locked; }
        set
        {
            Cursor.visible = value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private fps_PlayerParameter parameter;

    private void Start()
    {
        LockCursor = true;
        parameter = this.GetComponent<fps_PlayerParameter>();
    }

    private void Update()
    {
        InitialInput();
    }

    private void InitialInput()
    {
        parameter.inputSmoothLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        parameter.inputMoveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        parameter.inputCrouch = fps_Input.AxisToButton("Crouch");
        parameter.inputJump = fps_Input.AxisToButton("Jump");
        parameter.inputSprint = fps_Input.AxisToButton("Sprint");
        parameter.inputFire = fps_Input.AxisToButton("Fire1");
        parameter.inputReload = fps_Input.AxisToButton("Reload");
    }
}
