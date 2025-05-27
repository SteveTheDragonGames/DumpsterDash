using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Rat : MonoBehaviour
{

    public float speed = 5f;


    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
