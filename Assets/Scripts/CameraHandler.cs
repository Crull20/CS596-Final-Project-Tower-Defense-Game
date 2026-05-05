using System.Net;
using UnityEngine;
using Unity.Cinemachine;

public class CameraHandler : MonoBehaviour
{
    [Header("Camera")] 
    [SerializeField] private CinemachineOrbitalFollow followCamera;
    [SerializeField] private LayerMask cameraObstacleLayers;
    [SerializeField] private float cameraYawSensitivity = 0.15f;
    [SerializeField] private float minCameraDistance = 3f;
    [SerializeField] private float maxCameraDistance = 8f;
    [SerializeField] private float zoomSpeed = 0.15f;

    private float currentCameraDistance = 6f;
    private Vector2 lookInput;
    private Vector2 yaw;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        followCamera = !followCamera ? GetComponent<CinemachineOrbitalFollow>() : followCamera;
    }
    
    public void HandleCameraRotation(Vector2 input)
    {
        // Calculate the final camera position with camera sensitivity
        lookInput = input * cameraYawSensitivity;
        yaw.x += lookInput.x;
        yaw.y -= lookInput.y; // Follows finger direction: swipe down => camera looks down, swipe up => camera looks up
        // Clamp the vertical axis to avoid wrapping around the player and flipping the forward position
        // Values higher than 90 or lower than -90 will flip the forward position
        yaw.y = Mathf.Clamp(yaw.y, -89.5f, 89.5f);

        if (!followCamera) return; // Must have a valid reference to a Cinemachine camera
        
        // Orbital Follow Cinemachine cameras use the horizontal and vertical axes for camera positioning
        followCamera.HorizontalAxis.Value = yaw.x;
        followCamera.VerticalAxis.Value = yaw.y;
    }
    
    public void HandleCameraZoom(float pinchDelta)
    {
        // Calculate the camera distance with the camera zoom speed
        // Clamp the distance
        currentCameraDistance -= pinchDelta * zoomSpeed;
        currentCameraDistance = Mathf.Clamp(currentCameraDistance, minCameraDistance, maxCameraDistance);
        // Orbital Follow Cinemachine cameras use Radius as the camera distance
        followCamera.Radius = currentCameraDistance;
    }
}
