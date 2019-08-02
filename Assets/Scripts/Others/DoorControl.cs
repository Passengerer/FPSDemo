using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorControl : MonoBehaviour {

    public int doorId;
    public Vector3 slideFrom;
    public Vector3 slideTo;
    public float fadeSpeed = 5;
    public bool requireKey;
    public AudioClip doorSwitchClip;
    public AudioClip accessDeniedClip;

    private Transform door;
    private GameObject player;
    private fps_PlayerInventory inventory;
    private AudioSource audioSource;
    private int count = 0;      // 触发器内人数

    public int Count
    {
        get { return count; }
        set
        {
            if (count == 0 && value == 1 || count == 1 && value == 0)
            {
                audioSource.clip = doorSwitchClip;
                audioSource.Play();
            }
            count = value;
        }
    }

    // Use this for initialization
    void Start () {
        if (transform.childCount > 0)
            door = transform.GetChild(0);
        player = GameObject.FindGameObjectWithTag(Tags.player);
        inventory = player.GetComponent<fps_PlayerInventory>();
        audioSource = this.GetComponent<AudioSource>();
        door.localPosition = slideFrom;
    }

    private void Update()
    {
        if (Count > 0)
        {
            door.localPosition = Vector3.Lerp(door.localPosition, slideTo, fadeSpeed * Time.deltaTime);
            if (Vector3.Distance(door.localPosition, slideTo) < 0.05f)
            {
                door.localPosition = slideTo;
            }
        }
        else
        {
            door.localPosition = Vector3.Lerp(door.localPosition, slideFrom, fadeSpeed * Time.deltaTime);
            if (Vector3.Distance(door.localPosition, slideFrom) < 0.05f)
            {
                door.localPosition = slideFrom;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.player)){
            if (requireKey)
            {
                if (inventory.HasKey(doorId))
                    ++Count;
                else
                {
                    audioSource.clip = accessDeniedClip;
                    audioSource.Play();
                }
            }
            else ++Count;
        }
        else if (other.CompareTag(Tags.enemy) && other is CapsuleCollider)
        {
            ++Count;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.player))
        {
            if (requireKey && !inventory.HasKey(doorId))
                return;
            Count = Mathf.Max(0, Count - 1);
        }
        else if (other.CompareTag(Tags.enemy) && other is CapsuleCollider)
        {
            Count = Mathf.Max(0, Count - 1);
        }
    }
}
