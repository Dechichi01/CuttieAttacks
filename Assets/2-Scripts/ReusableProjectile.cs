using UnityEngine;
using System.Collections;

public class ReusableProjectile : Projectile {

    bool inactive, isFalling;
    public LayerMask floorMask;

    protected override void Update()
    {
        if (!inactive)
        {
            float moveDistance = speed * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);
        }
    }

    public override void OnObjectReuse()
    {
        inactive = false;
        base.OnObjectReuse();
    }

    protected override void OnHitObject(Collider c, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnHitObject(c, hitPoint, hitNormal);
        inactive = true;

        RaycastHit hit;
        if (Physics.Raycast(hitPoint, Vector3.down, out hit, 10, floorMask))
        {
            Vector3 initialPos = transform.position;
            hitNormal = Quaternion.Euler(0f, Random.Range(-45f, 45f), 0f)*hitNormal;
            Debug.DrawRay(transform.position, hitNormal, Color.red, 50);
            Vector3 finalPos = new Vector3(transform.position.x + hitNormal.x, hit.point.y, transform.position.z + hitNormal.z);
            StartCoroutine(FallDown(initialPos, finalPos));
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (inactive && !isFalling && col.CompareTag("Player"))
        {
            shootingWeapon.Reload(1);
            Destroy();
        }
    }

    IEnumerator FallDown(Vector3 initialPos, Vector3 finalPos)
    {
        isFalling = true;

        float fallPercent = 0;
        float reactionPercent = 0;
        float reactionVel = 3 / 0.4f;
        float gravity = 1 / 0.4f;

        Vector3 tempPos;
        Quaternion initialRot = transform.rotation;
        Quaternion finalRot = Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        while (fallPercent < 1)
        {
            fallPercent += Time.deltaTime * gravity;
            reactionPercent += Time.deltaTime * reactionVel;

            tempPos = Vector3.Lerp(initialPos, finalPos, reactionPercent);
            transform.position = new Vector3(tempPos.x, Mathf.Lerp(initialPos.y, finalPos.y, fallPercent), tempPos.z);
            transform.rotation = Quaternion.Lerp(initialRot, finalRot, gravity);
            yield return null;
        }
        isFalling = false;
        transform.position = finalPos;
        
    }
}
