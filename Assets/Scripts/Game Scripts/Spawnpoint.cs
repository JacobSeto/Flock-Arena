using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] GameObject graphics;

    private void Start()
    {
        graphics.SetActive(false);
    }
}
