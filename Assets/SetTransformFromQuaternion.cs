using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SetTransformFromQuaternion : MonoBehaviour
{
    public Vector3 position;
    public Vector4 rotation;


    // Start is called before the first frame update
    void Start()
    {
        Quaternion newrotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        this.transform.SetPositionAndRotation(this.position, newrotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
