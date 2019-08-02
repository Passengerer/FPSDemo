using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_KeyPickUp : MonoBehaviour {

    public int keyId;
    public AudioClip keyGrabAudio;

    private GameObject player;
    private fps_PlayerInventory inventory;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        inventory = player.GetComponent<fps_PlayerInventory>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            AudioSource.PlayClipAtPoint(keyGrabAudio, this.transform.position);
            inventory.AddKey(keyId);
            Destroy(this.gameObject);
        }
    }
}
