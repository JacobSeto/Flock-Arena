using UnityEngine;
using Photon.Pun;

public class Recoil : MonoBehaviourPunCallbacks
{
    PhotonView view;

    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Settings
    [SerializeField] private float recoilSpeed;
    [SerializeField] private float dampSpeed;

    private void Update()
    {

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, dampSpeed * Time.deltaTime);
        //slerp better for rotations / directions
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(float recoilX, float recoilY, float recoilZ)
    {
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

}
