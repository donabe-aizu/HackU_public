using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleVRMR : MonoBehaviour
{
    [SerializeField] 
    private OVRPassthroughLayer passthroughLayer;

    private Camera camera;
    private bool passthroughToggled;

    private void Start()
    {
        camera = Camera.main;
        passthroughToggled = OVRManager.IsPassthroughRecommended();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            passthroughToggled = !passthroughToggled;
            passthroughLayer.enabled = passthroughToggled;
            camera.clearFlags = passthroughToggled ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
            camera.backgroundColor = passthroughToggled ? Color.clear : Color.white;
        }
    }
}
