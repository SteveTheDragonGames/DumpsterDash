using System.Collections;
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
        IHittable hittable = other.GetComponent<IHittable>();
        if (hittable != null)
        {
            Vector2 hitDir = (other.transform.position - transform.position).normalized;
            hittable.TakeHit(HitType.Boot, hitDir);
        }
    }
}
