using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAI : MonoBehaviour, IHittable
{
    public enum RatState { Roaming, Knocked, Dead }

    private RatState currentState = RatState.Roaming;

    private Rigidbody2D rb;
    private Collider2D col;

    public float moveSpeed = 2f;
    private Vector2 moveDirection;

    private bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        moveDirection = Vector2.left; // Default starting direction
    }

    void Update()
    {
        if (canMove && currentState == RatState.Roaming)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void FlipDirection()
    {
        moveDirection = -moveDirection;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // We'll handle player damage later
            FlipDirection();
        }
    }

    public void TakeHit(HitType type, Vector2 direction)
    {
        if (currentState == RatState.Dead || currentState == RatState.Knocked) return;

        switch (type)
        {
            case HitType.Boot:
                currentState = RatState.Knocked;
                rb.velocity = Vector2.zero;
                rb.AddForce(direction * 8f, ForceMode2D.Impulse);
                rb.AddTorque(30f);
                StartCoroutine(DeathAfterDelay());
                break;

            case HitType.Smoosh:
                DieImmediately();
                break;

            case HitType.Electric:
                StartCoroutine(FryAndDie());
                break;
        }

        IEnumerator DeathAfterDelay()
        {
            canMove = false;
            yield return new WaitForSeconds(0.75f);
            Destroy(gameObject);
        }

        void DieImmediately()
        {
            canMove = false;
            currentState = RatState.Dead;
            Destroy(gameObject);
        }

        IEnumerator FryAndDie()
        {
            canMove = false;
            currentState = RatState.Knocked; // Or a future "Zapped" state if you want
                                             // TODO: play zap effect/sound here
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        } 

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("KickHitBox"))
        {
            Vector2 direction = (transform.position - other.transform.position).normalized;
            TakeHit(HitType.Boot, direction);
        }
        else if (other.CompareTag("PlayerFeet")) // optional, for smoosh detection
        {
            TakeHit(HitType.Smoosh, Vector2.zero);
        }
        else if (other.CompareTag("ElectricZone")) // optional, for shocking tiles/devices
        {
            TakeHit(HitType.Electric, Vector2.zero);
        }
    }
}
