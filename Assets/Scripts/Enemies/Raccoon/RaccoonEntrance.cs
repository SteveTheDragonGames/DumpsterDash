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

    private Transform player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim.Play("POPOUT");
    }

    void Start()
    {

        StartCoroutine(nameof(WaitASecond));

        Vector2 spawnPosition = this.transform.position;
        Vector2 playerPosition = player.transform.position;

        //determine direction
        if (playerPosition.x < spawnPosition.x)
        {
            //player is to the right, face Right.   
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            //player is to the left, face Left.    
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    IEnumerator WaitASecond()
    {
        yield return new WaitForSeconds(1f);
        //that's it.
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
        raccoonAI.DecideInitialBehavior();
        raccoonAI.enabled = true; //Enable AI after intro sequence.
        
    }
}
