using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_PlayerInventory : MonoBehaviour {

    private List<int> keyArr;

	// Use this for initialization
	void Start () {
        keyArr = new List<int>();
	}
	
    public void AddKey(int id)
    {
        if (!keyArr.Contains(id))
            keyArr.Add(id);
    }

    public bool HasKey(int id)
    {
        if (keyArr.Contains(id))
            return true;
        return false;
    }
}
