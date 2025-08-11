using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;


public class RaccoonAI : MonoBehaviour, IHittable
{

    public enum RaccoonState
    {
        Idle,
        Chase,
        Attack,
        Stunned,
        Berserk,
        Retreat,
        Dead
    }

    [SerializeField] Transform[] stateParents;

    public RaccoonState currentState = RaccoonState.Idle;
    public float detectionRange = 5f;
    public float moveSpeed = 2f;
    public Transform player;
    private string _lastStateName;

    private Rigidbody2D rb;
    private bool isStunned = false;
    [SerializeField] private int playerDamage = 5;
    //Raccoon Aggression
    [Range(0f, 1f)]
    private float aggressionChance = 0.2f; // Starts low (10–20%)
    public float AggressionChance
    {
        get { return aggressionChance; }
        set { aggressionChance = Mathf.Clamp01(value);
        UnityEngine.Debug.Log($"Aggression chance set to: {aggressionChance}");
        }
    }

    public float stunRecoveryBonus = 0;

    [SerializeField] private float attackRange = 1f;

    //Raccoon health
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    //Raccoon windup time
    [SerializeField] private float attackWindupTime = 0.5f;
    [SerializeField] private float attackDuration = 0.5f;

    //Raccoon Stunned time
    private float flinchTime = 0.4f;

    //Raccoon attack flash
    private IEnumerator FlashColor(Color color, float flashTime, int flashes = 1)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;

        for (int i = 0; i < flashes; i++)
        {
            sr.color = color;
            yield return new WaitForSeconds(flashTime / 2f);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashTime / 2f);
        }
    }




    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        currentHealth = maxHealth;
        SetState(currentState.ToString()); // <-- don’t force Idle
    }



    public void DecideInitialBehavior()
    {
        float roll = Random.value;
        UnityEngine.Debug.Log($"Raccoon roll: {roll}, aggressionChance: {aggressionChance}");

        if (roll < aggressionChance)
        {
            currentState = RaccoonState.Attack;
            SetState("Windup"); // or "Attack" if you prefer instant smack
        }

        else
        {
            currentState = RaccoonState.Retreat;
            UnityEngine.Debug.Log("Raccoon decided: RETREAT!");
            SetState("RunAway");
        }
    }

    void Update()
    {
        if (isStunned || isDead) return;

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

    public void SetState(string stateName)
    {
        if (string.IsNullOrWhiteSpace(stateName))
        {
            Debug.LogWarning("[RaccoonAI] SetState called with empty name.");
            return;
        }

        string target = stateName.Trim();
        bool matched = false;
        Transform idle = null;

        foreach (var t in stateParents)
        {
            if (!t) continue;
            if (string.Equals(t.name.Trim(), "Idle", System.StringComparison.OrdinalIgnoreCase))
                idle = t;

            bool on = string.Equals(t.name.Trim(), target, System.StringComparison.OrdinalIgnoreCase);
            t.gameObject.SetActive(on);
            if (on) matched = true;
        }

        if (!matched)
        {
            Debug.LogWarning($"[RaccoonAI] NO MATCH for '{target}'. Falling back to Idle.");
            if (idle) idle.gameObject.SetActive(true);
        }
    }






    void HandleIdle()
    {
        if (player == null) return;

        SetState("Idle");

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            currentState = RaccoonState.Chase;
            return;
        }
    }



    void HandleChase()
    {
        if (player == null) return;

        // Vector to player and distance (compute once)
        Vector2 toPlayer = (player.position - transform.position);
        float distance = toPlayer.magnitude;

        // Ensure the correct visual state (idempotent)
        SetState("Chase");

        // Move horizontally toward the player
        Vector2 dir = toPlayer.normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);

        // Flip sprite based on direction
        var scale = transform.localScale;
        scale.x = Mathf.Sign(dir.x) >= 0 ? 1f : -1f;
        transform.localScale = scale;

        // Transition to Attack when close enough
        if (distance <= attackRange)
        {
            rb.velocity = Vector2.zero;
            currentState = RaccoonState.Attack;
            return;
        }

        //stop chasing if player gets away
        if (distance > detectionRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            currentState = RaccoonState.Idle;
            return;
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
        SetState("Windup");

        float flashTime = 0.1f; // How long the red flash lasts
        float flashTriggerTime = attackWindupTime - flashTime;

        yield return new WaitForSeconds(flashTriggerTime);

        // ⚡ Flash red just before punch
        StartCoroutine(FlashColor(Color.red, flashTime, 1));

        // Wait the remaining time to complete the windup
        yield return new WaitForSeconds(flashTime);

        ExecuteAttack();
    }


    void ExecuteAttack()
    {
        SetState("Attack"); // Trigger the attack animation
        rb.velocity = Vector2.zero; // Lock in place during attack

        DealDamageIfPlayerInRange();
        // Give it time to finish the animation before changing state
        Invoke(nameof(EndAttack), attackDuration);
    }



    void EndAttack()
    {
        hasAttacked = false;
        currentState = RaccoonState.Chase;
        SetState("Chase");


        // Optional: If you want him to go back to chase immediately
        // currentState = RaccoonState.Chase;
    }

    void DealDamageIfPlayerInRange()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            if (player.TryGetComponent(out CoyoteHealthUI health))
                health.TakeDamage(playerDamage);

            if (player.TryGetComponent(out PlayerActions actions))
                actions.Hit();
        }
    }






    public void Stun(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            currentState = RaccoonState.Stunned;
            rb.velocity = Vector2.zero;
            SetState("Flinch");
            StartCoroutine(StunDuration(duration));
        }
    }


    IEnumerator StunDuration(float duration)
    {
        float adjustedDuration = Mathf.Max(0f, duration - stunRecoveryBonus);
        yield return new WaitForSeconds(adjustedDuration);
        isStunned = false;
        currentState = RaccoonState.Idle;
        SetState("Idle");
    }



    void HandleStunned() { /* Empty, just managed by coroutine */ }

    public void TakeHit(int damage)
    {
        if (isDead || isStunned) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // prevent negative HP
        UnityEngine.Debug.Log($"Raccoon took {damage} damage. Remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Stun(flinchTime);
        }
    }


    public void TakeHit(HitType hitType, Vector2 hitDirection)
    {
        if (isDead || isStunned) return;

        switch (hitType)
        {
            case HitType.Boot:
                TakeHit(3); // this is your existing int-based logic
                break;
            case HitType.Spritz:
                TakeHit(1);
                break;
        }
    }


    void HandleBerserk()
    {
        // Optional wild logic — faster, more aggressive, maybe unpredictable movement
    }


    void HandleRetreat()
    {
        SetState("RunAway");
        Vector2 direction = (transform.position - player.position).normalized;
        float retreatSpeedMultiplier = 1.5f;
        rb.velocity = new Vector2(direction.x * moveSpeed * retreatSpeedMultiplier, rb.velocity.y);

        transform.localScale = new Vector3(
            direction.x > 0 ? 1f : -1f,
            1f, 1f
        );

        


        // 💥 NEW: Destroy if far offscreen
        if (Mathf.Abs(transform.position.x - player.position.x) > 12f)
        {
            UnityEngine.Debug.Log("Raccoon has retreated offscreen. Destroying.");
            Destroy(gameObject);
        }
    }


    void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();
        SetState("Dead");
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        GetComponent<Collider2D>().isTrigger = true;
        Destroy(gameObject, 3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void SetStateToIdle()
    {
        currentState = RaccoonState.Idle;
    }


}


