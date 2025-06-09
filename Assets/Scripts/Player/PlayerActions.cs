using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStates))]
public class PlayerActions : MonoBehaviour
{
    public int spritzes = 0;
    public float spritzDuration = 1f;
    public float howlDuration = 0.5f;
    public GameObject kickHitbox;

    private PlayerStates states;
    private Animator anim;

    void Awake()
    {
        states = GetComponent<PlayerStates>();
        anim = GetComponent<Animator>();
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
        states.SetState(PlayerState.Kicking);
        anim.SetTrigger("doKick");
        StartCoroutine(DisableHitbox());
    }

    private IEnumerator DisableHitbox()
    {
        kickHitbox.SetActive(true);
        yield return new WaitForEndOfFrame();
        kickHitbox.SetActive(false);
        states.SetState(PlayerState.Idle);
    }
}