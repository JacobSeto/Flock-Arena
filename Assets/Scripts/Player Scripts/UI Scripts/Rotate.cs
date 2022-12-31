using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    private void Update()
    {
        transform.Rotate(0, rotationSpeed, 0);
    }
}
