using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Use this class to display transform data in the UI 
// using both text and visual overlay
public class TrackingVisualization : MonoBehaviour
{
    // Transform coming from our optical tracking plugin - From stylus to hololens DRB
    public GameObject StylusTipToCamera;

    // Text object to update for optically tracked tool
    public GameObject TargetToCamera;

    // Transform coming from our IR plugin - From Depth camera to world
    public GameObject DepthToWorld;

    // Tool to display optical tracking overlay
    public GameObject StylusTipToWorld;

    public GameObject TargetToWorld;

    // Text object to update for IR tracked tool
    public TextMeshPro StylusTipToCameraText;

    // Text object to update for IR tracked tool
    public TextMeshPro TargetToCameraText;

    // Text object to update for IR tracked tool
    public TextMeshPro DepthToWorldText;

    string text;
    Vector3 pos;
    Matrix4x4 stylusTipToWorldMatrix;
    Matrix4x4 targetToWorldMatrix;

    // Update is called once per frame
    void Update()
    {

        // Update IR Transform Overlay
        stylusTipToWorldMatrix = DepthToWorld.transform.localToWorldMatrix * StylusTipToCamera.transform.localToWorldMatrix;
        stylusTipToWorldMatrix = MatrixExtensions.FlipTransformRightLeft(stylusTipToWorldMatrix);
        StylusTipToWorld.transform.SetPositionAndRotation(stylusTipToWorldMatrix.GetPosition(), stylusTipToWorldMatrix.rotation);

        // Update Optical Transform Overlay
        targetToWorldMatrix = DepthToWorld.transform.localToWorldMatrix * TargetToCamera.transform.localToWorldMatrix;
        targetToWorldMatrix = MatrixExtensions.FlipTransformRightLeft(targetToWorldMatrix);
        TargetToWorld.transform.SetPositionAndRotation(targetToWorldMatrix.GetPosition(), targetToWorldMatrix.rotation);

        // Stylus To Depth Text
        pos = StylusTipToCamera.transform.position;
        text = "StylusTipToCamera: (" + System.Math.Round(pos.x * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.y * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.z * 1000, 4).ToString() + ")";
        StylusTipToCameraText.SetText(text);

        // Stylus To Hololens Text
        pos = TargetToCamera.transform.position;
        text = "TargetToCamera: (" + System.Math.Round(pos.x * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.y * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.z * 1000, 4).ToString() + ")";
        TargetToCameraText.SetText(text);

        // Stylus To Hololens Text
        pos = DepthToWorld.transform.position;
        text = "DepthToWorld: (" + System.Math.Round(pos.x * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.y * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.z * 1000, 4).ToString() + ")";
        DepthToWorldText.SetText(text);



    }

}
