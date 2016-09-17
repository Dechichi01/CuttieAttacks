using UnityEngine;
using System.Collections;

public class Bullet : Projectile {

    protected override void Start()
    {
        Destroy(lifeTime);
    }

    protected override void OnHitObject(Collider c, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnHitObject(c, hitPoint, hitNormal);
        Destroy();
    }
}
