using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Use this class to display transform data in the UI 
// using both text and visual overlay
public class CreateOverlay : MonoBehaviour
{
    // Transform coming from our IR plugin - From Depth camera to world
    public GameObject DepthToWorld;

    Matrix4x4 objectToWorld;

    public TextMeshPro textObject;


    // Update is called once per frame
    void Update()
    {
        // Update Optical Transform Overlay
        objectToWorld = DepthToWorld.transform.localToWorldMatrix * this.transform.localToWorldMatrix;
        objectToWorld = MatrixExtensions.FlipTransformRightLeft(objectToWorld);
        this.transform.SetPositionAndRotation(objectToWorld.GetPosition(), objectToWorld.rotation);


    }

}
