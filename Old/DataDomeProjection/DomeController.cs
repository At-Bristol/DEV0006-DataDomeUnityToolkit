/*=====================================================================================================================================
Unity Data Dome Toolkit prepared by Zubr VR Ltd.

Unity Data Dome Toolkit copyright (c) 2016, At-Bristol (Enterprises) Limited
All rights reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
======================================================================================================================================*/


using UnityEngine;
using System.Collections;

public class DomeController : MonoBehaviour
{
    //----------------------------------------------------------------------------------------------------
    // DEFAULTS AND LIMITS
    //----------------------------------------------------------------------------------------------------

    public const float WorldCameraDefPitch = -80.0f;
    public const float WorldCameraMinPitch = -120.0f;
    public const float WorldCameraMaxPitch = +120.0f;

    public const float WorldCameraDefRoll  =    0.0f;
    public const float WorldCameraMinRoll  = -180.0f;
    public const float WorldCameraMaxRoll  = +180.0f;

    public const int DefFOV = 270;
    public const int MinFOV = 120;
    public const int MaxFOV = 270;

    public const float DefBackFadeIntensity     =  0.1f;
    public const float DefCrescentFadeIntensity =  0.5f;
    public const float DefCrescentFadeRadius    =  0.8f;
    public const float DefCrescentFadeOffset    = -0.2f;

    //----------------------------------------------------------------------------------------------------
    // ERROR MESSAGES
    //----------------------------------------------------------------------------------------------------

    private const string ProjectionCameraMissingError = "DomeController: required component \"Projection Camera\" is missing from Dome Projector asset! Reverting the Dome Projector prefab will probably fix this error.";
    private const string ProjectionCameraNotMainCamera = "DomeController: \"Projection Camera\" is not the main camera! Most likely, you still need to delete the \"Main Camera\" that was added when Unity created the scene.";
    private const string WorldCameraMissingError = "DomeController: required component \"World Camera\" is missing from the Dome Projector asset! Reverting the Dome Projector prefab will probably fix this error.";

    //----------------------------------------------------------------------------------------------------
    // ENUMS
    //----------------------------------------------------------------------------------------------------

    // Anti-aliasing types supported by the dome projection renderer.
    public enum AntiAliasingType
    {
        Off,                    // No anti-aliasing
        SSAA_2X,                // 2X super sampling
        SSAA_4X,                // 4X super sampling
    }

    // Cubemap sizes supported by the dome projection renderer.
    public enum CubeMapType
    {
        Cube512    = 512,       // Cubemap with 512x512 faces
        Cube1024   = 1024,      // Cubemap with 1024x1024 faces
        Cube2048   = 2048,      // etc.
        Cube4096   = 4096,
        Cube8192   = 8192,
    }

    //----------------------------------------------------------------------------------------------------
    // EDITOR PROPERTIES
    //----------------------------------------------------------------------------------------------------

    [Range(WorldCameraMinPitch, WorldCameraMaxPitch)]
    public float worldCameraPitch = WorldCameraDefPitch;

    [Range(WorldCameraMinRoll, WorldCameraMaxRoll)]
    public float worldCameraRoll = WorldCameraDefRoll;

    // Field of view of the fisheye 'lens' used during the dome projection.
    [Range(MinFOV, MaxFOV)]
    public int FOV = DefFOV;

    // Size of the cubemaps captured from the scene.
    // Larger cubemaps == higher quality
    // Smaller cubemaps == better performance
    public CubeMapType cubeMapType = CubeMapType.Cube1024;

    // Anti-aliasing type used when rendering the dome projection
    public AntiAliasingType antiAliasingType = AntiAliasingType.SSAA_2X;

    // Linear front-to-back fade.
    [Range(0.0f, 1.0f)]
    public float backFadeIntensity = DefBackFadeIntensity;

    // Crescent fade.
    [Range(0.0f, 1.0f)]
    public float crescentFadeIntensity = DefCrescentFadeIntensity;

    [Range(0.0f, 1.0f)]
    public float crescentFadeRadius = DefCrescentFadeRadius;

    [Range(-1.0f, +1.0f)]
    public float crescentFadeOffset = DefCrescentFadeOffset;

    //----------------------------------------------------------------------------------------------------
    // PUBLIC
    //----------------------------------------------------------------------------------------------------

    public Camera projectionCamera { get { return m_projectionCamera; } }

    public Camera worldCamera { get { return m_worldCamera; } }

    //----------------------------------------------------------------------------------------------------
    // UNITY EVENTS
    //----------------------------------------------------------------------------------------------------
       
	void Awake()
    {
        // Fetch projection camera from child.
        Transform projectionCameraTrans = transform.FindChild("Projection Camera");
        if (projectionCameraTrans == null)
            Debug.LogError(ProjectionCameraMissingError);
        m_projectionCamera = projectionCameraTrans.GetComponent<Camera>();
        if (m_projectionCamera == null)
            Debug.LogError(ProjectionCameraMissingError);

        // Check if projection camera is the main camera.
        if (Camera.main != m_projectionCamera)
            Debug.LogError(ProjectionCameraNotMainCamera);

        // Fetch world camera from child.
        Transform worldCameraTrans = transform.FindChild("World Camera");
        if (worldCameraTrans == null)
            Debug.LogError(WorldCameraMissingError);
        m_worldCamera = worldCameraTrans.GetComponent<Camera>();
        if (m_worldCamera == null)
            Debug.LogError(WorldCameraMissingError);
	}
	
	void Update()
    {
        if (m_worldCamera == null)
            return;

        // Ensure FOV stays within a valid range.
        FOV = Mathf.Clamp(FOV, MinFOV, MaxFOV);

        // Apply world camera pitch and roll.
        worldCameraPitch = Mathf.Clamp(worldCameraPitch, WorldCameraMinPitch, WorldCameraMaxPitch);
        worldCameraRoll  = Mathf.Clamp(worldCameraRoll,  WorldCameraMinRoll,  WorldCameraMaxRoll);
        m_worldCamera.transform.localRotation = Quaternion.Euler(new Vector3(worldCameraPitch, 0, worldCameraRoll));
	}

    //----------------------------------------------------------------------------------------------------
    // PRIVATE
    //----------------------------------------------------------------------------------------------------

    Camera m_projectionCamera;
    Camera m_worldCamera;
}
