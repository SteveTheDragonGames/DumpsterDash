using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : MonoBehaviour
{
    private BoxCollider2D col2D;
    public float speed = 5f;

    void Awake()
    {
        col2D = GetComponent<BoxCollider2D>();
        col2D.enabled = false;
        Invoke(nameof(EnableCollider), 0.5f);
    }

    void EnableCollider()
    {
        if (col2D != null)
        {
            col2D.enabled = true;
        }
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public void TurnAround()
    {
        //turn around brighteyes
        speed = -speed;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TurnAround();
        }
    }


}
