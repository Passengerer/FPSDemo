using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_ObjectPool : MonoBehaviour {

    public GameObject gameObj;

    private List<GameObject> list;

    private void Awake()
    {
        list = new List<GameObject>();
    }

    public GameObject Get()
    {
        GameObject ret;
        if (list.Count > 0)
        {
            ret = list[0];
            list.Remove(ret);
        }
        else
        {
            ret = Instantiate(gameObj);
        }
        ret.SetActive(true);
        return ret;
    }

    public void Put(GameObject obj)
    {
        list.Add(obj);
        obj.SetActive(false);
    }
}
