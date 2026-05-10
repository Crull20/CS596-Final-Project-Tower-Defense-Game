using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        // store main camera once so health bar/UI object knows what to face
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        // if no camera, stop to prevent errors
        if (cam == null) return;

        // match camera forward direction so object always faces camera
        transform.forward = cam.transform.forward;
    }
}