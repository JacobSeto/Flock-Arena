using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour, IDamageable
{
    //External collider that takes in damage for main damage gameobject
    public Transform damageTransform;  //Transform of the Damageable game object
    [SerializeField] float damageMultiplyer;

    public Transform DamageTransform()
    {
        return damageTransform;
    }
    public void TakeDamage(float damage)
    {
        damageTransform.GetComponent<IDamageable>().TakeDamage(damage * damageMultiplyer);
    }
}
