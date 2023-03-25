using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RifleProjectile : Projectile
{
    [HideInInspector] public Rifle rifle;
    [HideInInspector] public int numBounce;
    [HideInInspector] public float bounceBonus;
    [SerializeField] public float bounceMultiplyer;  //velocity multiplier everytime rifle bounces

    public override void OnTriggerEnter(Collider other)
    {
        if (view.IsMine && !hit)
        {
            if (other.gameObject.GetComponentInParent<IDamageable>() != null)
            {
                other.gameObject.GetComponentInParent<IDamageable>().TakeDamage(damage);
                DestroyProjectile(0);
                hit = true;
            }
            else if(numBounce >= 1)
            {
                print(numBounce);
                Ray ray = new Ray(transform.position, rb.velocity);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    rb.velocity = hit.normal * rb.velocity.magnitude * bounceMultiplyer;
                }
                else
                {
                    rb.velocity = -rb.velocity * bounceMultiplyer;
                }
                damage += bounceBonus;
                numBounce--;
            }
            else
            {
                DestroyProjectile(0);
            }
        }

    }

    private void OnDestroy()
    {
        if (view.IsMine)
        {
            rifle.RifleActive(true);
        }
    }


}
