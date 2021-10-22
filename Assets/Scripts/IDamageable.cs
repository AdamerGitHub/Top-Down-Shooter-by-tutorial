using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void CalcResist(float damage, float effectDamage, int damageType, int damageEffectType);

    void TakeHitGraphics(Vector3 hitPoint, Vector3 hitDirection);

    void TakeDamage(float damage, float effectDamage);
}