using UnityEngine;

[RequireComponent(typeof(PlayerStates))]
public class PlayerInput : MonoBehaviour
{
    private PlayerStates states;
    private PlayerMovement movement;
    private PlayerSearch search;
    private PlayerActions actions;

    void Awake()
    {
        states = GetComponent<PlayerStates>();
        movement = GetComponent<PlayerMovement>();
        search = GetComponent<PlayerSearch>();
        actions = GetComponent<PlayerActions>();
    }

    void Update()
    {
        // Movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontal) > 0.01f && states.CanMove())
        {
            movement.Move(horizontal);
            states.SetState(PlayerState.Moving);
        }
        else if (states.IsState(PlayerState.Moving))
        {
            states.SetState(PlayerState.Idle);
        }

        // Jump input
        if (Input.GetButtonDown("Jump") && states.CanJump())
        {
            movement.Jump();
            states.SetState(PlayerState.Jumping);
        }

        // Howl input
        if (Input.GetButtonDown("Fire3") && states.CanAttack())
        {
            actions.Howl();
        }

        // Spritz/Kick input
        if (Input.GetButtonDown("Fire1") && states.CanAttack())
        {
            actions.SpritzOrKick();
        }

        // Search input
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && states.CanSearch())
        {
            search.Search();
        }
    }
}