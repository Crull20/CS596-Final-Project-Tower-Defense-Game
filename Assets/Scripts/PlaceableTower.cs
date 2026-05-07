using UnityEngine;

public class PlaceableTower : MonoBehaviour
{
    [SerializeField] private TowerAttack towerAttack;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private TowerRangeIndicator rangeIndicator;

    [Header("Preview Look")]
    [SerializeField] private Color previewColor = new Color(0f, 1f, 0f, 0.6f);
    [SerializeField] private Color blockedColor = new Color(1f, 0f, 0f, 0.6f);

    private MaterialPropertyBlock propertyBlock;
    private bool isPreview;

    private void Awake()
    {
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

        propertyBlock = new MaterialPropertyBlock();
    }

    public void SetPreviewMode(bool preview)
    {
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

        SetPreviewVisual(previewColor);
    }

    public void SetPlacementValid(bool valid)
    {
        if (!isPreview)
            return;

        SetPreviewVisual(valid ? previewColor : blockedColor);
    }

    private void SetPreviewVisual(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null)
                continue;

            r.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", color);
            propertyBlock.SetColor("_BaseColor", color);
            r.SetPropertyBlock(propertyBlock);
        }
    }
}