using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Enemy enemy;
    private float maxHealth;
    private Camera cam;
    private Canvas canvas;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        cam = Camera.main;
        canvas = GetComponent<Canvas>();

        if (enemy != null)
            maxHealth = enemy.MaxHealth;

        // hide at start
        if (canvas != null)
            canvas.enabled = false;
    }

    private void OnEnable()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        if (canvas != null)
            canvas.enabled = false;
    }

    private void LateUpdate()
    {
        if (enemy == null || fillImage == null)
            return;

        float healthPercent = enemy.CurrentHealth / maxHealth;

        // update bar
        fillImage.fillAmount = healthPercent;

        // only show when damaged
        if (canvas != null)
            canvas.enabled = healthPercent < 1f && healthPercent > 0f;

        // face camera
        if (cam != null)
        {
            transform.rotation = Quaternion.LookRotation(
                cam.transform.position - transform.position
            );
        }
    }
}