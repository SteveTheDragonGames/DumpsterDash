using UnityEngine;

[RequireComponent(typeof(PlayerStates))]
public class PlayerAnimationController : MonoBehaviour
{
    private PlayerStates states;
    private Animator anim;

    void Awake()
    {
        states = GetComponent<PlayerStates>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (states.IsState(PlayerState.Dead)) return;

        anim.SetBool("isWalking", states.IsState(PlayerState.Moving));
        anim.SetBool("isJumping", states.IsState(PlayerState.Jumping) || states.IsState(PlayerState.Falling));
        anim.SetBool("isSearching", states.IsState(PlayerState.Searching));
        // Spritz, Howl, and Kick are handled in their respective modules

        /*
        #if UNITY_EDITOR
        Debug.Log("Current State: " + states.currentState);
        #endif  
        */
    }

}