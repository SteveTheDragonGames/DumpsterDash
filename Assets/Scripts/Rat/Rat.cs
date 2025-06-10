using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : MonoBehaviour
{
    public GameObject smokePuffPrefab;
    public float speed = 5f;
    private BoxCollider2D col2D;
    private SpriteRenderer sr;
    private Animator anim;
    private Rigidbody2D rb;

    public float activeTime = 0.2f; // Adjust timing here
    public float TorqueRange = 8f;
    private string BOOT_ANIMATION = "Boot";
    public float kickForce = 4f;
    public float delay = 1.4f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col2D = GetComponent<BoxCollider2D>();
        col2D.enabled = false;
        Invoke(nameof(EnableCollider), 0.5f);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
        if (collision.CompareTag("Boundary"))
        {
            RatElectrocution zap = GetComponent<RatElectrocution>();
            if (zap != null)
                zap.TriggerZappyDeath();
            else
                Debug.LogWarning("RatElectrocution Component missing!");
        }

        if (collision.CompareTag("PlayerKicker"))
        {
            anim.SetTrigger(BOOT_ANIMATION);
            rb.constraints = RigidbodyConstraints2D.None;

            float direction = collision.transform.localScale.x;
            Vector2 force = new Vector2(direction * kickForce, 8f);
            rb.AddForce(force, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-TorqueRange, TorqueRange), ForceMode2D.Impulse);

            StartCoroutine(KillAfterDelay());
        }
    }

    IEnumerator KillAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        if (smokePuffPrefab)
        {
            Instantiate(smokePuffPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
        Debug.Log("Rat Destroyed!");
    }
}
