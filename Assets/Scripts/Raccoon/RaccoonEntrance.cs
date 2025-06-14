using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaccoonEntrance : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private bool hasLanded = false;

    public float standDuration = 0.3f;//third of a second

    public RaccoonAI raccoonAI; //link to actual AI logic script.

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim.Play("POPOUT");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!hasLanded && collision.collider.CompareTag("Ground"))
        {
            hasLanded = true;
            rb.velocity = Vector2.zero; //stop bouncing
            //rb.bodyType = RigidbodyType2D.Kinematic;//optional freeze

            StartCoroutine(HandleRaccoonEntrance());
        }
    }

    IEnumerator HandleRaccoonEntrance()
    {
        anim.Play("IDLE");
        yield return new WaitForSeconds(standDuration);
        anim.Play("ANGRY");
        yield return new WaitForSeconds(0.5f);//However long shakes fist
        anim.Play("IDLE");
        raccoonAI.enabled = true; //Enable AI after intro sequence.
    }
}
