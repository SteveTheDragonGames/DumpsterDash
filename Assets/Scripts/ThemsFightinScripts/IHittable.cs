using UnityEngine;

public interface IHittable
{
    void TakeHit(HitType type, Vector2 direction);
}
