﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void PlayerShoot();

public class fps_GunScript : MonoBehaviour {

    public static event PlayerShoot PlayerShootEvent;
    public float fireRate = 0.1f;
    public float reloadTime = 1.5f;
    public float damage = 20;
    public float flashRate = 0.02f;
    public float validDistance = 200;
    public float recoil = 2;   // 后坐力

    // 弹痕和击中场景特效的对象池
    public fps_ObjectPool bulletHolePool;
    public fps_ObjectPool envExplosionEffectPool;

    public GameObject explosion;
    public AudioClip fireAudio;
    public AudioClip reloadAudio;
    public AudioClip damageAudio;
    public AudioClip dryFireAudio;
    public Text bulletText;

    public int bulletCount = 30;
    public int chargerBulletCount = 60;

    private string fireAnim = "Single_Shot";
    private string reloadAnim = "Reload";
    private string walkAnim = "Walk";
    private string runAnim = "Run";
    private string idleAnim = "Idle";

    private Animation anim;
    private MeshRenderer flash;
    private float nextFireTime = 0;
    private int currentBullet;
    private int currentChargerBullet;
    private fps_PlayerParameter parameter;
    private fps_PlayerControl playerControl;
    private fps_Crosshair crosshair;

    // 定义一个玩家层，防止子弹打中自身碰撞器
    private LayerMask exceptPlayerMask;

    // Use this for initialization
    void Start () {
        currentBullet = bulletCount;
        currentChargerBullet = chargerBulletCount;
        bulletText.text = currentBullet + "/" + currentChargerBullet;

        anim = GetComponent<Animation>();
        flash = transform.Find("muzzle_flash").GetComponent<MeshRenderer>();
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerParameter>();
        playerControl = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerControl>();
        crosshair = GetComponentInParent<fps_Crosshair>();

        exceptPlayerMask = 1 << 10;     // 此处第10层为Player层
        exceptPlayerMask = exceptPlayerMask + (1 << 2); // 第2层为Ignore Raycast
        exceptPlayerMask = ~exceptPlayerMask;   // 取反，不检测玩家自身和Ignore Raycast层
    }
	
	// Update is called once per frame
	void Update () {
        if (parameter.inputReload && currentBullet < bulletCount)
            Reload();
        if (parameter.inputFire && !anim.IsPlaying(reloadAnim))
            Fire();
        else if (!anim.IsPlaying(reloadAnim))
            StateAnim(playerControl.State);
	}

    private void ReloadAnim()
    {
        anim.Stop(reloadAnim);
        anim[reloadAnim].speed = anim[reloadAnim].length / reloadTime;
        anim.Rewind(reloadAnim);
        anim.Play(reloadAnim);
    }

    private IEnumerator ReloadFinish()
    {
        yield return new WaitForSeconds(reloadTime);

        if (currentChargerBullet >= bulletCount - currentBullet)
        {
            currentChargerBullet -= (bulletCount - currentBullet);
            currentBullet = bulletCount;
        }
        else
        {
            currentBullet += currentChargerBullet;
            currentChargerBullet = 0;
        }
        bulletText.text = currentBullet + "/" + currentChargerBullet;
    }

    private void Reload()
    {
        if (!anim.IsPlaying(reloadAnim))
        {
            if (currentChargerBullet > 0)
            {
                StartCoroutine("ReloadFinish");
                AudioSource.PlayClipAtPoint(reloadAudio, transform.position);
                ReloadAnim();
            }
        }
    }

    private IEnumerator Flash()
    {
        flash.enabled = true;
        crosshair.ZoomCrosshair(true);
        yield return new WaitForSeconds(flashRate);
        flash.enabled = false;
        crosshair.ZoomCrosshair(false);
    }

    private void Fire()
    {
        if (Time.time > nextFireTime)
        {
            if (currentBullet == 0)
            {
                AudioSource.PlayClipAtPoint(dryFireAudio, transform.position);
            }
            else
            {
                --currentBullet;
                bulletText.text = currentBullet + "/" + currentChargerBullet;
                AudioSource.PlayClipAtPoint(fireAudio, transform.position);

                anim.Rewind(fireAnim);
                anim.Play(fireAnim);
                StartCoroutine("Flash");

                Recoil();

                DamageEnemy();
                if (PlayerShootEvent != null)
                    PlayerShootEvent();
            }
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Recoil()
    {
        // 简易制作的后坐力效果
        float offsetX = Random.Range(-0.2f, 0.2f);
        float offsetY = Random.Range(recoil - recoil / 8, recoil + recoil / 8);
        parameter.inputSmoothLook.x += offsetX;
        parameter.inputSmoothLook.y += offsetY;
    }

    public void DamageEnemy()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        
        if (Physics.Raycast(ray, out hit, validDistance, exceptPlayerMask))
        {
            if (!hit.collider.CompareTag(Tags.enemy))
            {               
                GameObject bulletHole = bulletHolePool.Get();
                bulletHole.transform.position = hit.point + hit.normal * 0.01f;
                bulletHole.transform.rotation = Quaternion.LookRotation(hit.normal);
                bulletHole.transform.SetParent(hit.transform);

                GameObject explosion = envExplosionEffectPool.Get();
                explosion.transform.position = hit.point + hit.normal * 0.01f;
                explosion.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
            else
            {
                Instantiate(explosion, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                AudioSource.PlayClipAtPoint(damageAudio, hit.point);
                hit.collider.GetComponent<fps_EnemyHealth>().TakeDamage(damage);
            }
        }
    }

    private void PlayerStateAnim(string animName)
    {
        if (!anim.IsPlaying(animName))
        {
            anim.Rewind(animName);
            anim.Play(animName);
        }
    }

    private void StateAnim(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                PlayerStateAnim(idleAnim);
                break;
            case PlayerState.Crouch:
                PlayerStateAnim(idleAnim);
                break;
            case PlayerState.Walk:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Run:
                PlayerStateAnim(runAnim);
                break;
        }
    }
}
