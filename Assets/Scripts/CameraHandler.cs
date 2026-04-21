using UnityEngine;
using Unity.Cinemachine;

public class CameraHandler : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private LayerMask cameraObstacleLayers;
    [SerializeField] private float cameraYawSensitivity = 0.15f;
    [SerializeField] private float minCameraDistance = 3f;
    [SerializeField] private float maxCameraDistance = 8f;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float cameraHeight = 4f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
