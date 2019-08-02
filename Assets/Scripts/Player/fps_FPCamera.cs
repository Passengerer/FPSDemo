using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class fps_FPCamera : MonoBehaviour {

    [Tooltip("鼠标敏感度")]
    public Vector2 mouseLookSensitivity = new Vector2(5, 5);
    [Tooltip("俯角、仰角限定范围")]
    public Vector2 rotationXLimit = new Vector2(-87, 87);
    [Tooltip("相机局部坐标")]
    public Vector3 positionOffset = new Vector3(0, 2, -0.2f);

    // 当前鼠标输入
    private Vector3 currentMouseLook = Vector3.zero;
    // 绕x轴、y轴的角度
    private float x_Angle = 0;
    private float y_Angle = 0;
    private fps_PlayerParameter parameter;
    private Transform m_Transform;

    private void Start()
    {
        parameter = GetComponentInParent<fps_PlayerParameter>();
        m_Transform = transform;
        m_Transform.localPosition = positionOffset;
    }

    private void Update()
    {
        UpdateInput();
    }

    private void LateUpdate()
    {
        // 旋转父物体，只转水平方向
        Quaternion xQuaternion = Quaternion.AngleAxis(y_Angle, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(0, Vector3.right);
        m_Transform.parent.rotation = xQuaternion * yQuaternion;

        yQuaternion = Quaternion.AngleAxis(x_Angle, Vector3.right);
        // 先做水平旋转，再做竖直旋转
        m_Transform.rotation = xQuaternion * yQuaternion;
    }

    private void UpdateInput()
    {
        if (parameter.inputSmoothLook == Vector2.zero)
            return;

        GetMouseLook();
        // 绕x轴旋转角度为鼠标的y轴输入
        x_Angle += currentMouseLook.y;
        // 绕y轴旋转角度为鼠标的x轴输入
        y_Angle += currentMouseLook.x;

        y_Angle = y_Angle > 360 ? y_Angle - 360 : y_Angle;
        y_Angle = y_Angle < -360 ? y_Angle + 360 : y_Angle;

        x_Angle = x_Angle > 360 ? x_Angle - 360 : x_Angle;
        x_Angle = x_Angle < -360 ? x_Angle + 360 : x_Angle;
        // 限制俯仰角
        x_Angle = Mathf.Clamp(x_Angle, rotationXLimit.x, rotationXLimit.y);
    }

    private void GetMouseLook()
    {
        currentMouseLook.x = parameter.inputSmoothLook.x;
        currentMouseLook.y = parameter.inputSmoothLook.y;

        currentMouseLook *= mouseLookSensitivity;
        // 鼠标输入的y值与视角旋转方向相反，故取反
        currentMouseLook.y *= -1;
    }
}
