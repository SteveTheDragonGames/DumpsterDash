using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStates))]
public class PlayerActions : MonoBehaviour
{
    public int spritzes = 0;
    public float spritzDuration = 1f;
    public float howlDuration = 0.5f;
    public GameObject kickHitBox; // <-- You were missing this!

    private PlayerStates states;
    private Animator anim;
    private KickHitBox kickBoxScript;
    private string KICK_ANIMATION = "isKicking";
    private string HOWL_ANIMATION = "isHowling";
    private string SPRITZ_ANIMATION = "isSpritzing";
    private string HIT_ANIMATION = "isHit";
    private string DEATH_ANIMATION = "isDead";


    void Awake()
    {
        states = GetComponent<PlayerStates>();
        anim = GetComponent<Animator>();

        // Fix: Use the actual GameObject reference
        if (kickHitBox != null)
        {
            kickBoxScript = kickHitBox.GetComponent<KickHitBox>();
        }
        else
        {
            Debug.LogWarning("Kick HitBox not assigned!");
        }
    }

    public void SpritzOrKick()
    {
        if (states.IsBusy() || states.IsState(PlayerState.Dead)) return;
        if (spritzes > 0)
        {
            states.SetState(PlayerState.Attacking);
            if (anim != null) anim.SetBool(SPRITZ_ANIMATION, true);
            spritzes--;
            StartCoroutine(FinishSpritz());
        }
        else
        {
            Kick();
        }
    }

    private IEnumerator FinishSpritz()
    {
        yield return new WaitForSeconds(spritzDuration);
        if (anim != null) anim.SetBool(SPRITZ_ANIMATION, false);
        if (!states.IsState(PlayerState.Dead))
            states.SetState(PlayerState.Idle);
    }

    public void Howl()
    {
        if (states.IsBusy() || states.IsState(PlayerState.Dead)) return;
        states.SetState(PlayerState.Howling);
        if (anim != null) anim.SetBool(HOWL_ANIMATION, true);
        StartCoroutine(FinishHowl());
    }

    private IEnumerator FinishHowl()
    {
        yield return new WaitForSeconds(howlDuration);
        if (anim != null) anim.SetBool(HOWL_ANIMATION, false);
        if (!states.IsState(PlayerState.Dead))
            states.SetState(PlayerState.Idle);
    }

    public void Kick()
    {
        if (states.IsBusy() || states.IsState(PlayerState.Dead)) return;
        if (kickHitBox == null || kickBoxScript == null) return;

        kickHitBox.SetActive(true);
        states.SetState(PlayerState.Kicking);
        if (anim != null) anim.SetTrigger(KICK_ANIMATION);

        StartCoroutine(ResetStateAfterKick(kickBoxScript.activeTime));
    }

    private IEnumerator ResetStateAfterKick(float duration)
    {
        yield return new WaitForSeconds(duration);
        states.SetState(PlayerState.Idle);
    }

    public void Hit()
    {
        if (states.IsBusy() || states.IsState(PlayerState.Dead)) return;
        if (anim != null) anim.SetTrigger(HIT_ANIMATION);
        states.SetState(PlayerState.Stunned);
        StartCoroutine(RecoverFromHit());
    }
    public void Die()
    {
        if (states.IsBusy() || states.IsState(PlayerState.Dead)) return;
        if (anim != null) anim.SetTrigger(DEATH_ANIMATION);
        states.SetState(PlayerState.Dead);
    }

    IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(0.75f);
        if (!states.IsState(PlayerState.Dead))
            states.SetState(PlayerState.Idle);
    }


}


