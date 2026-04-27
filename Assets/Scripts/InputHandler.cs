using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement player; // Reference to the player's movement handler
    
    [Header("Camera Settings")]
    [SerializeField] private float cameraYawSensitivity = 0.15f; // Camera sensitivity
    [SerializeField] private float pinchSensitivity = 0.15f;
    
    /// <summary>
    /// Stores each Touch input by their fingerIDs and categorizes them based on
    /// their TouchType and what they're controlling. This allows for multi-input
    /// functionality.
    /// </summary>
    private Dictionary<TouchType, HashSet<int>> touchDict = new()
    {
        {TouchType.Joystick, new HashSet<int>()},
        {TouchType.Camera, new HashSet<int>()},
    };
    
    // Pinch gesture parameters
    private float prevPinchDist;
    private bool isPinching;
    
    // Screen space boundaries
    private float halfScreenWidth; // Divide the screen space width in half
    
    /// <summary>
    /// Touch input roles.
    /// </summary>
    enum TouchType
    {
        /// <summary>
        /// Touch has no role.
        /// </summary>
        None,
        
        /// <summary>
        /// Touch controls the joystick.
        /// </summary>
        Joystick,
        
        /// <summary>
        /// Touch controls the camera position.
        /// </summary>
        Camera,
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        halfScreenWidth = Screen.width / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches) // Check each finger touching the screen
        {
            // TODO: Tower dragging?
            if (EventSystem.current &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                continue;
            }

            switch (touch.phase) // Determine current touch phase
            {
                case TouchPhase.Began:
                    // Check if the touch input is on the left side of the screen
                    // Joystick can only be controlled using one finger 
                    if (touch.position.x < halfScreenWidth && touchDict[TouchType.Joystick].Count == 0 &&
                        player.moveJoystick.TryBegin(touch, halfScreenWidth)) // TryBegin() ensures finger can operate the joystick
                        touchDict[TouchType.Joystick].Add(touch.fingerId); // Set finger as Joystick type using its fingerId
                    // Camera control can only happen on the right side of the screen using only one finger
                    // Do not move the camera while pinching
                    else if (touch.position.x >= halfScreenWidth && touchDict[TouchType.Camera].Count == 0 && 
                             !isPinching) 
                        touchDict[TouchType.Camera].Add(touch.fingerId); // Set finger as Camera type using its fingerId
                    break;

                case TouchPhase.Moved:
                    // Check if the current finger is used to move the camera
                    if (touchDict[TouchType.Camera].Contains(touch.fingerId))
                    {
                        // Get the delta position of the finger and use it to move the camera
                        Vector2 lookInput = touch.deltaPosition * cameraYawSensitivity;
                        player.HandleCameraRotation(lookInput);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    // Determine what the finger was controlling using its ID
                    TouchType type = GetTouchType(touch.fingerId);
                    switch (type)
                    {
                        // Remove the finger's ID from the TouchType in the dictionary
                        case TouchType.Joystick:
                            touchDict[TouchType.Joystick].Remove(touch.fingerId);
                            break;
                        case TouchType.Camera:
                            touchDict[TouchType.Camera].Remove(touch.fingerId);
                            break;
                    }
                    break;
            }
            
            HandlePinchZoom();

            // Only allow the finger that is controlling the joystick to handle joystick logic
            if (!player.moveJoystick || !player.moveJoystick.IsMyFinger(touch.fingerId)) continue;
            player.moveJoystick.ProcessTouch(touch); // Have the joystick process touches
        }
    }

    /// <summary>
    /// Detects and handles the pinching gesture to control how close the camera is
    /// to the player.
    /// </summary>
    void HandlePinchZoom()
    {
        bool wasPinching = isPinching; // Store the previous pinching state
        
        List<Touch> pinchTouches = new(); // List of valid pinching inputs
        foreach (Touch touch in Input.touches)
        {
            // Do not detect the finger controlling the joystick
            if (touchDict[TouchType.Joystick].Contains(touch.fingerId)) continue;
            
            pinchTouches.Add(touch); // Add the touch to the potential pinch gesture
        }

        if (pinchTouches.Count != 2) // Pinching needs exactly 2 fingers
        {
            isPinching = false;
            return;
        }
        
        isPinching = true;
        // Make sure no finger is moving the camera when one of the two fingers are lifted and replaced
        if (!wasPinching && isPinching) touchDict[TouchType.Camera].Clear();
        
        // Get the two fingers
        Touch touch0 = pinchTouches[0];
        Touch touch1 = pinchTouches[1];

        // Store the previous positions of the fingers
        Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        // Calculate the previous and current distances between both fingers
        float prevDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);

        // Get the difference between both distances and let PlayerMovement handle camera zoom
        float pinchDelta = currentDistance - prevDistance;
        player.HandleCameraZoom(pinchDelta);
    }

    /// <summary>
    /// Returns the attribute of the Touch input based on where its fingerID
    /// is stored in touchDict's TouchTypes.
    /// </summary>
    /// <param name="fingerId">Integer ID of the Touch object</param>
    /// <returns>TouchType</returns>
    TouchType GetTouchType(int fingerId)
    {
        TouchType type = TouchType.None;
        if (touchDict[TouchType.Joystick].Contains(fingerId)) type = TouchType.Joystick;
        if (touchDict[TouchType.Camera].Contains(fingerId)) type = TouchType.Camera;
        return type;
    }
}
