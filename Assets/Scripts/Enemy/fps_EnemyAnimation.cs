using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fps_EnemyAnimation : MonoBehaviour {

    public float deadZone = 5;

    private Transform playerTF;
    private fps_EnemySight sight;
    private NavMeshAgent nav;
    private HashIDs hash;
    private Animator anim;
    private AnimatorSetup animSetup;

    private void Start()
    {
        playerTF = GameObject.FindGameObjectWithTag(Tags.player).transform;
        sight = GetComponent<fps_EnemySight>();
        nav = GetComponent<NavMeshAgent>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        anim = GetComponent<Animator>();
        animSetup = new AnimatorSetup(anim, hash);

        nav.updateRotation = false;
        anim.SetLayerWeight(1, 1f);
        anim.SetLayerWeight(2, 1f);

        deadZone *= Mathf.Deg2Rad;
    }

    private void Update()
    {
        NavAnimSetup();
    }

    private void OnAnimatorMove()
    {
        nav.velocity = anim.deltaPosition / Time.deltaTime;
        transform.rotation = anim.rootRotation;
    }

    private void NavAnimSetup()
    {
        float speed;
        float angle;

        if (sight.playerInSight)
        {
            speed = 0;
            angle = FindAngle(transform.forward, playerTF.position - transform.position, transform.up);
        }
        else
        {
            speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
            angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);

            if (Mathf.Abs(angle) < deadZone)
            {
                transform.LookAt(transform.position + nav.desiredVelocity);
                angle = 0;
            }
        }
        animSetup.SetUp(speed, angle);
    }

    private float FindAngle(Vector3 from, Vector3 to, Vector3 up)
    {
        if (to == Vector3.zero) return 0f;

        float angle = Vector3.Angle(from, to);
        Vector3 normal = Vector3.Cross(from, to);
        angle *= Mathf.Sign(Vector3.Dot(normal, up));
        angle *= Mathf.Deg2Rad;

        return angle;
    }
}
