using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Weapon
{
    public override void Shoot()
    {
        Ray ray = playerController.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));  //casts ray from the center of the screen
        ray.direction += new Vector3(Random.Range(-spread,spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
        ray.origin = playerController.cameraTransform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            BulletHit(hit);
        }
        base.Shoot();
        //weapon fire sound
    }

    public virtual void BulletHit(RaycastHit hit)
    {
        //item info class does not have the damage info,so cast iteminfo class to weaponinfo class to access damage variable
        hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
        view.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
    }

    [PunRPC]
    public void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        GameObject bullet;
        Collider[] cols = Physics.OverlapSphere(hitPosition, .5f);
        if(cols.Length == 0)
        {
            return;
        }
        if (cols[0].CompareTag("Head") || cols[0].CompareTag("Body"))
        {
            bullet = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
        }
        else
        {
            bullet = Instantiate(bulletMissPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletMissPrefab.transform.rotation);
        }
        Destroy(bullet, 2.5f);
        bullet.transform.SetParent(cols[0].transform);
    }

}
