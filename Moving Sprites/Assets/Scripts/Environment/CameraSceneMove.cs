using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraSceneMove : MonoBehaviour
{
    public Transform playerLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3((float) (9 + Math.Floor(playerLocation.position.x / 18) * 18), (float) (5 + Math.Floor(playerLocation.position.y / 10) * 10), transform.position.z);
    }
}
