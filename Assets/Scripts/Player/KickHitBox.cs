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
}
