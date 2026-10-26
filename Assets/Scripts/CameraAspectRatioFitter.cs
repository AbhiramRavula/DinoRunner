using UnityEngine;

/// <summary>
/// Enforces a consistent 16:9 aspect ratio across all devices (Desktop, Mobile WebGL, Tablets).
/// Automatically letterboxes (top/bottom) or pillarboxes (left/right) so the camera never
/// shows empty areas outside the game world or spawns obstacles in plain view on ultrawide screens.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraAspectRatioFitter : MonoBehaviour
{
    [Tooltip("Target aspect ratio for the game (16:9 = 1.777778)")]
    public float targetAspect = 16f / 9f;

    [Tooltip("Color of the letterbox/pillarbox borders")]
    public Color letterboxColor = Color.black;

    private Camera mainCam;
    private Camera backgroundCam;
    private int lastWidth;
    private int lastHeight;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
        CreateBackgroundCamera();
        UpdateViewport();
    }

    private void CreateBackgroundCamera()
    {
        // A secondary camera that clears the screen to black before the main camera draws
        GameObject bgCamObj = new GameObject("LetterboxCamera");
        bgCamObj.transform.SetParent(transform);
        bgCamObj.transform.localPosition = Vector3.zero;

        backgroundCam = bgCamObj.AddComponent<Camera>();
        backgroundCam.depth = mainCam.depth - 1;
        backgroundCam.clearFlags = CameraClearFlags.SolidColor;
        backgroundCam.backgroundColor = letterboxColor;
        backgroundCam.cullingMask = 0; // Don't render any objects, just clear screen
        backgroundCam.orthographic = true;
    }

    private void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            UpdateViewport();
        }
    }

    public void UpdateViewport()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        if (Screen.height <= 0 || Screen.width <= 0 || mainCam == null)
            return;

        float currentAspect = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Screen is taller than 16:9 (Portrait phone or iPad) -> Letterbox top & bottom
            Rect rect = mainCam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) * 0.5f;
            mainCam.rect = rect;
        }
        else
        {
            // Screen is wider than 16:9 (Ultrawide mobile landscape 19.5:9 or 20:9) -> Pillarbox left & right
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = mainCam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) * 0.5f;
            rect.y = 0;
            mainCam.rect = rect;
        }
    }
}
