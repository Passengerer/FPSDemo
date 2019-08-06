using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fps_EnemyAI : MonoBehaviour {

    public float petrolSpeed = 2;
    public float chaseSpeed = 5;
    public float chaseWaitTime = 5;
    public float petrolWaitTime = 1;
    public Transform[] wayPoints;

    private fps_EnemySight sight;
    private NavMeshAgent nav;
    private Transform playerTF;
    private fps_PlayerHealth playerHealth;
    private float chaseTimer;
    private float petrolTimer;
    private int wayPointIndex;

	// Use this for initialization
	void Start () {
        sight = GetComponent<fps_EnemySight>();
        nav = GetComponent<NavMeshAgent>();
        playerTF = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealth = playerTF.GetComponent<fps_PlayerHealth>();
	}
	
	// Update is called once per frame
	void Update () {
        if (sight.playerInSight && playerHealth.hp > 0)
            Shooting();
        else if (sight.playerPosition != sight.resetPosition && playerHealth.hp > 0)
            Chasing();
        else
            Petrolling();
	}

    private void Shooting()
    {
        nav.SetDestination(transform.position);
    }

    private void Chasing()
    {
        Vector3 sightingDeltaPos = sight.playerPosition - transform.position;

        if (sightingDeltaPos.sqrMagnitude > 4)
        {
            nav.SetDestination(sight.playerPosition);
        }
        nav.speed = chaseSpeed;

        if (nav.remainingDistance < nav.stoppingDistance)
        {
            chaseTimer += Time.deltaTime;

            if (chaseTimer >= chaseWaitTime)
            {
                sight.playerPosition = sight.resetPosition;
                chaseTimer = 0;
            }
        }
        else
            chaseTimer = 0;
    }

    private void Petrolling()
    {
        nav.speed = petrolSpeed;

        if (nav.destination == sight.resetPosition || nav.remainingDistance < nav.stoppingDistance)
        {
            petrolTimer += Time.deltaTime;
            if (petrolTimer >= petrolWaitTime)
            {
                petrolTimer = 0;
                wayPointIndex = (wayPointIndex + 1) % wayPoints.Length;
            }
        }
        else
            petrolTimer = 0;

        nav.SetDestination(wayPoints[wayPointIndex].position);
    }
}
