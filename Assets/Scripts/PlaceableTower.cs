using UnityEngine;

public class PlaceableTower : MonoBehaviour
{
    [SerializeField] private TowerAttack towerAttack;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private TowerRangeIndicator rangeIndicator;

    [Header("Preview Material")]
    [SerializeField] private Material previewMaterialTemplate;

    // stores preview materials and placement state
    private Material runtimePreviewMaterial;
    private Material[][] originalMaterials;
    private bool isPreview;

    private static readonly int PlacementValidId = Shader.PropertyToID("_PlacementValid");

    private void Awake()
    {
        // find missing tower components automatically
        if (towerAttack == null)
            towerAttack = GetComponent<TowerAttack>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (colliders == null || colliders.Length == 0)
            colliders = GetComponentsInChildren<Collider>(true);

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        if (rangeIndicator == null)
            rangeIndicator = GetComponentInChildren<TowerRangeIndicator>(true);

        CacheOriginalMaterials();
    }

    private void CacheOriginalMaterials()
    {
        // sasves the tower's original materials so they can be restored later
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                originalMaterials[i] = renderers[i].sharedMaterials;
        }
    }

    public void SetPreviewMode(bool preview)
    {
        // enables or disables placement preview behavior
        isPreview = preview;

        if (towerAttack != null)
            towerAttack.enabled = !preview;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                colliders[i].enabled = !preview;
        }

        if (rangeIndicator != null)
        {
            rangeIndicator.Refresh();
            rangeIndicator.SetVisible(preview);
        }

        if (preview)
        {
            ApplyPreviewMaterial();
            SetPlacementValid(true);
        }
        else
        {
            RestoreOriginalMaterials();
        }
    }

    public void SetPlacementValid(bool valid)
    {
        // updates the preview material to show valid or invalid placement
        if (!isPreview || runtimePreviewMaterial == null)
            return;

        runtimePreviewMaterial.SetFloat(PlacementValidId, valid ? 1f : 0f);
    }

    private void ApplyPreviewMaterial()
    {
        // applies the preview material to every tower renderer
        if (previewMaterialTemplate == null)
        {
            Debug.LogWarning("PlaceableTower: Preview Material Template is not assigned.");
            return;
        }

        if (runtimePreviewMaterial == null)
            runtimePreviewMaterial = new Material(previewMaterialTemplate);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
                continue;

            Material[] mats = new Material[renderers[i].sharedMaterials.Length];

            for (int j = 0; j < mats.Length; j++)
                mats[j] = runtimePreviewMaterial;

            renderers[i].materials = mats;
        }
    }

    private void RestoreOriginalMaterials()
    {
        // restores the tower's normal materials after placement
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && originalMaterials[i] != null)
                renderers[i].sharedMaterials = originalMaterials[i];
        }

        if (runtimePreviewMaterial != null)
        {
            Destroy(runtimePreviewMaterial);
            runtimePreviewMaterial = null;
        }
    }
}