using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Jumping,
    Falling,
    Searching,
    Attacking,
    Kicking,
    Howling,
    Stunned
}

public class PlayerStates : MonoBehaviour
{
    public PlayerState currentState = PlayerState.Idle;

    public void SetState(PlayerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }

    public bool IsState(PlayerState state) => currentState == state;
    public bool CanMove() =>
    currentState == PlayerState.Idle ||
    currentState == PlayerState.Moving ||
    currentState == PlayerState.Jumping ||
    currentState == PlayerState.Falling;

    public bool CanAttack() =>
    currentState == PlayerState.Idle ||
    currentState == PlayerState.Moving ||
    currentState == PlayerState.Jumping;


    public bool CanJump() => (currentState == PlayerState.Idle || currentState == PlayerState.Moving);
    public bool CanSearch() => (currentState == PlayerState.Idle || currentState == PlayerState.Moving);
    public bool IsBusy() => currentState == PlayerState.Searching || currentState == PlayerState.Howling || currentState == PlayerState.Stunned;
}