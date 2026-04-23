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
            if (canvas == null) return null;
            return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }
    }

    private void Awake()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        rootRect = transform as RectTransform;
        canvasRect = canvas.transform as RectTransform;

        HideJoystick();
    }

    public bool IsMyFinger(int fingerId)
    {
        return fingerId == activeFingerId;
    }

    public bool TryBegin(Touch touch, float halfScreenWidth)
    {
        activeFingerId = touch.fingerId;
        gameObject.SetActive(true);

        Vector2 canvasLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            touch.position,
            UICamera,
            out canvasLocalPoint
        );

        rootRect.anchoredPosition = canvasLocalPoint;
        InputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        return true;
    }

    public void ProcessTouch(Touch touch)
    {
        if (touch.fingerId != activeFingerId) return;

        switch (touch.phase)
        {
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                UpdateJoystick(touch.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndTouch();
                break;
        }
    }

    private void UpdateJoystick(Vector2 screenPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootRect,
            screenPosition,
            UICamera,
            out localPoint
        );

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, radius);

        handle.anchoredPosition = clamped;
        InputVector = clamped / radius;
    }

    private void EndTouch()
    {
        activeFingerId = -1;
        HideJoystick();
    }

    private void HideJoystick()
    {
        InputVector = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;

        gameObject.SetActive(false);
    }
}