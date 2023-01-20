using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    // Class that adds extra functionality to Toggles
    [SerializeField] Toggle toggle;
    [SerializeField] GameObject toggleActiveGameObject;

    public void ToggleGameObject()
    {
        //sets GameObject active or inactive based on value of toggle (active if on, else off)
        if (toggle.isOn)
            toggleActiveGameObject.SetActive(true);
        else
            toggleActiveGameObject.SetActive(false);
    }
}
