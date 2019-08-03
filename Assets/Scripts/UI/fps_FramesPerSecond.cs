using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fps_FramesPerSecond : MonoBehaviour {

    public float intervalDisplayTime = 0.5f;

    private float timer;
    private Text txt;

    private void Start()
    {
        timer = intervalDisplayTime;
        txt = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
		if (timer - Time.deltaTime > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = intervalDisplayTime;
            txt.text = Mathf.Floor(1 / Time.deltaTime) + " FPS";
        }
	}
}
