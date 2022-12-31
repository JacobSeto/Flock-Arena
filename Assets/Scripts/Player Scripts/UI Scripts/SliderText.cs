using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderText : MonoBehaviour
{
    public Slider mouseSlider;
    public TextMeshProUGUI mouseText;

    private void Start()
    {
        UpdateMouseSensText();
    }
    public void UpdateMouseSensText()
    {
        string sensitivity = (Mathf.Round(mouseSlider.value) / 100).ToString();
        if (sensitivity.IndexOf(".") == -1) sensitivity += ".00";
        mouseText.text = "Mouse Sensitivity: " + sensitivity;
    }
}