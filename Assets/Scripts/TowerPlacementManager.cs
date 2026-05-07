using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerPlacementManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Placement")]
    [SerializeField] private LayerMask placementLayers;
    [SerializeField] private float yOffset = 0f;

    private GameObject previewInstance;
    private PlaceableTower previewTower;
    private GameObject pendingPrefab;
    private bool isPlacing;
    private bool hasValidPlacement;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmPlacement);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelPlacement);

        SetPlacementButtonsVisible(false);
    }

    private void Update()
    {
        if (!isPlacing || previewInstance == null)
            return;

        if (TryGetPointerScreenPosition(out Vector2 screenPos, out int pointerId))
        {
            if (!IsPointerOverUI(pointerId))
                UpdatePreviewPosition(screenPos);
        }
    }

    public void BeginPlacement(GameObject towerPrefab)
    {
        if (towerPrefab == null)
            return;

        CancelPlacement();

        pendingPrefab = towerPrefab;
        previewInstance = Instantiate(pendingPrefab);
        previewTower = previewInstance.GetComponent<PlaceableTower>();

        if (previewTower == null)
            previewTower = previewInstance.AddComponent<PlaceableTower>();

        previewTower.SetPreviewMode(true);

        isPlacing = true;
        hasValidPlacement = false;
        SetPlacementButtonsVisible(true);
    }

    public void ConfirmPlacement()
    {
        if (!isPlacing || previewInstance == null || !hasValidPlacement)
            return;

        previewTower.SetPreviewMode(false);

        previewInstance = null;
        previewTower = null;
        pendingPrefab = null;
        isPlacing = false;
        hasValidPlacement = false;

        SetPlacementButtonsVisible(false);
    }

    public void CancelPlacement()
    {
        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = null;
        previewTower = null;
        pendingPrefab = null;
        isPlacing = false;
        hasValidPlacement = false;

        SetPlacementButtonsVisible(false);
    }

    private void UpdatePreviewPosition(Vector2 screenPos)
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, placementLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = hit.point;
            pos.y += yOffset;

            previewInstance.transform.position = pos;
            hasValidPlacement = true;

            if (previewTower != null)
                previewTower.SetPlacementValid(true);
        }
        else
        {
            hasValidPlacement = false;

            if (previewTower != null)
                previewTower.SetPlacementValid(false);
        }
    }

    private bool TryGetPointerScreenPosition(out Vector2 screenPos, out int pointerId)
    {
        screenPos = default;
        pointerId = -1;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            screenPos = touch.position;
            pointerId = touch.fingerId;
            return true;
        }

        if (Input.mousePresent)
        {
            screenPos = Input.mousePosition;
            pointerId = -1;
            return true;
        }

        return false;
    }

    private bool IsPointerOverUI(int pointerId)
    {
        if (EventSystem.current == null)
            return false;

        if (pointerId >= 0)
            return EventSystem.current.IsPointerOverGameObject(pointerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void SetPlacementButtonsVisible(bool visible)
    {
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(visible);

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(visible);
    }
}