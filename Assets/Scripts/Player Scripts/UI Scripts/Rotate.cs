using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    //used to rotate GameObjects

    [SerializeField] float rotationSpeedX;
    [SerializeField] float rotationSpeedY;
    [SerializeField] float rotationSpeedZ;
    private void FixedUpdate()
    {
        transform.Rotate(rotationSpeedX, rotationSpeedY, rotationSpeedZ);
    }
}
