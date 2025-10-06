using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxValue(int maxHealthValue)
    {
        slider.maxValue = maxHealthValue;
        slider.value = maxHealthValue;
    }

    public void SetSliderValue(int value)
    {
        slider.value = value;
    }

}
