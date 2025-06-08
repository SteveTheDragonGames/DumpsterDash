using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickHitBox : MonoBehaviour
{
    public float activeTime = 0.2f; // Adjust timing here

    private void OnEnable()
    {
        Invoke(nameof(Disable), activeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rat"))
        {
            // Knockback, damage, or destroy rat
            Destroy(other.gameObject); // Or trigger animation
            Debug.Log("Rat booted!");
        }
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}
