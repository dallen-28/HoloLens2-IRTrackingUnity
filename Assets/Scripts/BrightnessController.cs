using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class BrightnessController : MonoBehaviour
{
    public Material targetMaterial;


    public void OnSliderValueChanged(SliderEventData eventData)
    {
        // Assuming you have a material named "DynamicImageMaterial" on your object
        if (targetMaterial != null)
        {
            // Adjust these property names according to your shader
            targetMaterial.SetFloat("_Contrast", eventData.NewValue);
            // Add more property assignments for contrast, sharpness, temperature, tint, etc.
        }
    }
}
