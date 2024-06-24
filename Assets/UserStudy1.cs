using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class UserStudy1 : MonoBehaviour
{
    // Bounding region
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;

    // Target Prefab
    public GameObject targetPrefab;
    GameObject target;

    // StylusTipToWorld 
    public GameObject stylusTipToWorld;

    // TargetToWorld
    public GameObject targetToWorld;

    // Offset of black hole model
    public GameObject blackHoleOrigin;

    // Store number of target insertions
    int counter;

    // Store insertion state
    bool inInsertionState;

    // For storing data
    List<string> dataLinesMatlab;

    // Timers  
    public Stopwatch trialTimer;
    public Stopwatch insertionTimer;

    // For sphere to needle position
    Vector3 sphereToNeedle;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        counter = 0;
        dataLinesMatlab = new List<string>();
        dataLinesMatlab.Add("EuclideanErrors XError YError ZError InsertionTime\n");

        inInsertionState = false;

    }

    public void OnStart()
    {
        if (inInsertionState)
        {
            UnityEngine.Debug.Log("Already in Insertion State");
        }
        else
        {
            OnNext();
        }
    }
    public void OnStop()
    {
        if (!inInsertionState)
        {
            UnityEngine.Debug.Log("Already in Idle State");
        }
        else
        {
            OnNext();
        }
    }

    public void OnNext()
    {
        // if not in insertion state
        if (!inInsertionState)
        {
            // If first insertion start timer and instantiate target prefab
            if (counter == 0)
            {
                // Start trial timer
                trialTimer = Stopwatch.StartNew();

                // Instantiate sphere prefab
                target = Instantiate(targetPrefab);

            }
            // Start insertion timer
            insertionTimer = Stopwatch.StartNew();

            // Hide Target
            this.target.SetActive(true);

            // get random x, y, z location
            float randX = Random.Range(minX, maxX);
            float randY = Random.Range(minY, maxY);
            float randZ = Random.Range(minZ, maxZ);

            // Assume targetToWorld is in the center of phantom;
            Vector3 targetPoint = targetToWorld.transform.localToWorldMatrix.MultiplyPoint(blackHoleOrigin.transform.position + new Vector3(randX, randY, randZ));

            // Set target to random position within bounding box of phantom
            this.target.transform.position = targetPoint;

            inInsertionState = true;
        }

        // If we are in insertion state
        else
        {
            // Stop insertion timer
            insertionTimer.Stop();

            // Save tip position and calculate errors
            sphereToNeedle = stylusTipToWorld.transform.localToWorldMatrix.inverse.MultiplyPoint(target.transform.position);

            // Hide Target
            this.target.SetActive(false);

            // Add errors to error structure in memory
            dataLinesMatlab.Add((sphereToNeedle.magnitude * 1000).ToString() + " "
                + (sphereToNeedle.x * 1000).ToString() + " " + (sphereToNeedle.y * 1000).ToString() + " "
                + (sphereToNeedle.z * 1000).ToString() + " " + insertionTimer.Elapsed.TotalSeconds.ToString() + "\n");



            // Increment insertion counter
            counter++;

            // If 5 insertions successfully completed save the data
            // else move target to new position
            if (counter == 5)
            {
                trialTimer.Stop();
                dataLinesMatlab.Add("Total trial time: " + trialTimer.Elapsed.TotalSeconds.ToString() + " Seconds\n");
                SaveData();
            }
            inInsertionState = false;
        }
    }
    public void SaveData()
    {
        string path = Path.Combine(Application.persistentDataPath, "Errors.csv");
        byte[] data = System.Text.Encoding.ASCII.GetBytes(string.Concat(dataLinesMatlab.ToArray()));
        UnityEngine.Windows.File.WriteAllBytes(path, data);

        Application.Quit();
    }
}
