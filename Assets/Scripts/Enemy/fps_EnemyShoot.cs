using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_EnemyShoot : MonoBehaviour {

    public float maxDamage = 70;
    public float minDamage = 40;
    public AudioClip shotClip;
    public float flashIntensity = 3;
    public float fadeSpeed = 10;

    private LineRenderer laserShotLine;
    private Animator anim;
    private HashIDs hash;
    private Light laserShotLight;
    private Transform playerTF;
    private fps_PlayerHealth playerHealth;
    private fps_EnemySight sight;
    private bool shooting;
    private float scaledDamage;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        laserShotLine = GetComponentInChildren<LineRenderer>();
        laserShotLight = laserShotLine.GetComponent<Light>();
        playerTF = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealth = playerTF.GetComponent<fps_PlayerHealth>();
        sight = GetComponent<fps_EnemySight>();

        laserShotLine.enabled = false;
        laserShotLight.intensity = 0;

        scaledDamage = maxDamage - minDamage;
	}
	
	// Update is called once per frame
	void Update () {
        float shot = anim.GetFloat(hash.shotFloat);
        if (shot > 0.05f && !shooting)
        {
            Shoot();
        }
        else if (shot < 0.05f)
        {
            shooting = false;
            laserShotLine.enabled = false;
        }
        laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0, fadeSpeed * Time.deltaTime);
	}

    private void OnAnimatorIK(int layerIndex)
    {
        float aimWeight = anim.GetFloat(hash.aimWeightFloat);
        anim.SetIKPosition(AvatarIKGoal.RightHand, playerTF.position + Vector3.up * 1.9f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
    }

    private void Shoot()
    {
        shooting = true;
        float fractionalDistance = 1 - Vector3.Distance(transform.position, playerTF.position) / sight.distance;
        float damage = scaledDamage * fractionalDistance + minDamage;

        playerHealth.TakeDamage(damage);
        ShotEffcts();
    }

    private void ShotEffcts()
    {
        laserShotLine.SetPosition(0, laserShotLine.transform.position);
        laserShotLine.SetPosition(1, playerTF.position + Vector3.up * 1.9f);
        laserShotLine.enabled = true;

        laserShotLight.intensity = flashIntensity;
        AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);
    }
}
