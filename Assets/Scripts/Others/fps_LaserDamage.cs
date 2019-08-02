using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_LaserDamage : MonoBehaviour {

    public float damage = 30;
    public float damageDelay = 1;

    private float lastDamageTime;
    private fps_PlayerHealth Player;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerHealth>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == Player.gameObject && Time.time > lastDamageTime + damageDelay)
        {
            Player.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}
