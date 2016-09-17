using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Player startPlayer;
    public Canvas canvas;
    public int currentWeaponAmmo
    {
        get
        {
            return ((ShootingWeapon)playerCurrentWeapon).currentAmmoAmount;
        }
    }

    public Player currentPlayer { get; set; }
    public Weapon playerCurrentWeapon { get; set; }
    private MapGenerator mapGen;
    private ControllersUI controllersUI;
    private GameUI gameUI;

    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

	void Awake()
    {
        //Find map, player, and equip weapon
        mapGen = FindObjectOfType<MapGenerator>().GetComponent<MapGenerator>();
        Map map = mapGen.map;
        currentPlayer = (Player) Instantiate(startPlayer, mapGen.CoordToPosition(map.mapSize.x / 2, 2), Quaternion.identity);
        WeaponManager weaponManager = currentPlayer.transform.GetComponent<WeaponManager>();
        playerCurrentWeapon = weaponManager.EquipWeapon(weaponManager.startingWeaponIndex);
        //playerCurrentWeapon = weaponManager.allWeapons[weaponManager.startingWeaponIndex];

        //Set up UI
        canvas = FindObjectOfType<Canvas>();
        controllersUI = canvas.GetComponent<ControllersUI>();
        gameUI = canvas.GetComponent<GameUI>();
        gameUI.SetWeaponImage(playerCurrentWeapon.UI_image);

        //Setup audio
        Transform audioManager = FindObjectOfType<AudioManager>().transform;
        audioManager.position = currentPlayer.transform.position;
        audioManager.rotation = currentPlayer.transform.rotation;
        audioManager.parent = currentPlayer.transform;

        //Set up controllers
        SwipeDetector swipeDetect = currentPlayer.GetComponent<SwipeDetector>();
        swipeDetect.aimJoystickRect = controllersUI.AimJoysticRect;
        swipeDetect.attackJoystickRect = controllersUI.AttackJoystickRect;
        swipeDetect.changeWeaponJoystickRect = controllersUI.ChangeWeaponRect;
        FindObjectOfType<CameraController>().GetComponent<CameraController>().playerT = currentPlayer.transform;
        
    }
	
}
