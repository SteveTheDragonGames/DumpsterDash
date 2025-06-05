using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float minX, maxX;
    private Transform player;
    private Vector3 tempPos;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }


    void LateUpdate()
    {
        if (!player) return;
        tempPos = transform.position;
        tempPos.x = player.position.x;

        //Clamp the camera so it doesn't go offscreen.
        tempPos.x = Mathf.Clamp(tempPos.x, minX, maxX);
        transform.position = tempPos;
    }
}
