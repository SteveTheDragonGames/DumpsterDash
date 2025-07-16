using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStates))]
public class PlayerSearch : MonoBehaviour
{
    public float searchDuration = 1.5f;
    public SignAnimator signAnimator;

    private PlayerStates states;
    private bool isNearDumpster;
    private Transform currentDumpsterLocation;
    private Coroutine searchRoutine;
    private Dumpster currentDumpster;
    private int level = 1;


    void Awake()
    {
        states = GetComponent<PlayerStates>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Dumpster"))
        {
            isNearDumpster = true;
            currentDumpsterLocation = col.transform;
            currentDumpster = currentDumpsterLocation.GetComponent<Dumpster>();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Dumpster"))
        {
            isNearDumpster = false;
            currentDumpsterLocation = null;
            currentDumpster = null;
        }
    }

    public void Search()
    {
        if (!isNearDumpster || states.IsBusy()) return;
        // Snap to dumpster
        Vector3 pos = transform.position;
        currentDumpster.OpenLid();
        transform.position = new Vector3(currentDumpsterLocation.position.x, pos.y, pos.z);
        // Start search
        states.SetState(PlayerState.Searching);
        searchRoutine = StartCoroutine(FinishSearch());
    }

    private IEnumerator FinishSearch()
    {
        yield return new WaitForSeconds(searchDuration);
        if (currentDumpster == null)
        {
            states.SetState(PlayerState.Idle);
            yield break;
        }

        float spawnChance = Random.value;
        if (spawnChance < 0.8f)
        {
            var junk = currentDumpster.GetRandomJunk(level);
            var go = Instantiate(junk.junkItem, currentDumpsterLocation.position + Vector3.up, Quaternion.identity);
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb) rb.AddForce(new Vector2(Random.Range(-3f, 3f), 5f), ForceMode2D.Impulse);

            if (signAnimator != null)
                signAnimator.PopSign(junk.name, junk.description);
            else
                Debug.LogWarning("SignAnimator not hooked up!");


            
        }
        else
        {
            var prefab = currentDumpster.racoon;
            float spawnX = Random.value < 0.5f ? -2f : 2f;

            var r = Instantiate(prefab, currentDumpsterLocation.position + new Vector3(spawnX, 1f, 0), Quaternion.identity);

            // Set aggression on the actual instance
            var ai = r.GetComponent<RaccoonAI>();
            if (ai != null)
                ai.AggressionChance = 0.1f + (level * 0.1f);

            // Add launch force
            var rb = r.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(Random.Range(-1.5f, 1.5f), 10f), ForceMode2D.Impulse);
            }
        }


        yield return new WaitForSeconds(0.2f);
        currentDumpster.CloseLid();
        states.SetState(PlayerState.Idle);
    }
}
