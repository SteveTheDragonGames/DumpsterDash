using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float _destroyTime = 1f;

    void Start()
    {
        Invoke(nameof(DestroySelf), _destroyTime);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}