using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RifleProjectile : Projectile
{
    [HideInInspector] public Rifle rifle;
    [HideInInspector] public int numBounceRemaining;
    [HideInInspector] public float bounceBonus;
    [SerializeField] public float bounceMultiplyer;  //velocity multiplier everytime rifle bounces

    public override void OnTriggerEnter(Collider other)
    {
        if (numBounceRemaining >= 1)
        {
            Ray ray = new Ray(transform.position, rb.velocity);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                rb.velocity = Vector3.Reflect(rb.velocity, hit.normal) * bounceMultiplyer;
            }
            else
            {
                rb.velocity = -rb.velocity * bounceMultiplyer;
            }
            damage += bounceBonus;
            numBounceRemaining--;
        }
        else
        {
            base.OnTriggerEnter(other);
        }

    }

    /**
     * When rifle boomerang destroyed, set rifle active again and add bonus ammo
     */
    private void OnDestroy()
    {
        if (view.IsMine)
        {
            rifle.ammo += (rifle.boomerBounce-numBounceRemaining)*rifle.bounceAmmoBonus;
        }
        rifle.RifleActive(true);
    }


}
