﻿using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IRToolTrack
{
    public class IRToolController : MonoBehaviour
    {
        
        public string identifier;
        public GameObject[] spheres;
        public bool disableUntilDetection = false;
        public float sphere_radius = 6.5f;
        private bool childrenActive = true;

        private IRToolTracking irToolTracking;
        private Int64 lastUpdate = 0;
        private Vector3 targetPosition = Vector3.zero;
        private Quaternion targetRotation = Quaternion.identity;
        private List<Vector3> positions = new List<Vector3>();
        private List<Quaternion> rotations = new List<Quaternion>();
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
            irToolTracking = FindObjectOfType<IRToolTracking>();
            if (disableUntilDetection)
            {
                gameObject.SetChildrenActive(false);
                childrenActive = false;
            }
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
            if (tool_transform != null && tool_transform[0]!= float.NaN && tool_transform[7]!=0 && lastUpdate<trackingTimestamp)
            {
                if (!childrenActive)
                {
                    gameObject.SetChildrenActive(true);
                    childrenActive = true;
                }

                Quaternion q = new Quaternion(tool_transform[3], tool_transform[4], tool_transform[5], tool_transform[6]);
                targetRotation = q;
                targetPosition = new Vector3(tool_transform[0], tool_transform[1], tool_transform[2]);
                
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
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            lastUpdate = trackingTimestamp;
        }
    }
}