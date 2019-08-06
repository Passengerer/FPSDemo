using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fps_EnemySight : MonoBehaviour {

    public float fieldOfViewAngle = 110;
    public float distance = 25;
    public bool playerInSight;
    public Vector3 resetPosition = Vector3.zero;
    public Vector3 playerPosition;

    private GameObject player;
    private fps_PlayerHealth playerHealth;
    private fps_PlayerControl playerControl;
    private Animator anim;
    private HashIDs hash;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerHealth = player.GetComponent<fps_PlayerHealth>();
        playerControl = player.GetComponent<fps_PlayerControl>();
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        fps_GunScript.PlayerShootEvent += ListernPlayer;
    }

    private void Update()
    {
        UpdatePlayerInfo();

        if (playerHealth.hp > 0)
        {
            anim.SetBool(hash.playerInSightBool, playerInSight);
        }
        else
            anim.SetBool(hash.playerInSightBool, false);
    }

    private void UpdatePlayerInfo()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < distance)
        {
            // 判断是否在视角内
            Vector3 direction = player.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            
            if (angle < fieldOfViewAngle * 0.5f)
            {
                // 判断是否有障碍物
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, 
                    direction.normalized, out hit))
                {
                    if (hit.collider.CompareTag(Tags.player))
                    {
                        playerInSight = true;
                        playerPosition = player.transform.position;
                        return;
                    }
                }
            }
            // 判断是否发出声音
            if (playerControl.State == PlayerState.Walk || playerControl.State == PlayerState.Run)
            {
                playerPosition = player.transform.position;
            }
        }
        playerInSight = false;
    }
    // 判断声音是否在距离内
    private void ListernPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < distance)
            playerPosition = player.transform.position;
    }

    private void OnDestroy()
    {
        fps_GunScript.PlayerShootEvent -= ListernPlayer;
    }
}
