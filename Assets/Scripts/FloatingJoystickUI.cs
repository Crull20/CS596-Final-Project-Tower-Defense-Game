using UnityEngine;

public class FloatingJoystickUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float radius = 80f;

    public Vector2 InputVector { get; private set; }
    public bool IsHeld => activeFingerId != -1;

    private int activeFingerId = -1;
    private RectTransform canvasRect;
    private RectTransform rootRect;

    private Camera UICamera
    {
        get
        {
            // overlay canvases do not need a UI camera
            // camera/world-space canvases use canvas worldcamera
            if (canvas == null) return null;
            return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }
    }

    private void Awake()
    {
        // find parent canvas
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // cache recttransforms for converting screen touches to UI positions
        rootRect = transform as RectTransform;
        canvasRect = canvas.transform as RectTransform;

        // Hid joystick until player touches movement area
        HideJoystick();
    }

    public bool IsMyFinger(int fingerId)
    {
        // used by input system to check whether this touch belongs to joystick
        return fingerId == activeFingerId;
    }

    public bool TryBegin(Touch touch, float halfScreenWidth)
    {
        // store finger that started controlling joystick
        activeFingerId = touch.fingerId;
        gameObject.SetActive(true);

        // convert touch screen position into a local canvas potitino
        Vector2 canvasLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            touch.position,
            UICamera,
            out canvasLocalPoint
        );

        // move joystick root to where touch started
        rootRect.anchoredPosition = canvasLocalPoint;

        // reset input and handle position at the beginning of the touch
        InputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        return true;
    }

    public void ProcessTouch(Touch touch)
    {
        // ignore touches that do not belong to this joystick
        if (touch.fingerId != activeFingerId) return;

        switch (touch.phase)
        {
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                // update joystick direction while finger is held or moved
                UpdateJoystick(touch.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                // release joystick when finger lifts or touch is canceled
                EndTouch();
                break;
        }
    }

    private void UpdateJoystick(Vector2 screenPosition)
    {
        // convert current finger position into local space relative to joystick root
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootRect,
            screenPosition,
            UICamera,
            out localPoint
        );

        // limit handle so it cannot move outside the joystick radius
        Vector2 clamped = Vector2.ClampMagnitude(localPoint, radius);

        // move joystick handle visually
        handle.anchoredPosition = clamped;

        // normalize handle position into a -1 to 1 input vector
        InputVector = clamped / radius;
    }

    public void EndTouch()
    {
        // clear active finger and hide/reset joystick
        activeFingerId = -1;
        HideJoystick();
    }

    private void HideJoystick()
    {
        // stop movement input
        InputVector = Vector2.zero;

        // reset handle back to center
        if (handle != null)
            handle.anchoredPosition = Vector2.zero;

        // hide floating joystick until next touch begins
        gameObject.SetActive(false);
    }
}