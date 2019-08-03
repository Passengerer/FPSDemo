using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_DestroySelf : MonoBehaviour {

    public float timeToDestroy = 5;

    private float timer = 0;
	
	// Update is called once per frame
	void Update () {
        if (timer < timeToDestroy)
            timer += Time.deltaTime;
        else
            Destroy(gameObject);
	}
}
