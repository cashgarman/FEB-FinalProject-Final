using UnityEngine;
using UnityEngine.Events;

public class Slider : MonoBehaviour
{
    public float minX;
    public float maxX;
    public float value;

    public UnityEvent onValueChanged;
    private float prevValue;

    void Update()
    {
        // Calculate how far along the slider is as a percentagle (0 to 1)
        value = (transform.localPosition.x - minX) / (maxX - minX);
        value = Mathf.Clamp(value, 0f, 1f);

        // If the value changed
        if(value != prevValue)
        {
            onValueChanged.Invoke();
            prevValue = value;
        }
    }
}
