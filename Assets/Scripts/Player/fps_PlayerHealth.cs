using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class fps_PlayerHealth : MonoBehaviour {

    public bool isDead;
    public float resetAfterDeathTime = 5;
    public float maxHP = 100;
    public float hp;
    public float recoverSpeed = 10;
    public AudioClip damageClip;
    public AudioClip deathClip;

    private float timer;
    private FadeInOut fader;
    private ColorCorrectionCurves colorCurves;

    private void Start()
    {
        hp = maxHP;
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
        colorCurves = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<ColorCorrectionCurves>();
        BleedBehavior.BloodAmount = 0;
    }

    private void Update()
    {
        if (!isDead)
        {
            hp += recoverSpeed * Time.deltaTime;
            BleedBehavior.BloodAmount = 1 - hp / maxHP;
            if (hp > maxHP)
                hp = maxHP;
        }
        if (hp < 0)
        {
            if (!isDead)
                PlayerDead();
            else
                LevelReset();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        AudioSource.PlayClipAtPoint(damageClip, this.transform.position);
        hp -= damage;
    }

    public void DiableInput()
    {
        transform.Find("FP_Camera/WeaponCamera").gameObject.SetActive(false);
        this.GetComponent<AudioSource>().enabled = false;
        this.GetComponent<fps_FPInput>().enabled = false;
        this.GetComponent<fps_PlayerControl>().enabled = false;
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
            canvas.SetActive(false);
        colorCurves.GetComponent<fps_FPCamera>().enabled = false;
    }

    public void PlayerDead()
    {
        isDead = true;
        DiableInput();
        colorCurves.enabled = true;
        AudioSource.PlayClipAtPoint(deathClip, this.transform.position);
    }

    public void LevelReset()
    {
        timer += Time.deltaTime;
        colorCurves.saturation -= Time.deltaTime / 2;
        colorCurves.saturation = Mathf.Max(0, colorCurves.saturation);
        if (timer > resetAfterDeathTime)
            fader.EndScene();
    }
}
