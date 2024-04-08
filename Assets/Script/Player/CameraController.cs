using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Vector3 thirdPersonCameraOffset = new Vector3(0, 2, -3);
    [SerializeField] private float minZoom = -2f;
    [SerializeField] private float maxZoom = -10f;
    [SerializeField] private float zoomSensitivity = 2f;

    private void Start()
    {
       thirdPersonCamera.enabled = false; // Initially disable third person camera
    }

    private void Update()
    {
        HandleCameraSwitching();
        HandleCameraZoom();
    }

    private void HandleCameraSwitching()
    {
        if (Input.GetKeyDown(KeyCode.C)) // 'C' key to switch camera
        {
            firstPersonCamera.enabled = !firstPersonCamera.enabled;
            thirdPersonCamera.enabled = !thirdPersonCamera.enabled;
        }
    }

    private void HandleCameraZoom()
    {
        if (thirdPersonCamera.enabled)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            thirdPersonCameraOffset.z = Mathf.Clamp(thirdPersonCameraOffset.z + scroll * zoomSensitivity, 
                maxZoom, minZoom);
            thirdPersonCamera.transform.localPosition = thirdPersonCameraOffset;
        }
    }
}