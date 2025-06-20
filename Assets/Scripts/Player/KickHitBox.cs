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
        // Existing rat logic
        RatHit rat = other.GetComponent<RatHit>();
        if (rat != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            rat.HitRat(RatHit.HitType.Boot, dir);
            UnityEngine.Debug.Log("Kick hit! Direction: " + dir + ", Target: " + other.name);
            return;
        }

        // New raccoon logic
        RaccoonAI raccoon = other.GetComponent<RaccoonAI>();
        if (raccoon != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            raccoon.TakeHit(1); // Or pass in a damage value if variable
            UnityEngine.Debug.Log("Raccoon kick hit! Direction: " + dir + ", Target: " + other.name);
        }
    }
}
