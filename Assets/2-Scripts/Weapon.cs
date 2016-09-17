using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {

    public Sprite UI_image;
    public int weaponNum;
    public float yPlayerRot = 0;
    protected float damage = 1f;
    protected Transform owner;

    public abstract void Use();

    public virtual void SetupPlayer()
    {
        Vector3 ownerRot = owner.GetChild(0).localRotation.eulerAngles;
        owner.GetChild(0).localRotation = Quaternion.Euler(ownerRot.x, yPlayerRot, ownerRot.z);
    }

    public virtual void Equip(Transform _owner, Transform holder)
    {
        owner = _owner;
        Vector3 previousPos = transform.position;
        Quaternion previousRot = transform.rotation;
        transform.parent = holder;
        transform.localPosition = previousPos;
        transform.localRotation = previousRot;
    }

}
