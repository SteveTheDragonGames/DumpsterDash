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
        if (spritzes > 0)
        {
            states.SetState(PlayerState.Attacking);
            anim.SetBool("isSpritzing", true);
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
        anim.SetBool("isSpritzing", false);
        states.SetState(PlayerState.Idle);
    }

    public void Howl()
    {
        states.SetState(PlayerState.Howling);
        anim.SetBool("Howl", true);
        StartCoroutine(FinishHowl());
    }

    private IEnumerator FinishHowl()
    {
        yield return new WaitForSeconds(howlDuration);
        anim.SetBool("Howl", false);
        states.SetState(PlayerState.Idle);
    }

    public void Kick()
    {
        if (kickHitBox == null || kickBoxScript == null) return;

        kickHitBox.SetActive(true);
        states.SetState(PlayerState.Kicking);
        anim.SetTrigger("doKick");

        StartCoroutine(ResetStateAfterKick(kickBoxScript.activeTime));
    }

    private IEnumerator ResetStateAfterKick(float duration)
    {
        yield return new WaitForSeconds(duration);
        states.SetState(PlayerState.Idle);
    }
}
