using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_ExitTrigger : MonoBehaviour {

	public float timeToInactivePlayer;
    public float timeToRestart;

    private float timer;
    private GameObject player;
    private FadeInOut fader;
    private fps_PlayerHealth playerHealth;
    private bool playerInExit;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
        playerHealth = player.GetComponent<fps_PlayerHealth>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            timer = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            timer += Time.deltaTime;

            if (playerHealth != null && timer > timeToInactivePlayer)
            {
                playerHealth.DiableInput();
                playerHealth = null;
            }
            if (timer > timeToRestart)
            {
                fader.EndScene();
            }
        }
    }
}
