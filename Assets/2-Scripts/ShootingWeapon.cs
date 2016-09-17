using UnityEngine;
using System.Collections;
using System;

public class ShootingWeapon : Weapon {

    public Projectile projectile;
    public int ammoAmount = 20;
    [HideInInspector]
    public int currentAmmoAmount;
    public Transform muzzle;
    public float muzzleVelocity = 35f;
    protected float nextShotTime;
    public float msBetweenShots = 100;

    public Animation recoilAnim;
    public string recoilAnimationName = "recoil";
    protected int recoilAnimationHash;

    protected Player player;

    public virtual void Start()
    {
        PoolManager.instance.CreatePool(projectile.gameObject, 30);
        recoilAnimationHash = Animator.StringToHash(recoilAnimationName);
        currentAmmoAmount = ammoAmount;

    }


    public override void Use()
    {
        if (currentAmmoAmount > 0 && Time.time > nextShotTime)
        {
            currentAmmoAmount--;
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = PoolManager.instance.ReuseObject(projectile.gameObject, muzzle.position, muzzle.rotation).GetComponent<Projectile>();

            newProjectile.SetSpeed(muzzleVelocity);
            newProjectile.SetDamage(damage);
            newProjectile.SetShootingWeapon(this);
        }
    }

    public virtual void Reload(int ammoQuantity = 0)
    {
        if (ammoQuantity ==0)
        {
            currentAmmoAmount = ammoAmount;
            return;
        }

        currentAmmoAmount += ammoQuantity;
        currentAmmoAmount = Mathf.Clamp(currentAmmoAmount, 0, ammoAmount);
    }

}
