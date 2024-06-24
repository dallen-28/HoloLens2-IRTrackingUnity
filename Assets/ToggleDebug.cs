using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnToggle()
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
