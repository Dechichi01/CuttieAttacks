﻿using UnityEngine;
using System.Collections;
using Panda;

public class Enemy : LivingEntity {

	public enum State {Idle, Chasing, Attacking};
	public State currentState;

    /// <summary>
    /// Used by the AI BT
    /// </summary>

    [Task]
    public bool playerInSight { get { return (!isPatrolEnemy) || (!isAttacking && !isFending && FOV.visibleTargets.Count > 0);   }    }
    //[Task]
    //public bool chasingPlayer { get { return isChasing; } }
    [Task]
    public bool canAttack { get
        {
            if (hasTarget && !isFending)
            {
                if (Time.time > nextAttackTime)
                {
                    float sqrDstToTarget = (playerTarget.thisTransform.position - transform.position).sqrMagnitude; //take the distance between two positions in sqrMagnitude
                    float radius = Mathf.Pow(attackDistanceThreshold + myCollisionRadius + playerTarget.radius, 2);
                    if (sqrDstToTarget <= radius)
                    {
                        nextAttackTime = Time.time + timeBetweenAttacks;
                        return true;
                    }
                }
            }
            return false;
        }
    }

	NavMeshAgent navAgent;

	Target playerTarget;

    FieldOfView FOV;

	LivingEntity targetLivingEntity;
	Material skinMaterial;

    public Pickup[] dropPickups;
    public Rope rope;
    public ParticleSystem deathEffect;

    public static event System.Action OnDeathStatic;//Used by the ScoreKeeper

	float damage = 1f;

	public float attackDistanceThreshold = .5f;
    public float refreshRate = 0.2f;
	float timeBetweenAttacks = 1;

	float nextAttackTime;
	float myCollisionRadius;

    public bool hasTarget;
    bool isAttacking;
    bool isFending;
    [Task]
    public bool isChasing;
    public bool isPatrolEnemy = false;

    void Awake()
    {
        FOV = GetComponent<FieldOfView>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = false;

        myCollisionRadius = GetComponent<CapsuleCollider>().radius;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            Transform playerT = GameObject.FindGameObjectWithTag("Player").transform;
            playerTarget = new Target("Player", playerT, playerT.GetComponent<CapsuleCollider>().radius);
            targetLivingEntity = playerTarget.thisTransform.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }
    }

	override protected void Start () {
		base.Start();
	}

    [Task]
    public bool StartChase()
    {
        StopAllCoroutines();
        isChasing = true;
        navAgent.enabled = true;
        if (hasTarget)
        {
            currentState = State.Chasing;
            targetLivingEntity.OnDeath += OnTargetDeath; //That's how we subscribe a method to a System.Action method (OnDeath)

            StartCoroutine(UpdatePath());
        }
        return true;
    }

    [Task]
    public bool StartWatch()
    {
        StopAllCoroutines();
        isChasing = false;
        StartCoroutine(Watch());
        return true;
    }

    [Task]
    bool Attack()
    {
        AudioManager.instance.PlaySound("Enemy Attack", transform.position);

        StartCoroutine(AttackPlayer());
        return true;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection, float amountToFend = 0)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health)
        {
            if (OnDeathStatic != null)
                OnDeathStatic();
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward,hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
        StartCoroutine(Fend(amountToFend));
    }

    public override void Die()
    {
        Instantiate(dropPickups[Random.Range(0, dropPickups.Length)], transform.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        base.Die();
    }

    void OnTargetDeath(){
        StopAllCoroutines();
		hasTarget = false;
        targetLivingEntity.OnDeath -= OnTargetDeath;//Good practice to unsubscribe from a method
		currentState = State.Idle;
	}

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth)
    {
        navAgent.speed = moveSpeed;
        if (hasTarget)
            damage = Mathf.Ceil(targetLivingEntity.startingHealth / hitsToKillPlayer);

        startingHealth = enemyHealth;

    }

    protected override void Destroy(float delay = 0)
    {
        navAgent.enabled = false;
        isChasing = false;
        PandaBehaviour panda = GetComponent<PandaBehaviour>();
        panda.Reset();
        base.Destroy(delay);
    }

    IEnumerator AttackPlayer(){

        isAttacking = true;
		currentState = State.Attacking;
		navAgent.enabled = false;

        Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (playerTarget.thisTransform.position - transform.position).normalized;
		Vector3 attackPosition = playerTarget.thisTransform.position - dirToTarget*(myCollisionRadius);

		float attackSpeed = 3;
		float percent = 0;
		bool hasAppliedDamage = false;


		while (percent <= 1){
			if (playerTarget.name == "Player" && percent >= 0.5 && !hasAppliedDamage){
				hasAppliedDamage = true;
				targetLivingEntity.TakeDamage(damage);
			}

			percent += Time.deltaTime*attackSpeed;
			float interpolation = (-Mathf.Pow(percent,2) + percent)*4;
			transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

			yield return null;
		}

		//skinMaterial.color = originalColour;
		currentState = State.Chasing;
		navAgent.enabled = true;
        isAttacking = false;
	}

    IEnumerator UpdatePath(){            

        while (hasTarget){
            Debug.Log("Ocorrendo");
			if (currentState == State.Chasing){
				Vector3 dirToTarget = (playerTarget.thisTransform.position - transform.position).normalized;
				Vector3 targetPosition = playerTarget.thisTransform.position - dirToTarget*(myCollisionRadius + playerTarget.radius/2 + attackDistanceThreshold/2);
				if (!dead){
					navAgent.SetDestination(targetPosition);	
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}

    IEnumerator Watch()
    {
        float percent = 0;
        float turnTime = 0.6f;
        Quaternion start, end;
        while (true)
        {
            start = transform.rotation;
            end = transform.rotation * Quaternion.Euler(0f, 90f, 0f);

            percent = 0;
            while (percent < 1)
            {
                percent += ((1 / turnTime) * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(start, end, percent);
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator Fend(float amountToFend)
    {
        if (amountToFend == 0)
            yield break;

        Vector3 start = transform.position;
        Vector3 end = start - (playerTarget.thisTransform.position - start).normalized*amountToFend;
        float percent = 0;
        float fendSpeed = 1 / 0.2f;

        isFending = true;
        while (percent < 1)
        {
            percent += fendSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, percent);
            yield return null;
        }
        isFending = false;
    }
}

public class Target
{
    public string name;
    public Transform thisTransform;
    public float radius;

    public Target(string name, Transform thisTransform, float radius)
    {
        this.name = name;
        this.thisTransform = thisTransform;
        this.radius = radius;
    }
}

