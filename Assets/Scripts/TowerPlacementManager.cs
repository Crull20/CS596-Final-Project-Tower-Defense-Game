using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TowerPlacementManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private TextMeshProUGUI towerCounter;

    [Header("Placement")]
    [SerializeField] private LayerMask placementLayers;
    [SerializeField] private float yOffset = 0f;

    // tracks tower inventory and current preview object
    private int towerCount;
    private GameObject previewInstance;
    private PlaceableTower previewTower;
    private GameObject pendingPrefab;

    // tracks whether player is currently placing a tower
    private bool isPlacing;
    private bool hasValidPlacement;

    // stores the latest valid placement position and touch finger
    private Vector3 lastValidPosition;
    private int placementFingerId = -1;

    private void Awake()
    { 
        // sets up the camera and hides placement buttons
        if (mainCamera == null)
            mainCamera = Camera.main;

        // if (confirmButton != null)
        //     confirmButton.onClick.AddListener(ConfirmPlacement);
        //
        // if (cancelButton != null)
        //     cancelButton.onClick.AddListener(CancelPlacement);

        SetPlacementButtonsVisible(false);
    }

    private void Update()
    {
        // updates tower preview while placement is active
        if (!isPlacing || previewInstance == null)
            return;

        towerCounter.text = "x" + towerCount.ToString();

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMousePlacement();
#else
        HandleTouchPlacement();
#endif
    }

    public void BeginPlacement(GameObject towerPrefab)
    {
        // starts tower placement and creates a preview tower
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
        placementFingerId = -1;

        if (playerMovement != null)
            playerMovement.CanMove = false;

        SetPlacementButtonsVisible(true);
    }

    public void ConfirmPlacement()
    {
        // places tower if preview position is valid
        if (!isPlacing || previewInstance == null || !hasValidPlacement)
            return;

        previewInstance.transform.position = lastValidPosition;
        previewTower.SetPreviewMode(false);

        previewInstance = null;
        previewTower = null;
        pendingPrefab = null;
        isPlacing = false;
        hasValidPlacement = false;
        placementFingerId = -1;

        if (playerMovement != null)
            playerMovement.CanMove = true;

        SetPlacementButtonsVisible(false);
    }

    public void CancelPlacement()
    {
        // cancels placement and removes preview tower
        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = null;
        previewTower = null;
        pendingPrefab = null;
        isPlacing = false;
        hasValidPlacement = false;
        placementFingerId = -1;

        if (playerMovement != null)
            playerMovement.CanMove = true;

        SetPlacementButtonsVisible(false);
    }

    private void HandleTouchPlacement()
    {

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (placementFingerId == -1)
            {
                if (touch.phase == TouchPhase.Began &&
                    !IsPointerOverUI(touch))
                {
                    placementFingerId = touch.fingerId;
                    UpdatePreviewPosition(touch.position);
                    return;
                }
            }
            else if (touch.fingerId == placementFingerId)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    UpdatePreviewPosition(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    placementFingerId = -1;
                }

                return;
            }
        }
    }

    private void HandleMousePlacement()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        UpdatePreviewPosition(Input.mousePosition);
    }

    private void UpdatePreviewPosition(Vector2 screenPos)
    {
        // moves preview tower to a valid placement surface
        if (mainCamera == null || previewInstance == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, placementLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = hit.point;
            pos.y += yOffset;

            previewInstance.transform.position = pos;
            lastValidPosition = pos;
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

    private bool IsPointerOverUI(Touch touch)
    {
        // checks if placement touch is over UI
        if (EventSystem.current == null)
            return false;

        return inputHandler.IsOverUI(touch);
    }

    private void SetPlacementButtonsVisible(bool visible)
    {
        // shows or hides placement confirm and cancel buttons
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(visible);

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(visible);
    }

    public bool IsPlacing => isPlacing;

    public int TowerCount => towerCount;

    public void AddTower()
    {
        // Add a tower to the player's inventory when a tower is bought
        towerCount++;
    }
}