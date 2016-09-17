using UnityEngine;
using System.Collections;
using System;

public class Gun : ShootingWeapon {

    //public int projectilesPerMag = 6;

    private bool isReloading;
    public float reloadTime = .3f;

    [Header ("Effects")]
    public Transform shell;
    public Transform shellEjection;
    private MuzzleFlash muzzleFlash;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    public override void Start()
    {
        base.Start();
        muzzleFlash = GetComponent<MuzzleFlash>();
        //PoolManager.instance.CreatePool(projectile.gameObject, 30);
        PoolManager.instance.CreatePool(shell.gameObject, 30);
    }

    void LateUpdate()
    {
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

            //PoolManager.instance.ReuseObject(shell.gameObject, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();

            AudioManager.instance.PlaySound(shootAudio, transform.position);

        }
	}


}
