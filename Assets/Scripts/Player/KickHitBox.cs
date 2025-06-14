using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class KickHitBox : MonoBehaviour
{
    public float activeTime = 0.2f; // Adjust timing here

    private void OnEnable()
    {
        Invoke(nameof(Disable), activeTime);
    }



    void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RatHit rat = other.GetComponent<RatHit>();
        if (rat != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            rat.HitRat(RatHit.HitType.Boot, dir);
        }
    }
}
