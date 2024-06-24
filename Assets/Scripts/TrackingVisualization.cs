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

    //public GameObject mainCamera;

    string text;
    Vector3 pos;
    Matrix4x4 stylusTipToWorldMatrix;
    Matrix4x4 targetToWorldMatrix;

    // Store previous matrices and perform linear interpolation between previous and target
    Vector3 currentStylusPos;
    Quaternion currentStylusRot;

    Vector3 currentTargetPos;
    Quaternion currentTargetRot;

    float lerpFactor = 0.35f;

    // Update is called once per frame
    void Update()
    {

        // Update IR Transform Overlay
        stylusTipToWorldMatrix = DepthToWorld.transform.localToWorldMatrix * StylusTipToCamera.transform.localToWorldMatrix;
        stylusTipToWorldMatrix = MatrixExtensions.FlipTransformRightLeft(stylusTipToWorldMatrix);

        // Perform smoothing
        currentStylusPos = Vector3.Lerp(currentStylusPos, stylusTipToWorldMatrix.GetPosition(), lerpFactor);
        currentStylusRot = Quaternion.Lerp(currentStylusRot, stylusTipToWorldMatrix.rotation, lerpFactor); 


        //StylusTipToWorld.transform.SetPositionAndRotation(stylusTipToWorldMatrix.GetPosition(), stylusTipToWorldMatrix.rotation);
        StylusTipToWorld.transform.SetPositionAndRotation(currentStylusPos, currentStylusRot);

        // Update Optical Transform Overlay
        targetToWorldMatrix = DepthToWorld.transform.localToWorldMatrix * TargetToCamera.transform.localToWorldMatrix;
        targetToWorldMatrix = MatrixExtensions.FlipTransformRightLeft(targetToWorldMatrix);

        // Perform smoothing
        currentTargetPos = Vector3.Lerp(currentTargetPos, targetToWorldMatrix.GetPosition(), lerpFactor);
        currentTargetRot = Quaternion.Lerp(currentTargetRot, targetToWorldMatrix.rotation, lerpFactor);


        //TargetToWorld.transform.SetPositionAndRotation(targetToWorldMatrix.GetPosition(), targetToWorldMatrix.rotation);
        TargetToWorld.transform.SetPositionAndRotation(currentTargetPos, currentTargetRot);

        // Stylus To Depth Text
        // Temporarily replacing with main camera text
        pos = StylusTipToCamera.transform.position;
        //pos = mainCamera.transform.position;
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
