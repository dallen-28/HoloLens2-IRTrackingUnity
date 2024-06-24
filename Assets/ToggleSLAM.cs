using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ToggleSLAM : MonoBehaviour
{
    public GameObject slamCam;
    public GameObject trackingController;
    public GameObject depthToWorld;
    public GameObject staticDepthToWorld;

    // Start is called before the first frame update

    private void Awake()
    {
        //ToggleSlam();
    }

    void Start()
    {
        //ToggleSlam();
    }

    public void ButtonIsClicked()
    {
        if (trackingController.activeSelf)
        {
            // Disable Tracking controller so depthToWorld won't be dynamically updated
            trackingController.SetActive(false);

            // Set depthToWorld to hardcoded transform
            depthToWorld.transform.SetPositionAndRotation(staticDepthToWorld.transform.position, staticDepthToWorld.transform.rotation);
        }
        else
        {
            trackingController.SetActive(true);
        }      

        // Disable/Enable SLAM
        ToggleSlam();

    }
    void ToggleSlam()
    {
        Component[] components = Camera.main.GetComponents(typeof(MonoBehaviour));
        bool found = false;
        foreach (Component component in components)
        {
            string name = component.ToString();
            MonoBehaviour mb = (MonoBehaviour)component;
            if (name.Contains("TrackedPoseDriver"))
            {
                if (mb.enabled)
                {
                    mb.enabled = false;

                }
                else
                {
                    mb.enabled = true;
                }

                break;

            }
        }
    }
}
