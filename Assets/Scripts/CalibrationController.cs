using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms.VisualStyles;
using TMPro;
using UnityEngine;
using UnityEngine.XR.OpenXR.Input;

// Takes in Target To Depth (From IR Tracking plugin)
// and Target To Hololens (From OpenIGTLink) and
// saves position sequences to file

public class CalibrationController : MonoBehaviour
{
    public GameObject StylusToDepth;
    public GameObject StylusToHololens;
    public GameObject ConfirmationText;

    public int numFrames;

    string[] opticalFcsvLines;
    string[] pluginFcsvLines;
    string[] opticalMatlabLines;
    string[] pluginMatlabLines;
 
    List<string> opticalDataListFcsv;
    List<string> pluginDataListFcsv;
    List<string> opticalDataListMatlab;
    List<string> pluginDataListMatlab;
    List<Vector3> opticalPoints;
    List<Vector3> pluginPoints;

    bool record;
    Vector3 opticalPos;
    Vector3 pluginPos;
    Vector3 opticalSum;
    Vector3 pluginSum;
    Vector3 opticalAvg;
    Vector3 pluginAvg;

    public void Start()
    {
        record = false;

        opticalDataListFcsv = new List<string>();
        pluginDataListFcsv = new List<string>();

        opticalDataListMatlab = new List<string>();
        pluginDataListMatlab = new List<string>();

        opticalPoints = new List<Vector3>();
        pluginPoints = new List<Vector3>();
    }

    public void Update()
    {
        if(record)
        {
            SaveTrackingData();
        }

    }
    public void ToggleRecord()
    {
        if (record)
        {
            Debug.Log("Stop recording");
            // Turn off Red LED
        }
        else
        {
            Debug.Log("Start recording");
        }

        record = !record;
    }

    // Initialize collection coroutine
    public void SaveTrackingData()
    {

        StartCoroutine(CollectXFrames());
    }
    // Format list for viewing as Slicer Markups
    public void SavePointsToFCSV()
    {
        opticalDataListFcsv.Add("# Markups fiducial file version = 4.11\n");
        opticalDataListFcsv.Add("# CoordinateSystem = 0\n");
        opticalDataListFcsv.Add("# columns = id,x,y,z,ow,ox,oy,oz,vis,sel,lock,label,desc,associatedNodeID\n");

        pluginDataListFcsv.Add("# Markups fiducial file version = 4.11\n");
        pluginDataListFcsv.Add("# CoordinateSystem = 0\n");
        pluginDataListFcsv.Add("# columns = id,x,y,z,ow,ox,oy,oz,vis,sel,lock,label,desc,associatedNodeID\n");

        string line;
        for (int i = 0; i < opticalPoints.Count; i++)
        {
            // Optical
            line = "1," + (opticalPoints[i].x * 1000).ToString() + ", " + (opticalPoints[i].y * 1000).ToString() + ", "
                + (opticalPoints[i].z * 1000).ToString() + ",0,0,0,1,1,1,0,OpticalPoints-" + (i + 1).ToString() + ",,\n";
            opticalDataListFcsv.Add(line);

            // Plugin
            line = "1," + (pluginPoints[i].x * 1000).ToString() + ", " + (pluginPoints[i].y * 1000).ToString() + ", " 
                + (pluginPoints[i].z * 1000).ToString() + ",0,0,0,1,1,1,0,PluginPoints-" + (i + 1).ToString() + ",,\n";

            pluginDataListFcsv.Add(line);
        }
    }

    // Format list for viewing in matlab
    public void SavePointsToMatlab()
    {
        string line;
        for (int i = 0; i < opticalPoints.Count; i++)
        {
            // Optical
            line = (opticalPoints[i].x * 1000).ToString() + ", " + (opticalPoints[i].y * 1000).ToString() 
                + ", " + (opticalPoints[i].z * 1000).ToString() + "\n";
            opticalDataListMatlab.Add(line);

            // Plugin
            line = (pluginPoints[i].x * 1000).ToString() + ", " + (pluginPoints[i].y * 1000).ToString() 
                + ", " + (pluginPoints[i].z * 1000).ToString() + "\n";

            pluginDataListMatlab.Add(line);
        }
    }

    // Collect X Frames
    IEnumerator CollectXFrames()
    {

        ConfirmationText.GetComponent<TextMeshPro>().SetText("Collecting Frames...");

        opticalSum = new Vector3(0,0,0);
        pluginSum = new Vector3(0,0,0);

        for (int i=1; i <= numFrames; i++)
        {
            opticalPos = StylusToHololens.transform.position;
            pluginPos = StylusToDepth.transform.position;

            //opticalPoints.Add(opticalPos);

            opticalSum += opticalPos;
            pluginSum += pluginPos;

            yield return null;
        }
        // Calculate point average
        opticalAvg = opticalSum/numFrames;
        pluginAvg = pluginSum/numFrames;

        ConfirmationText.GetComponent<TextMeshPro>().SetText("Frame Collection Complete");

        // Add to list
        opticalPoints.Add(opticalAvg);
        pluginPoints.Add(pluginAvg);

        
    }

    public void OnApplicationQuit()
    {
        SavePointsToFCSV();
        SavePointsToMatlab();

        opticalFcsvLines = opticalDataListFcsv.ToArray();
        opticalMatlabLines = opticalDataListMatlab.ToArray();
        pluginFcsvLines = pluginDataListFcsv.ToArray();
        pluginMatlabLines = pluginDataListMatlab.ToArray();

        string path = Path.Combine(Application.persistentDataPath, "OpticalPoints.fcsv");
        string path2 = Path.Combine(Application.persistentDataPath, "PluginPoints.fcsv");
        string path3 = Path.Combine(Application.persistentDataPath, "OpticalPoints.csv");
        string path4 = Path.Combine(Application.persistentDataPath, "PluginPoints.csv");
        byte[] data = System.Text.Encoding.ASCII.GetBytes(string.Concat(opticalFcsvLines));
        byte[] data2 = System.Text.Encoding.ASCII.GetBytes(string.Concat(pluginFcsvLines));
        byte[] data3 = System.Text.Encoding.ASCII.GetBytes(string.Concat(opticalMatlabLines));
        byte[] data4 = System.Text.Encoding.ASCII.GetBytes(string.Concat(pluginMatlabLines));

        UnityEngine.Windows.File.WriteAllBytes(path, data);
        UnityEngine.Windows.File.WriteAllBytes(path2, data2);
        UnityEngine.Windows.File.WriteAllBytes(path3, data3);
        UnityEngine.Windows.File.WriteAllBytes(path4, data4);

        Application.Quit();

    }
}
