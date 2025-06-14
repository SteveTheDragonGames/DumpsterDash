using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStates))]
public class PlayerMovement : MonoBehaviour
{
    public float moveForce = 10f;
    public float jumpForce = 11f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerStates states;
    private float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private SpriteRenderer sr;
    

    [SerializeField] private float fallMultiplier = 2.5f;
    public GameObject kickHitBox;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        states = GetComponent<PlayerStates>();
        sr = GetComponent<SpriteRenderer>();
    }



    void Update()
    {
        CheckGround();
        CheckFall();
        FallBoost();
    }

    private void CheckFall()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            states.SetState(PlayerState.Falling);
        }
        if (isGrounded &&
    (states.IsState(PlayerState.Jumping) || states.IsState(PlayerState.Falling)))
        {
            states.SetState(PlayerState.Idle);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && states.IsState(PlayerState.Jumping))
        {
            states.SetState(PlayerState.Idle);
        }
    }

    public void Move(float dir)
    {
        if (!states.CanMove()) return;
        rb.velocity = new Vector2(dir * moveForce, rb.velocity.y);
        // Flip sprite
        sr.flipX = dir < 0;
        //Flip KickHitBox
        kickHitBox.transform.localScale = new Vector3(dir, 1, 1);

    }

    public void Jump()
    {
        if (!isGrounded) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        states.SetState(PlayerState.Jumping);
    }

    private void FallBoost()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }
}