using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InputHandler : MonoBehaviour
{
    private PlayerMovement player; // Reference to the player's movement handler
    
    // Stores the finger IDs according to what they're touching
    // Allows for multi-touch input
    private Dictionary<TouchType, HashSet<int>> touchDict = new()
    {
        {TouchType.Joystick, new HashSet<int>()},
        {TouchType.Camera, new HashSet<int>()},
        {TouchType.Gesture, new HashSet<int>()}
    };

    private float halfScreenWidth; // Divide the screen space width in half
    [SerializeField] private float cameraYawSensitivity = 0.15f; // Camera sensitivity
    
    // Touch input roles
    enum TouchType
    {
        None,
        Joystick,
        Camera,
        Gesture
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
                    else if (touch.position.x >= halfScreenWidth && touchDict[TouchType.Camera].Count == 0) 
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

            // Only allow the finger that is controlling the joystick to handle joystick logic
            if (!player.moveJoystick || !player.moveJoystick.IsMyFinger(touch.fingerId)) continue;
            player.moveJoystick.ProcessTouch(touch); // Have the joystick process touches
        }
    }

    TouchType GetTouchType(int fingerId)
    {
        TouchType type = TouchType.None;
        if (touchDict[TouchType.Joystick].Contains(fingerId)) type = TouchType.Joystick;
        if (touchDict[TouchType.Camera].Contains(fingerId)) type = TouchType.Camera;
        return type;
    }
}
