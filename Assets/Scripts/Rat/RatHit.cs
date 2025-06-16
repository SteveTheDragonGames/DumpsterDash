using System.Diagnostics;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;


public class RatHit : MonoBehaviour
{
    public Animator animator;
    public float victoryDelay = 0.5f;
    [Range(0f, 1f)] public float tPoseChance = 0.1f; // 10% chance

    public enum HitType { Smoosh, Boot }

    public GameObject floatingScore;
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

    [SerializeField] private bool debugHitLog = false;

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

            bool coyoteAbove = collision.transform.position.y > transform.position.y + 0.2f;
            bool falling = coyoteRb.velocity.y < -0.1f;

            if (coyoteAbove && falling)
            {
                StopMoving();
                Debug.Log("✅ Stomp detected");
                Vector2 bounce = new Vector2(coyoteRb.velocity.x, 8f);
                coyoteRb.velocity = bounce;
                HitRat(HitType.Smoosh, Vector2.zero);
            }
            else
            {
                // Apply knockback
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                coyoteRb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse); // adjust force as needed
                collision.gameObject.GetComponent<PlayerActions>().Hit();
                UnityEngine.Debug.Log("Rat bit back!");
            }
            
            
        }

    }
    private void StopMoving()
    {
        if (rb != null) rb.velocity = Vector2.zero;
        var moveScript = GetComponent<Rat>();
        if (moveScript != null) moveScript.enabled = false;
    }


    public void HitRat(HitType type, Vector2 fromDirection)
    {
        //UnityEngine.Debug.Log("RatHit: Type = " + type + ", Score = " + scoreValue + ", Direction = " + fromDirection);


        if (debugHitLog)
            UnityEngine.Debug.Log($"Rat hit with {type} from direction {fromDirection}");


        if (hasBeenHit) return;
        hasBeenHit = true;

        GameManager.Instance.AwardPoints(scoreValue);
        if (floatingScore != null)
        {
            
            Vector3 popupPos = transform.position + new Vector3(0, 1.5f, 0); // float slightly above rat
            GameObject scorePopup = Instantiate(floatingScore, popupPos, Quaternion.identity);
            scorePopup.GetComponent<FloatingScore>().SetScore(scoreValue);
        }


        switch (type)
        {
            case HitType.Smoosh:
                if (smooshEffect) Instantiate(smooshEffect, transform.position, Quaternion.identity);
                if (audioSource && smooshSound) audioSource.PlayOneShot(smooshSound);
                StopMoving();
                DestroyGameObject();
                break;

            case HitType.Boot:
                if (bootEffect) Instantiate(bootEffect, transform.position, Quaternion.identity);
                if (audioSource && bootSound) audioSource.PlayOneShot(bootSound);
                StopMoving();
                if (rb != null)
                {
                    // 🥾 ADD THIS PART RIGHT HERE:
                    rb.constraints = RigidbodyConstraints2D.None;
                    Vector2 yeetDirection = (fromDirection.normalized + Vector2.up * 0.5f).normalized;
                    rb.velocity = yeetDirection * 35f; // GO BIG
                    rb.AddTorque(Random.Range(-50f, 50f), ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-100f, 100f), ForceMode2D.Impulse);
                    UnityEngine.Debug.Log("RAT YEETED with force: " + fromDirection.normalized * 20f);
                }

                StartCoroutine(nameof(DisableAfterBoot));
                break;
        }

        IEnumerator DisableAfterBoot()
        {
            //UnityEngine.Debug.Log("👻 DelayedDisable() CALLED on: " + gameObject.name);
            yield return new WaitForSeconds(.75f);
            DestroyGameObject();
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
                UnityEngine.Debug.LogWarning("RatElectrocution Component missing!");
        }

    }

    public void CelebrateVictory()
    {
        StartCoroutine(DoVictoryDance());
    }

    IEnumerator DoVictoryDance()
    {
        yield return new WaitForSeconds(victoryDelay);

        float roll = Random.value;
        if (roll <= tPoseChance)
        {
            animator.SetTrigger("TPose"); // T-pose of glory
        }
        else
        {
            animator.SetTrigger("Dance"); // Default goofy wiggle
        }
    }

    public void DestroyGameObject()
    {
        sr.enabled = false;
        col.enabled = false;
        rb.simulated = false;
        Destroy(gameObject, 0.4f); // short delay for sound or fx
    }
}
