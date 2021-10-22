using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public int startingHealth;
    protected int health;
    protected bool dead;

    [Header("Damage Resists")]
    public TypesDMGResist[] typesDMGResist;
    public EffectsResist[] effectsResist;

    string damageTypeSave;
    string damageStatEffectSave;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void CalcResist(float damage, float effectDamage, int damageType, int damageEffectType)
    {
        int dmgTypeResist = typesDMGResist[damageType].percentDamageResist;
        float _damage = damage * dmgTypeResist / 100;

        int effectTypeResist = effectsResist[damageEffectType].percentEffectResist;
        float _effectDamage = effectDamage * effectTypeResist / 100;

        TakeDamage(_damage, _effectDamage);
    }

    public virtual void TakeDamage(float _damage, float _effectDamage)
    {
        health -= (int)(_damage + _effectDamage);

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void TakeHitGraphics(Vector3 hitPoint, Vector3 hitDirection)
    {
        // Do some Stuff here with hit var
        //TakeDamage(damage);
    }

    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

    [System.Serializable]
    public class TypesDMGResist
    {
        [SerializeField] string inspectorDamageResist;

        public int percentDamageResist = 100;
    }

    [System.Serializable]
    public class EffectsResist
    {
        [SerializeField] string inspectorEffectResist;

        public int percentEffectResist = 100;
    }
}
