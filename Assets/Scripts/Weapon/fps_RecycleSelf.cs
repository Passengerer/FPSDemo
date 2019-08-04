using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_RecycleSelf : MonoBehaviour {

    public float recycleTime = 5;
    public string poolName;

    private fps_ObjectPool pool;
    private float timer = 0;

    private void Start()
    {
        pool = GameObject.Find(poolName).GetComponent<fps_ObjectPool>();
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        { 
            if (timer < recycleTime)
                timer += Time.deltaTime;
            else
            {
                timer = 0;
                pool.Put(gameObject);
            }
        }
    }
}
