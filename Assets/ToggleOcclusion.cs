using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOcclusion : MonoBehaviour
{
    public Material opaqueMaterial;
    public Material occlusionMaterial;

    MeshRenderer meshRenderer;

    bool isOpaque = true; // Flag to track the current material state

    // Start is called before the first frame update
    void Start()
    {
        // Get all child renderers
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void ToggleMaterial()
    {

        if (isOpaque)
        {
            meshRenderer.material = occlusionMaterial;
            //this.GetComponentInChildren<ObjectManipulator>().enabled = false;
        }
        else
        {
            meshRenderer.material = opaqueMaterial;
            //this.GetComponentInChildren<ObjectManipulator>().enabled = true;
        }
        isOpaque = !isOpaque;

    }

}
