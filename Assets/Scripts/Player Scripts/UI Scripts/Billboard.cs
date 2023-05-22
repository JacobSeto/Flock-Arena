using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera playerCamera;
    private void Update()
    {
        if (playerCamera == null)
            playerCamera = GameObject.FindWithTag("Player Camera")?.GetComponent<Camera>();

        if (playerCamera == null)
            return;
        else
        {
            transform.LookAt(playerCamera.transform);
            transform.Rotate(Vector3.up * 180);
        }
    }
}
