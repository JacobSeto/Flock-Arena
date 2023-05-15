using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //Moves GameObject at FixedUpdate
    [SerializeField] float moveX;
    [SerializeField] float moveY;
    [SerializeField] float moveZ;
    Vector3 move;

    private void Awake()
    {
        move = new Vector3(moveX, moveY, moveZ);
    }

    private void FixedUpdate()
    {
        transform.position += move;
    }
}
