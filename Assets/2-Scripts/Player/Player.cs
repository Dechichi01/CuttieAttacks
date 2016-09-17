using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (WeaponManager))]
public class Player : LivingEntity {

	public float moveSpeed = 5f;
    [HideInInspector]
    public Vector3 moveVelocity;//Used by the Spawner

    private PlayerController controller;
	private WeaponManager weaponManager;
	private Camera viewCamera;
    private SwipeDetector swipeControl;
    private Animator anim;
    private Vector3 aimVelocity;

    private Transform currentVisibleTarget;
    private Transform lastVisibleTarget;

    private float recoverDelay = 3;
    private float recoverTime;

    public SpawnRect spawnRect;

	override protected void Start () {
		base.Start();
		controller = GetComponent<PlayerController>();
		weaponManager = GetComponent<WeaponManager>();
		swipeControl = GetComponent<SwipeDetector>();
        anim = GetComponentInChildren<Animator>();
		viewCamera = Camera.main;
        spawnRect.SetOwner(transform);

        anim.SetInteger("weaponNum", weaponManager.EquipWeapon(weaponManager.startingWeaponIndex).weaponNum);
	}

    void Update()
    {
        GetInputAndMove();
        HandleTouchInput();
        controller.Aim();      
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    void GetInputAndMove ()
	{
        //TODO: Add Blend tree and fix that
        float xInput = CrossPlatformInputManager.GetAxisRaw("HorzLeft");
        float yInput = CrossPlatformInputManager.GetAxisRaw("VertLeft");
        anim.SetFloat("speed", xInput);

        Vector3 moveInput = new Vector3 (xInput, 0, yInput);
        moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);
	}

    void HandleTouchInput()
    {
        anim.SetBool("isShooting", false);
        switch (swipeControl.GetSwipeDirection())
        {
    	    case SwipeDetector.SwipeDirection.Right:
			    controller.Rotate(new Vector3(0f,90f,0f));
			    break;
            case SwipeDetector.SwipeDirection.Left:
			    controller.Rotate(new Vector3(0f,-90f,0f));
			    break;
            case SwipeDetector.SwipeDirection.Jump:
                controller.Rotate(new Vector3(0f, 90f, 0f));
                break;
            case SwipeDetector.SwipeDirection.Duck:
                controller.Rotate(new Vector3(0f, -90f, 0f));
                break;
            case SwipeDetector.SwipeDirection.ChangeWeapon:
                anim.SetInteger("weaponNum", weaponManager.SwitchToNextWeapon());
                break;
            case SwipeDetector.SwipeDirection.Attack:
                anim.SetBool("isShooting", true);
                weaponManager.Use();
                break;
    	}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("isShooting", true);
            weaponManager.Use();
        }

        if (Input.GetKeyDown(KeyCode.Z))
            controller.Rotate(new Vector3(0f, -90f, 0f));
        if (Input.GetKeyDown(KeyCode.X))
            controller.Rotate(new Vector3(0f, 90f, 0f));
        if (Input.GetKeyDown(KeyCode.C))
            anim.SetInteger("weaponNum", weaponManager.SwitchToNextWeapon());
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("DoorEntrance") || col.CompareTag("DoorExit"))
        {
            col.transform.parent.GetComponent<RoomDoor>().OpenDoor();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("DoorEntrance") || col.CompareTag("DoorExit"))
        {
            col.transform.parent.GetComponent<RoomDoor>().CloseDoor();
        }
    }

    void OnDrawGizmosSelected()
    {
        spawnRect.SetOwner(transform);
        spawnRect.DrawRectangle();
    }

}
