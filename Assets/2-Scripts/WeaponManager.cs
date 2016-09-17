using UnityEngine;
using System.Collections;
using System;

public class WeaponManager : MonoBehaviour {

    public Weapon[] allWeapons;
    public int startingWeaponIndex;
    private Weapon[] allWeaponsInstances;
    Weapon equippedWeapon;
    int equippedWeaponIndex;

    public Transform player;
    public Transform weaponHold;

	void Awake(){
        allWeaponsInstances = new Weapon[allWeapons.Length];
        for (int i = 0; i < allWeapons.Length; i++)
        {
            allWeaponsInstances[i] = Instantiate(allWeapons[i]);
            allWeaponsInstances[i].Equip(player, weaponHold);
            allWeaponsInstances[i].gameObject.SetActive(false);
        }
	}

	public Weapon EquipWeapon(int index){
		if (equippedWeapon != null){
            equippedWeapon.gameObject.SetActive(false);
		}
        equippedWeapon = allWeaponsInstances[index%allWeaponsInstances.Length];
        equippedWeapon.SetupPlayer();
        equippedWeaponIndex = index;
        equippedWeapon.gameObject.SetActive(true);

        return equippedWeapon;
	}

    public int SwitchToNextWeapon()
    {
        return EquipWeapon(equippedWeaponIndex+1).weaponNum;
    }

	public void Use(){
		if (equippedWeapon != null){
			equippedWeapon.Use();
		}
	}
}
