using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace IRToolTrack
{
    public class IRToolController : MonoBehaviour
    {

        public GameObject depthToWorld;
        public GameObject targetToDepth;
        public GameObject targetToDepthText;
        public GameObject depthToWorldText;
        public GameObject depthToWorldOrientationText;
        public GameObject targetToWorldText;

        public string identifier;
        public GameObject[] spheres;
        public bool disableUntilDetection = false;
        public bool disableWhenTrackingLost = false;
        public float secondsLostUntilDisable = 3;
        public float sphere_radius = 6.5f;
        public int max_occluded_spheres = 0;
        public float lowpass_factor_rotation = 0.3f;
        public float lowpass_factor_position = 0.6f;

        private bool childrenActive = true;

        private IRToolTracking irToolTracking;
        private Int64 lastUpdate = 0;
        private float lastSpotted = 0;
        private Vector3 targetPosition = Vector3.zero;
        private Quaternion targetRotation = Quaternion.identity;
        private List<Vector3> positions = new List<Vector3>();
        private List<Quaternion> rotations = new List<Quaternion>();
        private bool[] childAtIndexActive;
        public int sphere_count
        {
            get { return spheres.Length; }
        }

        public float[] sphere_positions
        {
            get {
                float[] coordinates = new float[sphere_count*3];
                int cur_coord = 0;
                for (int i = 0; i< sphere_count; i++) {
                    coordinates[cur_coord] = spheres[i].transform.localPosition.x;
                    coordinates[cur_coord + 1] = spheres[i].transform.localPosition.y;
                    coordinates[cur_coord + 2] = spheres[i].transform.localPosition.z;
                    cur_coord += 3;
                }
                return coordinates;
            }
        }


        void Start()
        {
            childAtIndexActive = new bool[transform.childCount];
            irToolTracking = FindObjectOfType<IRToolTracking>();
#if !UNITY_EDITOR
            if (disableUntilDetection)
            {
                for (int i = 0; i<transform.childCount; i++)
                {
                    var curChild = transform.GetChild(i).gameObject;
                    if (curChild.activeSelf)
                    {
                        childAtIndexActive[i] = true;
                        curChild.SetActive(false);
                    }
                }
                childrenActive = false;
            }
#endif
        }

        bool LoadROMFile(string romFilePath)
        {
            var romFile = Resources.Load(romFilePath);
            return false;
        }

        public enum Status
        {
            Inactive,
            Active
        }
        private Status _subStatus = Status.Inactive;

        public void StartTracking()
        {
            if (_subStatus == Status.Active)
            {
                Debug.Log("Tool tracking already started.");
                return;
            }
            //_listener.Start();
            Debug.Log("Started tracking "+identifier);
            _subStatus = Status.Active;
        }

        public void StopTracking()
        {
            if (_subStatus == Status.Inactive)
            {
                Debug.Log("Tracking of "+identifier+" already stopped.");
                return;
            }
            //_listener.Stop();
            Debug.Log("Stopped tracking " + identifier);
            _subStatus = Status.Inactive;
        }

        void Update()
        {
            if (_subStatus == Status.Inactive)
                return;
            Int64 trackingTimestamp = irToolTracking.GetTimestamp();
            float[] tool_transform = irToolTracking.GetToolTransform(identifier);


            // Depth To World Transform
            float[] depthToWorldTransform = irToolTracking.GetDepthToWorldTransform();
            //float[] depthToWorldTransform = new float[7] { 0,0,0,0,0,0,0};


            Quaternion quat = new Quaternion(depthToWorldTransform[3], depthToWorldTransform[4], depthToWorldTransform[5], depthToWorldTransform[6]);
            Vector3 pos = new Vector3(depthToWorldTransform[0], depthToWorldTransform[1], depthToWorldTransform[2]);

            depthToWorld.transform.SetPositionAndRotation(pos, quat);

            if (tool_transform != null && tool_transform[0]!= float.NaN && tool_transform[7]!=0 && lastUpdate<trackingTimestamp)
            {
                if (!childrenActive)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        var curChild = transform.GetChild(i).gameObject;
                        if (childAtIndexActive[i])
                        {
                            curChild.SetActive(true);
                        }
                    }
                    childrenActive = true;
                }

                Quaternion q = new Quaternion(tool_transform[3], tool_transform[4], tool_transform[5], tool_transform[6]);
                targetRotation = q;
                targetPosition = new Vector3(tool_transform[0], tool_transform[1], tool_transform[2]);
                lastSpotted = Time.time;
            }
            else if (childrenActive && disableWhenTrackingLost && Time.time-lastSpotted>secondsLostUntilDisable)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                childrenActive = false;
            }

            /*
            //Delay Positioning by one frame to maybe make it smoother
            if (lastUpdate == trackingTimestamp)
            {
                transform.position = targetPosition;
                //transform.rotation = Quaternion.Lerp(targetRotation, transform.rotation, 0);
            }
            else
            {
                transform.position = Vector3.Lerp(targetPosition, transform.position, 0.5f);
                //transform.rotation = Quaternion.Lerp(targetRotation, transform.rotation, 0.5f);
            }
            */


            targetToDepth.transform.SetPositionAndRotation(targetPosition, targetRotation);
            
            
            Matrix4x4 targetToWorldMatrix = depthToWorld.transform.localToWorldMatrix * targetToDepth.transform.localToWorldMatrix;

            targetToWorldMatrix = FlipTransformRightLeft(targetToWorldMatrix);

            Vector3 newPos = targetToWorldMatrix.GetPosition();

            // How is this not working
            transform.SetPositionAndRotation(targetToWorldMatrix.GetPosition(), targetToWorldMatrix.rotation);
            lastUpdate = trackingTimestamp;

            //Debug.Log(this.transform.position.ToString());

            Vector3 pos1 = targetToDepth.transform.position;
            string text = "TargetToCamera: (" + System.Math.Round(pos1.x * 1000, 4).ToString()
                + ", " + System.Math.Round(pos1.y * 1000, 4).ToString()
                + ", " + System.Math.Round(pos1.z * 1000, 4).ToString() + ")";
            targetToDepthText.GetComponent<TextMeshPro>().SetText(text);

            Vector3 pos2 = depthToWorld.transform.position;
            string text2 = "DepthToWorld: (" + System.Math.Round(pos2.x * 1000, 4).ToString()
                + ", " + System.Math.Round(pos2.y * 1000, 4).ToString()
                + ", " + System.Math.Round(pos2.z * 1000, 4).ToString() + ")";
            depthToWorldText.GetComponent<TextMeshPro>().SetText(text2);

            Quaternion pos4 = depthToWorld.transform.rotation;
            string text4 = "DepthToWorldOrientation: (" + System.Math.Round(pos4.w, 4).ToString()
                + ", " + System.Math.Round(pos4.x, 4).ToString()
                + ", " + System.Math.Round(pos4.y, 4).ToString() 
                + ", " + System.Math.Round(pos4.z, 4).ToString() + ")";
            depthToWorldOrientationText.GetComponent<TextMeshPro>().SetText(text4);


            Vector3 pos3 = this.transform.position;
            string text3 = "TargetToWorld: (" + System.Math.Round(pos3.x * 1000, 4).ToString()
                + ", " + System.Math.Round(pos3.y * 1000, 4).ToString()
                + ", " + System.Math.Round(pos3.z * 1000, 4).ToString() + ")";
            targetToWorldText.GetComponent<TextMeshPro>().SetText(text3);
        }
        Matrix4x4 FlipTransformRightLeft(Matrix4x4 matr)
        {
            matr.m20 = matr.m20 * -1.0f;
            matr.m02 = matr.m02 * -1.0f;
            matr.m21 = matr.m21 * -1.0f;
            matr.m12 = matr.m12 * -1.0f;
            matr.m23 = matr.m23 * -1.0f;
            return matr;
            
        }
    }
}