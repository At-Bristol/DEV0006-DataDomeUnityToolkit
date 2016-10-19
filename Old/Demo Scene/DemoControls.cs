using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DemoControls : MonoBehaviour
{
    const int FOVIncrement = 5;
    const float RotationIncrement = 5.0f;
    const float PositionIncrement = 0.1f;

    public DomeController domeController;

    public Transform demoCameraBase;

    public Text fpsText;

	void Awake()
    {
        if (domeController == null)
            Debug.LogError("DemoControls: Dome Controller not set!");

        m_initialWorldCameraPitch = domeController.worldCameraPitch;
        m_initialWorldCameraRoll  = domeController.worldCameraRoll;

        if (demoCameraBase == null)
            Debug.LogError("DemoControls: Demo Camera Base not set!");

        if (fpsText == null)
            Debug.LogError("DemoControl: FPS text not set!");
	}
	
	void LateUpdate()
    {
        if (domeController == null || domeController.worldCamera == null)
            return;

        Vector3 cameraPos       = demoCameraBase.position;
        Vector3 cameraForward   = domeController.worldCamera.transform.forward;
        Vector3 cameraRight     = domeController.worldCamera.transform.right;
        Vector3 cameraUp        = domeController.worldCamera.transform.up;

        // Demo Controls:
        // --------------

        // Rectangular brackets ([ and ]) decrease or increase the dome projection FOV.
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            domeController.FOV = Mathf.Max(domeController.FOV - FOVIncrement, DomeController.MinFOV);
        else if (Input.GetKeyDown(KeyCode.RightBracket))
            domeController.FOV = Mathf.Min(domeController.FOV + FOVIncrement, DomeController.MaxFOV);

        // [1] through [5] on the main keyboard control cube map size.
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            domeController.cubeMapType = DomeController.CubeMapType.Cube512;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            domeController.cubeMapType = DomeController.CubeMapType.Cube1024;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            domeController.cubeMapType = DomeController.CubeMapType.Cube2048;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            domeController.cubeMapType = DomeController.CubeMapType.Cube4096;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            domeController.cubeMapType = DomeController.CubeMapType.Cube8192;

        // [F1] through [F3] control the anti-aliasing modes (Off, 2X SSAA, 4X SSAA)
        else if (Input.GetKeyDown(KeyCode.F1))
            domeController.antiAliasingType = DomeController.AntiAliasingType.Off;
        else if (Input.GetKeyDown(KeyCode.F2))
            domeController.antiAliasingType = DomeController.AntiAliasingType.SSAA_2X;
        else if (Input.GetKeyDown(KeyCode.F3))
            domeController.antiAliasingType = DomeController.AntiAliasingType.SSAA_4X;

        // Up, down, left and right arrows control camera pitch and roll.
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            domeController.worldCameraPitch -= RotationIncrement;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            domeController.worldCameraPitch += RotationIncrement;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            domeController.worldCameraRoll += RotationIncrement;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            domeController.worldCameraRoll -= RotationIncrement;

        // W,A,S,D controls camera position.
        else if (Input.GetKeyDown(KeyCode.D))
            cameraPos += cameraRight * PositionIncrement;
        else if (Input.GetKeyDown(KeyCode.A))
            cameraPos -= cameraRight * PositionIncrement;
        else if (Input.GetKeyDown(KeyCode.W))
            cameraPos += cameraForward * PositionIncrement;
        else if (Input.GetKeyDown(KeyCode.S))
            cameraPos -= cameraForward * PositionIncrement;

        // Q and E control camera elevation.
        else if (Input.GetKeyDown(KeyCode.E))
            cameraPos += cameraUp * PositionIncrement;
        else if (Input.GetKeyDown(KeyCode.Q))
            cameraPos -= cameraUp * PositionIncrement;

        // R resets demo parameters.
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset
            cameraPos = Vector3.zero;

            domeController.worldCameraPitch = m_initialWorldCameraPitch;
            domeController.worldCameraRoll  = m_initialWorldCameraRoll;
            domeController.cubeMapType      = DomeController.CubeMapType.Cube1024;
            domeController.antiAliasingType = DomeController.AntiAliasingType.Off;
        }

        // V turns v-sync on and off.
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (QualitySettings.vSyncCount == 0)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        }

        // --------------

        // Update FPS counter
        if (fpsText != null)
        {
            // Maintain a running average of the last N frame deltas, for a more stable frame counter.
            m_frameDeltas[m_curFrameDelta++] = Time.deltaTime;
            if (m_curFrameDelta >= 10)
                m_curFrameDelta = 0;
            float totalFrameDelta = 0.0f;
            for (int i = 0; i < 10; i++)
                totalFrameDelta += m_frameDeltas[i];
            fpsText.text = string.Format("{0}", Mathf.Round(totalFrameDelta != 0.0f ? (float) NumFrameDeltas / totalFrameDelta : 0));
        }
	}

    // Camera properties
    private float m_initialWorldCameraPitch = 0.0f;
    private float m_initialWorldCameraRoll  = 0.0f;

    // FPS counter data
    private const int NumFrameDeltas = 10;
    private float[] m_frameDeltas = new float[NumFrameDeltas];
    private int m_curFrameDelta;
}
