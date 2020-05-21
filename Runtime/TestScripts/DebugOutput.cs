using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOutput : MonoBehaviour
{
    public bool shouldDebug = false;
    public float debugWaitTime = 0;
    public float debugTimer = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (shouldDebug)
        {
            debugTimer += Time.deltaTime;
            if(debugTimer > debugWaitTime)
            {
                debugTimer = 0;
                Debug.Log("test debug");
                Debug.LogError("test error");
                Debug.LogWarning("test warning");
            }
        }
    }
}
