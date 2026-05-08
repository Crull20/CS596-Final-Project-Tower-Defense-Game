using System;
using UnityEngine;

/*
This class represents an enemy in the game. It moves along a predefined path by following a series of 
pathing points.
*/

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    public Path currentPath;

    [Header("Combat")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float endPathDamage = 10f;

    private float currentHealth;
    private Vector3 targetPosition;
    private int currentPathingPointIndex = 0;

    private HealthBar healthBar;

    public event Action<Enemy> BecameUnavailable;
    public event Action<Enemy> OnDeath;
    public event Action OnTakeDamage;

    public bool deathHandled = false;

    public bool IsAlive => gameObject.activeInHierarchy && currentHealth > 0f;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public void SetHealthBar(HealthBar targetHealthBar)
    {
        healthBar = targetHealthBar;
    }

    private void OnEnable()
    {
        currentPathingPointIndex = 0;
        currentHealth = maxHealth;

        if (currentPath != null)
            targetPosition = currentPath.GetPosition(currentPathingPointIndex);
    }

    private void Update()
    {
        if (currentPath == null) return;
        if (currentHealth <= 0f) return;

        Vector3 dir = (targetPosition - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(-dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        float relativeDistance = (transform.position - targetPosition).magnitude;

        if (relativeDistance < 0.1f)
        {
            if (currentPathingPointIndex < currentPath.Pathingpoints.Length - 1)
            {
                currentPathingPointIndex++;
                targetPosition = currentPath.GetPosition(currentPathingPointIndex);
            }
            else
            {
                ReachEndOfPath();
            }
        }
    }

    private void ReachEndOfPath()
    {
        if (healthBar != null)
            healthBar.DrainHealth(endPathDamage);

        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeInHierarchy)
            return;

        currentHealth -= damage;
        OnTakeDamage?.Invoke();

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            deathHandled = false;
            OnDeath?.Invoke(this);

            if (!deathHandled)
                gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        BecameUnavailable?.Invoke(this);
    }
}