using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    [Header("Inscribed")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float drainAnimSpeed = 2f;

    [Header("UI Elements")]
    [SerializeField] private Image healthBar;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar == null)
        {
            Debug.LogError("HealthBar: Fill Image is not assigned in the Inspector.");
            enabled = false;
            return;
        }

        healthBar.fillAmount = 1f;
    }

    private void Update()
    {
        healthBar.fillAmount = Mathf.Lerp(
            healthBar.fillAmount,
            currentHealth / maxHealth,
            Time.deltaTime * drainAnimSpeed
        );

        if (currentHealth <= 0f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DrainHealth(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Base took {damage} damage. Current health: {currentHealth}/{maxHealth}");
    }
}