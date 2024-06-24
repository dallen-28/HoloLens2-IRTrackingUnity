using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWireFrame : MonoBehaviour
{
    public Material wireframeMaterial;
    public Material transparentMaterial;

    MeshRenderer meshRenderer; 

    bool isWireframe = true; // Flag to track the current material state

    // Start is called before the first frame update
    void Start()
    {
        // Get all child renderers
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void ToggleMaterial()
    {

        if (isWireframe)
        {
            meshRenderer.material = transparentMaterial;
        }
        else
        {
            meshRenderer.material = wireframeMaterial;
        }
        isWireframe = !isWireframe;

    }

}
