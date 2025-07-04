using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RatState { Roaming, Knocked, Dead }

public class RatAI : MonoBehaviour, IHittable
{

    public RatState ratState { get; private set; }

    private Rigidbody2D rb;
    private Collider2D col;

    public float moveSpeed = 2f;
    private Vector2 moveDirection;

    private bool canMove = true;

    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float torqueForce = 30f;

    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject smooshEffect;
    [SerializeField] private GameObject bootEffect;
    [SerializeField] private GameObject smokePuffPrefab;
    [SerializeField] private GameObject floatingScore;
    [SerializeField] private AudioClip smooshSound;
    [SerializeField] private AudioClip bootSound;
    [SerializeField] private AudioClip normalSqueak;
    [SerializeField] private AudioClip wilhelmSqueak;
    [SerializeField, Range(0f, 1f)] private float wilhelmChance = 0.03f;
    [SerializeField] private float victoryDelay = 0.5f;
    [SerializeField, Range(0f, 1f)] private float tPoseChance = 0.25f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        moveDirection = Vector2.left; // Default starting direction
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // default constraint
    }

    void Start()
    {
        ratState = RatState.Roaming;
    }

    void Update()
    {
        if (canMove && ratState == RatState.Roaming)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    //Show rats direction in editor
#if UNITY_EDITOR
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.green;
    Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveDirection * 0.5f);
}
#endif

    public void SetState(RatState newState)
    {
        ratState = newState;
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
            Rigidbody2D coyoteRb = collision.gameObject.GetComponent<Rigidbody2D>();
            bool coyoteAbove = collision.transform.position.y > transform.position.y + 0.2f;
            bool falling = coyoteRb.velocity.y < -0.1f;

            if (coyoteAbove && falling)
            {
                Vector2 bounce = new Vector2(coyoteRb.velocity.x, 8f);
                coyoteRb.velocity = bounce;
                TakeHit(HitType.Smoosh, Vector2.zero);
            }
            else
            {
                // Knock player back
                Vector2 knockback = (collision.transform.position - transform.position).normalized;
                coyoteRb.AddForce(knockback * 5f, ForceMode2D.Impulse);
                collision.gameObject.GetComponent<PlayerActions>().Hit();
                Debug.Log("Rat bit back!");
                FlipDirection();
            }
        }
    }

    public void TakeHit(HitType type, Vector2 direction)
    {
        Debug.Log("Rat took hit: " + type + " | New State: " + ratState);
        if (ratState == RatState.Dead || ratState == RatState.Knocked) return;

        switch (type)
        {
            case HitType.Smoosh:
                PlaySound(smooshSound);
                SpawnEffect(smooshEffect);
                AwardScore();
                DieImmediately();
                break;

            case HitType.Boot:
                PlaySound(bootSound);
                SpawnEffect(bootEffect);
                AwardScore();
                Yeet(direction);
                StartCoroutine(DeathAfterDelay());
                break;

            case HitType.Electric:
                StartCoroutine(ElectrocuteAndVanish());
                break;
        }

    }

    private IEnumerator DeathAfterDelay()
    {
        canMove = false;
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }

    private void DieImmediately()
    {
        canMove = false;
        ratState = RatState.Dead;
        Destroy(gameObject);
    }

    private IEnumerator FryAndDie()
    {
        canMove = false;
        ratState = RatState.Knocked; // Or a future "Zapped" state if you want
                                     // TODO: play zap effect/sound here
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
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

    private void AwardScore()
    {
        GameManager.Instance?.AwardPoints(10); // Or make `scoreValue` a field
        if (floatingScore)
        {
            Vector3 popupPos = transform.position + new Vector3(0, 1.5f, 0);
            Instantiate(floatingScore, popupPos, Quaternion.identity)
                .GetComponent<FloatingScore>()?.SetScore(10);
        }
    }

    private void SpawnEffect(GameObject fx)
    {
        if (fx) Instantiate(fx, transform.position, Quaternion.identity);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip) audioSource.PlayOneShot(clip);
    }

    private void Yeet(Vector2 dir)
    {
        rb.constraints = RigidbodyConstraints2D.None;
        Vector2 yeetDirection = (dir.normalized + Vector2.up * 0.5f).normalized;
        rb.velocity = yeetDirection * 35f;
        rb.AddTorque(Random.Range(-100f, 100f), ForceMode2D.Impulse);
    }

    private IEnumerator ElectrocuteAndVanish()
    {
        canMove = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), 10f), ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-15f, 15f), ForceMode2D.Impulse);

        animator?.SetTrigger("Zap");
        AudioClip clip = (Random.value < wilhelmChance) ? wilhelmSqueak : normalSqueak;
        PlaySound(clip);
        yield return new WaitForSeconds(0.4f);

        if (smokePuffPrefab)
            Instantiate(smokePuffPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void CelebrateVictory()
    {
        StartCoroutine(DoVictoryDance());
    }

    private IEnumerator DoVictoryDance()
    {
        yield return new WaitForSeconds(victoryDelay);
        float roll = Random.value;
        animator.SetTrigger(roll <= tPoseChance ? "TPose" : "Dance");
    }

}
