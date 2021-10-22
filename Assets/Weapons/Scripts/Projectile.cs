using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(Renderer))]
public class Projectile : MonoBehaviour
{
    [Header("Bullet")]
    public float damage = 1;
    public float effectDamage = 0;
    public float damageWidth = .1f;

    // Light DamageType
    [Header("Bullet Type")]
    public int damageType;
    public int damageEffectType;

    [Header("Lifetime")]
    public float lifetime = 3f;
    public float deathLifetime = 1f;

    Renderer projectileRenderer;
    Color initialColour;
    float rendererPercent = 0f;
    TrailRenderer trailRenderer;
    Color trailInitialColour;
    float trailPercent;

    Color fadeColour = Color.clear;

    [Header("Common")]
    public LayerMask collisionMask;
    public Color trailColour;

    float speed = 10f;

    void Start()
    {
        projectileRenderer = GetComponent<Renderer>();
        initialColour = projectileRenderer.material.GetColor("_BaseColor");
        trailRenderer = GetComponent<TrailRenderer>();
        trailInitialColour = trailRenderer.material.GetColor("_BaseColor");

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColour);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            FadeBullet();
        }
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + damageWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.CalcResist(damage, effectDamage, damageType, damageEffectType);

            // Give a Hitpoint, and a transform rotation forward. When an enemy destroy, he's will spawning an epic Particle Effect
            damageableObject.TakeHitGraphics(hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }

    void FadeBullet()
    {
        rendererPercent += Time.deltaTime / deathLifetime;
        trailPercent += Time.deltaTime / (deathLifetime);

        float rendererInterpolation = (Mathf.Pow(rendererPercent, 2) + rendererPercent) * 4;
        projectileRenderer.material.SetColor("_BaseColor", Color.Lerp(initialColour, fadeColour, rendererInterpolation));

        float trailInterpolation = (Mathf.Pow(trailPercent, 2) + trailPercent) * 4;
        trailRenderer.material.SetColor("_BaseColor", Color.Lerp(trailInitialColour, fadeColour, trailInterpolation));

        if (projectileRenderer.material.GetColor("_BaseColor") == fadeColour)
        {
            Destroy(gameObject);
        }
    }
}
