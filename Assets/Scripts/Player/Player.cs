using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float jumpForce = 11f;
    
    //GroundCheck
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckRadius = 0.2f;
    private bool isGrounded;


    private bool isHowling = false;
    private bool isSearching = false;
    private bool isSpritzing = false;
    private bool isJunk = false;
    [SerializeField] private float ITSHOWLINTIME = .5f;
    private bool isNearDumpster = false;
    private bool canMove = true;

    private float movementX;

    private Rigidbody2D myBody;
    private SpriteRenderer sr;
    private Animator anim;

    private string WALK_ANIMATION = "isWalking";
    private string JUMP_ANIMATION = "isJumping";
    private string HOWL_ANIMATION = "Howl";
    private string SEARCH_ANIMATION = "isSearching";
    private string SPRITZ_ANIMATION = "isSpritzing";

    private Transform currentDumpster = null;
    [SerializeField] private SignAnimator signAnimator;

    private JunkItem foundJunk;
    private GameObject racoonHellSpawn = null;
    private GameObject racoonPrefab = null;

    private Coroutine currentSearchRoutine;

    // Start is called before the first frame update
    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

 
    void Update()
    {
        CheckIfGrounded();
        PlayerCheckKeyboard();
        FlipSprite();
        AnimateCoyote();
    }

    void SetMovement(bool allowed)
    {
        canMove = allowed;
    }

    void PlayerCheckKeyboard()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) PlayerJump();
        if(Input.GetButtonDown("Fire1") && isGrounded) PlayerSpritz();
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded) PlayerSearch();
        if (Input.GetButtonDown("Fire3")) PlayerHowl();

        if (canMove)
        {
            movementX = Input.GetAxisRaw("Horizontal");
            transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
        }
        
    }

    void FlipSprite()
    {
        if (movementX > 0) sr.flipX = false;
        else if(movementX<0) sr.flipX = true;

    }
    void AnimateCoyote()
    {
        //if we're walking, walk_animation is true, otherwise not walking.
        anim.SetBool(WALK_ANIMATION, movementX != 0);
        anim.SetBool(JUMP_ANIMATION, !isGrounded);
        anim.SetBool(SEARCH_ANIMATION, isSearching);
    }


    void PlayerJump()
    {
        if (currentSearchRoutine != null) CancelSearch();
        myBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

    }

    void PlayerHowl()
    {
        if (isHowling) return;
        anim.SetBool(HOWL_ANIMATION, true);
        isHowling = true;
        SetMovement(false);
        StartCoroutine(FinishHowling());
    }

    IEnumerator FinishHowling()
    {
        yield return new WaitForSeconds(ITSHOWLINTIME);
        anim.SetBool(HOWL_ANIMATION, false);
        isHowling = false;
        SetMovement(true);
    }

    void PlayerSearch()
    {
        Dumpster dumpsterScript = currentDumpster.GetComponent<Dumpster>();
        if (dumpsterScript == null)
        {
            UnityEngine.Debug.LogWarning("No Dumpster script found on the current dumpster!");
        }

        dumpsterScript.OpenLid();

        if (isSearching || !isNearDumpster) return;


            SnapToDumpster();
            anim.SetBool(SEARCH_ANIMATION, true);
            isSearching = true;
            SetMovement(false);

            currentSearchRoutine = StartCoroutine(FinishSearching(1.5f));

    }

    public void CancelSearch()
    {
        if (currentSearchRoutine != null)
        {
            StopCoroutine(currentSearchRoutine);
            currentSearchRoutine = null;
        }

        anim.SetBool(SEARCH_ANIMATION, false);
        isSearching = false;
        SetMovement(true);
    }

    IEnumerator FinishSearching(float delay)
    {
        yield return new WaitForSeconds(delay);

        Dumpster dumpsterScript = currentDumpster.GetComponent<Dumpster>();
        if (dumpsterScript == null)
        {
            UnityEngine.Debug.LogWarning("No Dumpster script found on the current dumpster!");
            yield break;
        }


        float spawnChance = UnityEngine.Random.value;

        if (spawnChance < 0.8f)
        {
            isJunk = true;
            if (dumpsterScript != null)
            // pull out junk
            {
                foundJunk = dumpsterScript.GetRandomJunk();
                //UnityEngine.Debug.Log("You found: " + foundJunk.name + " - " + foundJunk.description + " (+" + foundJunk.scoreValue + " points)");
                Vector3 spawnPos = currentDumpster.transform.position + new Vector3(0, 1f, 0);
                GameObject currentJunk = Instantiate(foundJunk.junkItem, spawnPos, Quaternion.identity);
                Rigidbody2D rb = currentJunk.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.AddForce(new Vector2(UnityEngine.Random.Range(-3f, 3f), 5f), ForceMode2D.Impulse);//lil' pop effect
                }

            }
        }
        else
        {
            isJunk = false;
            if (dumpsterScript != null)
            {
                racoonPrefab = dumpsterScript.racoon;
            }
            float spawnX = (Random.value < 0.5f) ? -2f : 2f;
            Vector3 spawnPos = currentDumpster.transform.position + new Vector3(spawnX, 1f, 0);
            racoonHellSpawn = Instantiate(racoonPrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = racoonHellSpawn.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity = Vector2.zero; // Clean slate!
                rb.AddForce(new Vector2(Random.Range(-1.5f, 1.5f), 10f), ForceMode2D.Impulse);
            }
        }


            if (isJunk)
            {
                signAnimator.PopSign(foundJunk.name, foundJunk.description);
            }
       
        anim.SetBool(SEARCH_ANIMATION, false);
        isSearching = false;
        dumpsterScript.CloseLid();
        SetMovement(true);

    }



    void PlayerSpritz()
    {
        isSpritzing = true;
        anim.SetBool(SPRITZ_ANIMATION, true);
        StartCoroutine(FinishSpritzing());
    }

    IEnumerator FinishSpritzing()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool(SPRITZ_ANIMATION, false);
        isSpritzing = false;
    }

    private void SnapToDumpster()
    {
        transform.position = new Vector3(
            currentDumpster.position.x,
            transform.position.y,
            transform.position.z);
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //Debug.Log("isGrounded = " + isGrounded);

        if (isGrounded) anim.SetBool(JUMP_ANIMATION, false);


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dumpster"))
        {
            isNearDumpster = true;
            currentDumpster = collision.transform;
        }
            
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dumpster"))
        {
            isNearDumpster = false;
            currentDumpster = null;
        }    
    }

 

}
