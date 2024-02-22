using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;

public class ImageController : MonoBehaviour
{
    public Material targetMaterial;

    public void Start()
    {
        targetMaterial.SetFloat("_Brightness", 0);
        targetMaterial.SetFloat("_Contrast", 1);
        targetMaterial.SetFloat("_Sharpness", 0);
    }

    public void OnBrightnessValueChanged(float newNum)
    {
        // Assuming you have a material named "DynamicImageMaterial" on your object
        if (targetMaterial != null)
        {
            // Adjust these property names according to your shader
            targetMaterial.SetFloat("_Brightness", newNum);
            // Add more property assignments for contrast, sharpness, temperature, tint, etc.
        }
    }
    public void OnContrastValueChanged(float newNum)
    {
        // Assuming you have a material named "DynamicImageMaterial" on your object
        if (targetMaterial != null)
        {
            // Adjust these property names according to your shader
            targetMaterial.SetFloat("_Contrast", newNum);
            // Add more property assignments for contrast, sharpness, temperature, tint, etc.
        }
    }
    public void OnSharpnessValueChanged(float newNum)
    {
        // Assuming you have a material named "DynamicImageMaterial" on your object
        if (targetMaterial != null)
        {
            // Adjust these property names according to your shader
            targetMaterial.SetFloat("_Sharpness", newNum);
            // Add more property assignments for contrast, sharpness, temperature, tint, etc.
        }
    }
    public void OnTemperatureValueChanged(float newNum)
    {
        // Assuming you have a material named "DynamicImageMaterial" on your object
        if (targetMaterial != null)
        {
            // Adjust these property names according to your shader
            targetMaterial.SetFloat("_Temperature", newNum);
            // Add more property assignments for contrast, sharpness, temperature, tint, etc.
        }
    }
    public void OnTintValueChanged(float newNum)
    {
        // Assuming you have a material named "DynamicImageMaterial" on your object
        if (targetMaterial != null)
        {
            // Adjust these property names according to your shader
            targetMaterial.SetFloat("_Tint", newNum);
            // Add more property assignments for contrast, sharpness, temperature, tint, etc.
        }
    }
}
