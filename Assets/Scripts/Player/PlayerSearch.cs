using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStates))]
public class PlayerSearch : MonoBehaviour
{
    public float searchDuration = 1.5f;
    public SignAnimator signAnimator;

    private PlayerStates states;
    private bool isNearDumpster;
    private Transform currentDumpster;
    private Coroutine searchRoutine;

    void Awake()
    {
        states = GetComponent<PlayerStates>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Dumpster"))
        {
            isNearDumpster = true;
            currentDumpster = col.transform;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Dumpster"))
        {
            isNearDumpster = false;
            currentDumpster = null;
        }
    }

    public void Search()
    {
        if (!isNearDumpster || states.IsBusy()) return;
        // Snap to dumpster
        Vector3 pos = transform.position;
        transform.position = new Vector3(currentDumpster.position.x, pos.y, pos.z);
        // Start search
        states.SetState(PlayerState.Searching);
        searchRoutine = StartCoroutine(FinishSearch());
    }

    private IEnumerator FinishSearch()
    {
        yield return new WaitForSeconds(searchDuration);

        var ds = currentDumpster.GetComponent<Dumpster>();
        if (ds == null)
        {
            states.SetState(PlayerState.Idle);
            yield break;
        }

        float spawnChance = Random.value;
        if (spawnChance < 0.8f)
        {
            var junk = ds.GetRandomJunk();
            var go = Instantiate(junk.junkItem, currentDumpster.position + Vector3.up, Quaternion.identity);
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb) rb.AddForce(new Vector2(Random.Range(-3f, 3f), 5f), ForceMode2D.Impulse);

            signAnimator.PopSign(junk.name, junk.description);
        }
        else
        {
            var prefab = ds.racoon;
            float spawnX = Random.value < 0.5f ? -2f : 2f;
            var r = Instantiate(prefab, currentDumpster.position + new Vector3(spawnX, 1f, 0), Quaternion.identity);
            var rb = r.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(Random.Range(-1.5f, 1.5f), 10f), ForceMode2D.Impulse);
            }
        }

        ds.CloseLid();
        states.SetState(PlayerState.Idle);
    }
}
