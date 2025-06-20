using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RaccoonAI : MonoBehaviour
{

    public enum RaccoonState
    {
        Idle,
        Chase,
        Attack,
        Stunned,
        Berserk,
        Retreat     
    }

    public RaccoonState currentState = RaccoonState.Idle;
    public float detectionRange = 5f;
    public float moveSpeed = 2f;
    public Transform player;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isStunned = false;
    [SerializeField] private int playerDamage = 5;
    //Raccoon Aggression
    [Range(0f, 1f)]
    public float aggressionChance = 0.2f; // Starts low (10–20%)

    //Raccoon health
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    //Raccoon windup time
    [SerializeField] private float attackWindupTime = 0.5f;

    //Raccoon Stunned time
    private float flinchTime = 0.4f;

    //Animation states
    const string IDLE_ANIMATION = "isIdle";
    const string CHASING_ANIMATION = "isChasing";
    const string RETREATING_ANIMATION = "isRetreating";
    const string ATTACK_ANIMATION = "ATTACK";
    const string STUNNED_ANIMATION = "STUNNED";
    const string DEATH_ANIMATION = "DEATH";
    const string HIT_ANIMATION = "isHit";
    const string WINDUP_ANIMATION = "isWindingUp";


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        //create a player reference
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        //initialize health
        currentHealth = maxHealth;
    }

    public void DecideInitialBehavior()
    {
        float roll = Random.value;
        UnityEngine.Debug.Log($"Raccoon roll: {roll}, aggressionChance: {aggressionChance}");

        if (roll < aggressionChance)
        {
            currentState = RaccoonState.Attack;
            UnityEngine.Debug.Log("Raccoon decided: ATTACK!");
        }
        else
        {
            currentState = RaccoonState.Retreat;
            UnityEngine.Debug.Log("Raccoon decided: RETREAT!");
            anim.SetBool(RETREATING_ANIMATION, true);
        }
    }

    void Update()
    {
        if (isStunned) return;

        switch (currentState)
        {
            case RaccoonState.Idle:
                HandleIdle();
                break;
            case RaccoonState.Chase:
                HandleChase();
                break;
            case RaccoonState.Attack:
                HandleAttack();
                break;
            case RaccoonState.Stunned:
                HandleStunned(); // Optional: timeout & recover
                break;
            case RaccoonState.Berserk:
                HandleBerserk(); // Optional: later mechanic
                break;
            case RaccoonState.Retreat:
                UnityEngine.Debug.Log("Raccoon is RETREATING!");
                HandleRetreat();
                break;
        }
    }



    void HandleIdle()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < detectionRange)
        {
            currentState = RaccoonState.Chase;
            ResetAnimationStates();
            anim.SetBool(CHASING_ANIMATION, true);
        }
    }

    void HandleChase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Flip based on movement direction
        if (direction.x > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else
            transform.localScale = new Vector3(-1f, 1f, 1f);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 1f)
        {
            rb.velocity = Vector2.zero;
            currentState = RaccoonState.Attack;
            anim.SetBool(CHASING_ANIMATION, false); // Exit CHASE state cleanly
        }
    }


    private bool hasAttacked = false;

    void HandleAttack()
    {
        if (!hasAttacked)
        {
            hasAttacked = true;
            StartCoroutine(AttackWindup());
        }
    }

    IEnumerator AttackWindup()
    {
        anim.SetBool(WINDUP_ANIMATION, true); // New animation trigger for windup frame
        yield return new WaitForSeconds(attackWindupTime);
        ExecuteAttack();
    }

    void ExecuteAttack()
    {
        anim.SetBool(WINDUP_ANIMATION, false);
        anim.SetTrigger(ATTACK_ANIMATION); // Trigger the attack animation
        rb.velocity = Vector2.zero; // Lock in place during attack

        DealDamageIfPlayerInRange();
        // Give it time to finish the animation before changing state
        Invoke(nameof(EndAttack), 0.5f); // Adjust 0.5f to match anim length
    }



    void EndAttack()
    {
        hasAttacked = false;
        currentState = RaccoonState.Idle;

        // Optional: If you want him to go back to chase immediately
        // currentState = RaccoonState.Chase;
    }

    void DealDamageIfPlayerInRange()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 1f)
        {
            // Call Player's hurt function or reduce health
            player.GetComponent<CoyoteHealthUI>().TakeDamage(playerDamage);
            player.GetComponent<PlayerActions>().Hit();
        }
    }




    public void StunRaccoon(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            currentState = RaccoonState.Stunned;
            rb.velocity = Vector2.zero;
            anim.Play("STUNNED");
            StartCoroutine(StunDuration(duration));
        }
    }

    IEnumerator StunDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
        currentState = RaccoonState.Idle;
        ResetAnimationStates();
        anim.SetBool(IDLE_ANIMATION, true);
        anim.SetBool(CHASING_ANIMATION, false);
        anim.SetBool(RETREATING_ANIMATION, false);
    }

    void HandleStunned() { /* Empty, just managed by coroutine */ }

    public void TakeHit(int damage)
    {
        if ((isDead || isStunned)) return;
        currentHealth -= damage;
        UnityEngine.Debug.Log($"Raccoon took {damage} damage. Remaining: {currentHealth}");

        if (currentHealth <=0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Flinch());
        }
    }

    IEnumerator Flinch()
    {
        currentState = RaccoonState.Stunned;
        isStunned = true;
        anim.Play(HIT_ANIMATION);//plays flinch frame
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(flinchTime);
        isStunned = false;
        currentState = RaccoonState.Idle;
        ResetAnimationStates();
        anim.SetBool(IDLE_ANIMATION, true);
    }

    void HandleBerserk()
    {
        // Optional wild logic — faster, more aggressive, maybe unpredictable movement
    }

    void ResetAnimationStates()
    {
        anim.SetBool(IDLE_ANIMATION, false);
        anim.SetBool(CHASING_ANIMATION, false);
        anim.SetBool(RETREATING_ANIMATION, false);
    }

    void HandleRetreat()
    {
        // Move AWAY from the player instead of toward
        Vector2 direction = (transform.position - player.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Flip the sprite to face the correct direction
        transform.localScale = new Vector3(
            direction.x > 0 ? 1f : -1f,
            1f, 1f
        );

        // Set Animator flag
        if (!anim.GetBool(RETREATING_ANIMATION))
            anim.SetBool(RETREATING_ANIMATION, true);

        anim.SetBool(CHASING_ANIMATION, false);
        anim.SetBool(IDLE_ANIMATION, false);
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger(DEATH_ANIMATION);//gotta make a death animation state.
        GetComponent<BoxCollider2D>().enabled = false;
        //optionally destroy after time
        Destroy(gameObject, 3f);
    }

}


