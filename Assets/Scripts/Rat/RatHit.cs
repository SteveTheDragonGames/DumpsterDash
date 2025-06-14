//using System.Diagnostics;
using UnityEngine;

public class RatHit : MonoBehaviour
{
    public enum HitType { Smoosh, Boot }

    public GameObject smooshEffect;
    public GameObject bootEffect;
    public AudioClip smooshSound;
    public AudioClip bootSound;
    public int scoreValue = 10;
    public float bootForce = 5f;

    private AudioSource audioSource;
    private SpriteRenderer sr;
    private BoxCollider2D col;
    private Rigidbody2D rb;

    private bool hasBeenHit = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenHit) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D coyoteRb = collision.gameObject.GetComponent<Rigidbody2D>();

            bool coyoteAbove = collision.transform.position.y > transform.position.y + 0.3f;
            bool falling = coyoteRb.velocity.y < -0.5f;

            if (coyoteAbove && falling)
            {
                Vector2 bounce = new Vector2(coyoteRb.velocity.x, 8f);
                coyoteRb.velocity = bounce;

                HitRat(HitType.Smoosh, Vector2.zero);
                GameManager.Instance.AwardPoints(scoreValue);
            }
            else
            {
                // Optional: damage coyote or knock them back
                Debug.Log("Rat bit back!");
            }
        }
    }

    public void HitRat(HitType type, Vector2 fromDirection)
    {
        if (hasBeenHit) return;
        hasBeenHit = true;

        GameManager.Instance.AwardPoints(scoreValue);

        switch (type)
        {
            case HitType.Smoosh:
                if (smooshEffect) Instantiate(smooshEffect, transform.position, Quaternion.identity);
                if (audioSource && smooshSound) audioSource.PlayOneShot(smooshSound);
                break;

            case HitType.Boot:
                if (bootEffect) Instantiate(bootEffect, transform.position, Quaternion.identity);
                if (audioSource && bootSound) audioSource.PlayOneShot(bootSound);
                if (rb != null) rb.AddForce(fromDirection.normalized * bootForce, ForceMode2D.Impulse);
                //Delay disabling visuals and destruction so kick is visible.
                Invoke(nameof(DisableAfterBoot), 0.75f);// Give the rat time to fly.

                break;
        }

        void DisableAfterBoot()
        {
            sr.enabled = false;
            col.enabled = false;
            rb.simulated = false;

            Destroy(gameObject, 0.4f);// short delay for audio to finish.
        }

    }
}
