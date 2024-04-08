using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider manaSlider;
    public Image fill;

    public void SetSlider(float amount)
    {
        manaSlider.value = amount;
    }

    public void SetSliderMax(float amount)
    {
        manaSlider.maxValue = amount;
        SetSlider(amount);
    }
}
