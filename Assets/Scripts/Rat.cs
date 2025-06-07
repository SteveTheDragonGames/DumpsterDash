using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Rat : MonoBehaviour
{

    public float speed = 5f;

    SpriteRenderer sr = null;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            speed *= -1;
            sr.flipX = !sr.flipX;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Boundary"))
        {
            RatElectrocution zap = GetComponent<RatElectrocution>();
            if (zap != null)
                zap.TriggerZappyDeath();
            else UnityEngine.Debug.LogWarning("RatElectrocution Component missing!");
        }

    }
}
