using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour {

    [Tooltip("淡入淡出速度")]
    public float fadeSpeed;

    private bool sceneStarting = true;
    private RawImage image;

    private void Start()
    {
        image = this.GetComponent<RawImage>();
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
    }

    private void Update()
    {
        if (sceneStarting)
            StartScene();
    }
    // 将UI设置成透明
    private void FadeToClear()
    {
        image.color = Color.Lerp(image.color, Color.clear, fadeSpeed * Time.deltaTime);
    }
    // 将UI设置为不透明
    private void FadeToBlack()
    {
        image.color = Color.Lerp(image.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    private void StartScene()
    {
        FadeToClear();
        if (image.color.a <= 0.05f)
        {
            image.color = Color.clear;
            image.enabled = false;
            sceneStarting = false;
        }
    }

    public void EndScene()
    {
        FadeToBlack();
        if (image.color.a >= 0.95f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Demo");
        }
    }
}
