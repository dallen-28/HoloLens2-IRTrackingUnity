using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Use this class to display transform data in the UI 
// using both text and visual overlay
public class TrackingVisualization : MonoBehaviour
{
    // Transform coming from our IR plugin - From stylus to depth camera
    public GameObject StylusToDepth;

    public GameObject StylusToCamera;

    // Transform coming from our optical tracking plugin - From stylus to hololens DRB
    public GameObject StylusToHololens;

    // Transform coming from our hand eye calibration - can use preliminary for testing
    public GameObject HololensToDepth;

    // Transform coming from our IR plugin - From Depth camera to world
    public GameObject DepthToWorld;

    // Text object to update for IR tracked tool
    public GameObject StylusToDepthText;

    // Text object to update for optically tracked tool
    public GameObject StylusToHololensText;

    // Tool to display IR tracking overlay
    public GameObject SampleToolIR;

    // Tool to display optical tracking overlay
    public GameObject SampleToolOptical;

    // If toggled, 
    public bool showOverlay;

    string text;
    Vector3 pos;
    Matrix4x4 stylusToWorldOptical;
    Matrix4x4 stylusToWorldIR;

    // Update is called once per frame
    void Update()
    {
        // Stylus To Depth Text
        pos = StylusToDepth.transform.position;
        text = "StylusToDepth: (" + System.Math.Round(pos.x * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.y * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.z * 1000, 4).ToString() + ")";
        StylusToDepthText.GetComponent<TextMeshPro>().SetText(text);

        // Stylus To Hololens Text
        pos = StylusToHololens.transform.position;
        text = "StylusToHL2: (" + System.Math.Round(pos.x * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.y * 1000, 4).ToString()
            + ", " + System.Math.Round(pos.z * 1000, 4).ToString() + ")";
        StylusToHololensText.GetComponent<TextMeshPro>().SetText(text);
        

        if(showOverlay)
        {
            DisplayOverlay();
        }


    }
    void DisplayOverlay()
    {
        // Update IR Transform Overlay
        stylusToWorldIR = DepthToWorld.transform.localToWorldMatrix * StylusToDepth.transform.localToWorldMatrix;
        stylusToWorldIR = MatrixExtensions.FlipTransformRightLeft(stylusToWorldIR);
        SampleToolIR.transform.SetPositionAndRotation(stylusToWorldIR.GetPosition(), stylusToWorldIR.rotation);

        // Update Optical Transform Overlay
        stylusToWorldOptical = DepthToWorld.transform.localToWorldMatrix
            * HololensToDepth.transform.localToWorldMatrix * StylusToHololens.transform.localToWorldMatrix;
        stylusToWorldOptical = MatrixExtensions.FlipTransformRightLeft(stylusToWorldOptical);
        SampleToolOptical.transform.SetPositionAndRotation(stylusToWorldOptical.GetPosition(), stylusToWorldOptical.rotation);
    }

}
