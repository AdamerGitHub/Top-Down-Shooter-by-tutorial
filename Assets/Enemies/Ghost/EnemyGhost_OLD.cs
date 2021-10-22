using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EnemyGhost_OLD : LivingEntity
{
    public enum State { Idle, Chasing, Attacking };
    State currentState;

    public ParticleSystem deathEffect;

    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    // Token
    Renderer skinChildMaterial;
    Color[] skinChildOriginalColors;

    Color originalColour;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1f;
    int damage = 1;
    int effectDamage = 0;

    public float speed = 3f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;
    bool isEnabled;

    // Token. Applies spicific color to the all materials (by for loop)
    // If setOriginal = true, then make all colors to original color
    void ApplyColorToSkin(Color color, bool setOriginal)
    {
        if (setOriginal)
        {
            for (int i = 0; i < skinChildMaterial.materials.Length; i++)
            {
                skinChildMaterial.materials[i].color = skinChildOriginalColors[i];
            }

            return;
        }

        for (int i = 0; i < skinChildMaterial.materials.Length; i++)
        {
            skinChildMaterial.materials[i].color = color;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        skinChildMaterial = transform.GetChild(0).GetComponent<Renderer>(); // Token
        skinChildOriginalColors = new Color[skinChildMaterial.materials.Length]; // Token
        for (int i = 0; i < skinChildMaterial.materials.Length; i++) // Token, save the all original colors
        {
            skinChildOriginalColors[i] = skinChildMaterial.materials[i].color;
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            isEnabled = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            //StartCoroutine(UpdatePath());
        }
    }
    /*
    public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }
    */
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Rotate();

        if (hasTarget && Time.time > nextAttackTime)
        {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2f))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }

    }
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        isEnabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3f;
        float percent = 0;

        ApplyColorToSkin(new Color(1, 0, 0, 0.4f), false); // Token, set all colors to red
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage, effectDamage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        ApplyColorToSkin(Color.black, true); // Token set all colors to original
        currentState = State.Chasing;
        isEnabled = true;
    }

    void Move()
    {
       if (hasTarget && currentState == State.Chasing && isEnabled)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    void Rotate()
    {
        if(hasTarget)
        {
            transform.LookAt(target, Vector3.up);
        }
    }
}
    /*
    IEnumerator UpdatePath()
    {

    }
    */
