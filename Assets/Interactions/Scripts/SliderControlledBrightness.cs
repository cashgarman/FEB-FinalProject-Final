using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderControlledBrightness : MonoBehaviour
{
    public Light light;
    public float minIntensity;
    public float maxIntensity;
    public Slider slider;

    public void OnSliderValueChanged()
    {
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, slider.value);
    }
}
