using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    [Header("Inscribed")]
    public float maxHealth = 100f;
    public float drainAnimSpeed = 2f;

    [Header("UI Elements")]
    public Image healthBar;

    private float currentHealth = 100f;
    void Start()
    {
        healthBar ??= GameObject.Find("FillHealth").GetComponent<Image>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Change fill amount to reflect how much health the player has
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, Time.deltaTime * drainAnimSpeed);
        if (currentHealth <= 0f) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the round
    }

    public void DrainHealth(float damage)
    {
        // Subtract the damage from the player's health and clamp it between 0 and the max health
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
