using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatSmoosh : MonoBehaviour
{
    public GameObject smooshEffect; // optional particle or flattened sprite
    public AudioClip smooshSound;   // optional death sound
    private AudioSource audioSource;

    private SpriteRenderer sr;
    private BoxCollider2D col;
    private Rigidbody2D rb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D coyoteRb = collision.gameObject.GetComponent<Rigidbody2D>();

            bool coyoteAbove = collision.transform.position.y > transform.position.y + 0.3f;
            bool falling = coyoteRb.velocity.y < -0.5f;

            if (coyoteAbove && falling)
            {
                SmooshRat();
                // Bounce the coyote upward a bit
                coyoteRb.velocity = new Vector2(coyoteRb.velocity.x, 8f);
            }
            else
            {
                // Optional: Hurt the coyote or bounce them back
                Debug.Log("Rat bit back!");
            }
        }
    }

    void SmooshRat()
    {
        // Optional: Instantiate an effect
        if (smooshEffect != null)
            Instantiate(smooshEffect, transform.position, Quaternion.identity);

        if (audioSource != null && smooshSound != null)
            audioSource.PlayOneShot(smooshSound);

        // Disable visuals & collider right away
        sr.enabled = false;
        col.enabled = false;
        rb.simulated = false;

        // Destroy after short delay to let audio play
        Destroy(gameObject, 0.3f);
    }
}
