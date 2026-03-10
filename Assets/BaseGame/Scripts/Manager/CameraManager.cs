using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public BoxCollider2D cameraCollider;
    public Camera mainCamera;
    public bool matchAspect = false;
    public bool fitFully = true;
    private void Start()
    {
        FitCameraToCollider();
    }

    // Call from inspector context menu or editor
    [ContextMenu("Fit Camera To Collider")]
    public void FitCameraToCollider()
    {
        if (cameraCollider == null || mainCamera == null)
            return;

        if (!mainCamera.orthographic)
        {
            Debug.LogWarning("Main camera must be orthographic.");
            return;
        }

        // world-space bounds of the collider (accounts for scale)
        var b = cameraCollider.bounds;
        var worldHeight = b.size.y;
        var worldWidth = b.size.x;

        if (worldHeight <= 0f || worldWidth <= 0f)
            return;

        if (matchAspect)
        {
            // Force camera aspect to match collider ratio (exact match possible)
            mainCamera.aspect = worldWidth / worldHeight;
        }

        var orthoFromHeight = worldHeight * 0.5f;
        var orthoFromWidth = worldWidth / (2f * mainCamera.aspect);

        mainCamera.orthographicSize = fitFully ? Mathf.Max(orthoFromHeight, orthoFromWidth) : orthoFromHeight;

        // Center camera on collider (preserve camera.z)
        var camPos = mainCamera.transform.position;
        camPos.x = b.center.x;
        camPos.y = b.center.y;
        mainCamera.transform.position = camPos;
    }
}
