using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_Crosshair : MonoBehaviour {

    public float width;
    public float normalLength;
    public float normalDistance;
    public Texture2D crosshairTexture;

    private Texture tex;
    private GUIStyle lineStyle;
    private bool firing = false;
    public float length;
    public float distance;
    private float currentLength;
    private float currentDistance;
    public float firingLength;
    public float firingDistance;
    public float crouchingLength;
    public float crouchingDistance;

    private fps_PlayerControl playerControl;

    private void Start()
    {
        playerControl = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerControl>();
        lineStyle = new GUIStyle();
        lineStyle.normal.background = crosshairTexture;
        currentLength = normalLength;
        currentDistance = normalDistance;
        firingLength = normalLength * 2;
        firingDistance = normalDistance * 2;
        crouchingLength = normalLength / 2;
        crouchingDistance = normalDistance / 2;
    }

    private void Update()
    {
        UpdateState(playerControl.State);

        if (Mathf.Abs(currentDistance - distance) < 0.05f)
            currentDistance = distance;
        else
            currentDistance = Mathf.Lerp(currentDistance, distance, 0.1f);
        if (Mathf.Abs(currentLength - length) < 0.05f)
            currentLength = length;
        else
            currentLength = Mathf.Lerp(currentLength, length, 0.1f);
    }

    private void OnGUI()
    {
        GUI.Box(new Rect((Screen.width - currentDistance) / 2 - currentLength, 
            (Screen.height - width) / 2, currentLength, width), tex, lineStyle);
        GUI.Box(new Rect((Screen.width + currentDistance) / 2,
            (Screen.height - width) / 2, currentLength, width), tex, lineStyle);
        GUI.Box(new Rect((Screen.width - width) / 2,
            (Screen.height + currentDistance) / 2, width, currentLength), tex, lineStyle);
        GUI.Box(new Rect((Screen.width - width) / 2, 
            (Screen.height - currentDistance) / 2 - currentLength, width, currentLength), 
            tex, lineStyle);
    }

    // 缩放准星
    public void ZoomCrosshair(bool isFiring)
    {
        if (!firing && isFiring)
        {
            normalLength *= 3;
            normalDistance *= 3;
            firingLength *= 3;
            firingDistance *= 3;
            crouchingLength *= 3;
            crouchingDistance *= 3;
            firing = true;
        }
        else if (firing && !isFiring)
        {
            normalLength /= 3;
            normalDistance /= 3;
            firingLength /= 3;
            firingDistance /= 3;
            crouchingLength /= 3;
            crouchingDistance /= 3;
            firing = false;
        }
    }

    private void UpdateState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                length = normalLength;
                distance = normalDistance;
                break;
            case PlayerState.Crouch:
                length = crouchingLength;
                distance = crouchingDistance;
                break;
            case PlayerState.Walk:
                length = firingLength;
                distance = firingDistance;
                break;
            case PlayerState.Run:
                length = firingLength;
                distance = firingDistance;
                break;
        }
    }
}
