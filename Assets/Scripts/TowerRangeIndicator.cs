using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerRangeIndicator : MonoBehaviour
{
    [SerializeField] private TowerAttack towerAttack;
    [SerializeField] private int segments = 64;
    [SerializeField] private float lineYOffset = 0.05f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (towerAttack == null)
            towerAttack = GetComponentInParent<TowerAttack>();

        DrawCircle();
        SetVisible(false);
    }

    public void Refresh()
    {
        DrawCircle();
    }

    public void SetVisible(bool visible)
    {
        if (lineRenderer != null)
            lineRenderer.enabled = visible;
    }

    private void DrawCircle()
    {
        if (lineRenderer == null || towerAttack == null)
            return;

        float radius = towerAttack.AttackRadius;

        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments;
            float angle = t * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, lineYOffset, z));
        }
    }
}