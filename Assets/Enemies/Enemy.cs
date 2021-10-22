using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    [Header("Damage Settings")]
    public int damage = 1;
    public int effectDamage = 0;
    public int damageType = 0;
    public int DamageStatEffect = 0;

    [Header("Enemy Properties")]
    public bool navMeshAgentMovement;
    public float moveSpd;
    public float moveMaxSpd;

    [Header("Common Settings")]
    public int goldOnDeath = 10;

    public enum State { Idle, Chasing, Attacking };
    [HideInInspector] public State currentState;

    [Header("Effects")]
    public ParticleSystem deathEffect;

    // Get the Enemy references
    NavMeshAgent pathfinder;

    // Get an important Player references
    LivingEntity targetEntity;
    PlayerStats playerStats;
    Transform target;

    // Token Collor System
    Renderer skinChildMaterial;
    Color[] skinChildOriginalColors;

    // Attack Settings
    public float attackDistanceThreshold = 0.5f;
    public float timeBetweenAttacks = 1f;
    public float myCollisionRadius = 0.5f; // Token. Because a CapsuleCollider.radius used to set this value (and I remove CapsuleCollider), I set it manually

    // Some settings
    float nextAttackTime;
    // float myCollisionRadius = 0.5f; // Token. Because a CapsuleCollider.radius used to set this value (and I remove CapsuleCollider), I set it manually
    float targetCollisionRadius;

    bool hasTarget;

    bool updatePathStart;

    void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        skinChildMaterial = transform.GetChild(0).GetComponent<Renderer>(); // Token
        skinChildOriginalColors = new Color[skinChildMaterial.materials.Length]; // Token
        for (int i = 0; i < skinChildMaterial.materials.Length; i++) // Token, save the all original colors
        {
            skinChildOriginalColors[i] = skinChildMaterial.materials[i].GetColor("_BaseColor");
        }

        OnDeath += GiveGoldToPlayer;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            //Token. CapsuleCollider removed
            // myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
        else
        {
            OnTargetDeath();
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;
        }
    }

    // Token. Applies spicific color to the all materials (by for loop)
    // If setOriginal = true, then make all colors to original color
    protected virtual void ApplyColorToSkin(Color color, bool setOriginal)
    {
        if (setOriginal)
        {
            for (int i = 0; i < skinChildMaterial.materials.Length; i++)
            {
                skinChildMaterial.materials[i].SetColor("_BaseColor", skinChildOriginalColors[i]);
            }

            return;
        }

        for (int i = 0; i < skinChildMaterial.materials.Length; i++)
        {
            skinChildMaterial.materials[i].SetColor("_BaseColor", color);
        }
    }

    public override void CalcResist(float damage, float effectDamage, int damageType, int damageEffectType)
    {
        base.CalcResist(damage, effectDamage, damageType, damageEffectType);
    }

    public override void TakeHitGraphics(Vector3 hitPoint, Vector3 hitDirection)
    {
        if (health <= 0)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
        }
        base.TakeHitGraphics(hitPoint, hitDirection);
    }

    // Give to the player money
    public void GiveGoldToPlayer()
    {
        playerStats.ChangeCurrentGold(goldOnDeath);
    }

    // If Player die, currentState = idle (Or stand on position)
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        /*
        if (!hasTarget)
        {
            pathfinder.enabled = false;
            // Idle = Stop enemy movement or attacking
            currentState = State.Idle;
        }
        */
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2f))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }

            // Move without NavMeshAgent
            // navMeshAgentMovement if false need to make pathfinder to disabled on movement (or everywere else)
            if (navMeshAgentMovement == false && currentState == State.Chasing)
            {
                pathfinder.enabled = false;
                MoveToPlayer();
            }

            // Move with NavMeshAgent
            if (navMeshAgentMovement && !updatePathStart && currentState == State.Chasing)
            {
                pathfinder.enabled = true;
                StartCoroutine(UpdatePath());
                updatePathStart = true;
            }
        }
    }
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        if (navMeshAgentMovement)
        {
            pathfinder.enabled = false;
        }

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3f;
        float percent = 0;

        ApplyColorToSkin(Color.red, false); // Token, set all colors to red
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.CalcResist(damage, effectDamage, damageType, DamageStatEffect);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        ApplyColorToSkin(Color.black, true); // Token set all colors to original
        currentState = State.Chasing;
        if (navMeshAgentMovement)
        {
            pathfinder.enabled = true;
        }
    }

    public IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        if (!navMeshAgentMovement)
        {
            pathfinder.enabled = false;
            updatePathStart = false;
            yield break;
        }

        // If has target (looping) and State.Chasing then move to the target (calculated)
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);

                if(pathfinder.isOnNavMesh == true)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    void MoveToPlayer()
    {
        if(currentState == State.Chasing)
        {
            if(moveMaxSpd >= moveSpd)
            {
                moveSpd += Time.deltaTime;
            }

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpd);
            //transform.Translate((-dirToTarget) * moveSpd * Time.deltaTime);
            transform.position += ((dirToTarget) * moveSpd * Time.deltaTime);
            transform.LookAt(target, Vector3.up);
        }
    }
}