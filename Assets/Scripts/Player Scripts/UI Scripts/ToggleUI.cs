using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    // Class that adds extra functionality to Toggles

    //single toggle
    [SerializeField] Toggle toggle;
    public GameObject togglePair;  //GameObject that pairs with the Toggle



    public void ToggleGameObject()
    {
        //sets GameObject active or inactive based on value of toggle (active if on, else off)
        if (toggle.isOn)
        {
            togglePair.SetActive(true);
        }
        else
        {
            print("set unactive");
            togglePair.SetActive(false);
        }
            
    }

    public void ToggleInteractable()
    {
        //toggle interactable.  Only use if isOn is changed manually
        print(toggle.isOn);
        if (toggle.isOn)
        {
            toggle.interactable = false;
        }
        else
        {
            toggle.interactable = true;
        }
    }




}
