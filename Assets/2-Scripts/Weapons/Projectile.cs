using UnityEngine;
using System.Collections;

public class Projectile : PoolObject {

	public LayerMask collisionMask;
	public float speed = 100f;
	public float damage = 1f;

	protected float lifeTime = 3f;
	protected float skinWidth = .1f;

    protected ShootingWeapon shootingWeapon;
    public void SetShootingWeapon(ShootingWeapon weapon) { shootingWeapon = weapon; }

    protected virtual void Start()
    {
	}

	public virtual void SetSpeed(float newSpeed){
		speed = newSpeed;
	}

    public override void OnObjectReuse()
    {
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position, -transform.forward);
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

	protected virtual void Update () {
		float moveDistance = speed*Time.deltaTime;
		CheckCollisions(moveDistance);
		transform.Translate(Vector3.forward*moveDistance);
	}

	protected virtual void CheckCollisions(float moveDistance){
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)){
			OnHitObject(hit.collider, hit.point, hit.normal);
		}
	}

	protected virtual void OnHitObject(Collider c, Vector3 hitPoint, Vector3 hitNormal){
		IDamageable damageableObject = c.GetComponent<IDamageable>();
		if (damageableObject != null){
			damageableObject.TakeHit(damage, hitPoint, transform.forward);
		}
	}
}
