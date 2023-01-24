using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecursiveProjectile : Projectile
{
    public int numRecursive { get; set; } = 0;
    public string recursiveRound { get; set; }
    [SerializeField] float spawnHeight;
    [SerializeField] float recursiveAngle; //Between 0 and 90
    [SerializeField] float ranAngle;
    [SerializeField] float ranPosition;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == gameObject.tag)
            return;
        base.OnTriggerEnter(other);
    }

    public void Recursive()
    {
        //Instantiates Recursive projectiles on projectile death

        for (int i = 0; i < numRecursive; i++)
        {
            Quaternion recursiveDir = Quaternion.Euler(-recursiveAngle + Random.Range(0, ranAngle), (360 / numRecursive * i), 0f);
            Vector3 recursivePos = transform.position + new Vector3(Random.Range(-ranPosition, ranPosition), Random.Range(-ranPosition, ranPosition),Random.Range(-ranPosition, ranPosition));
            print(recursiveDir);
            GameObject projectile = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", recursiveRound), recursivePos + transform.up * spawnHeight, recursiveDir);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.playerController = playerController;

        }
    }

    private void OnDestroy()
    {
        if (view.IsMine)
            Recursive();
    }
}
