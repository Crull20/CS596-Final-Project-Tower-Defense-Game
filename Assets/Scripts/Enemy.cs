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

    // used by towers or other systems to stop targeting enemy
    // when it dies, reaches end, or is disabled by pooling
    public event Action<Enemy> BecameUnavailable;
    // fired when enemy dies
    public event Action<Enemy> OnDeath;
    // fired whenever enemy takes damage
    public event Action OnTakeDamage;

    public bool deathHandled = false;

    public bool IsAlive => gameObject.activeInHierarchy && currentHealth > 0f;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public void SetHealthBar(HealthBar targetHealthBar)
    {
        // pool assigns shared base/player health bar to each enemy
        healthBar = targetHealthBar;
    }

    private void OnEnable()
    {
        // resent enemy state each time it is reused from the object pool
        currentPathingPointIndex = 0;
        currentHealth = maxHealth;

        // start moving toward the first path point.
        if (currentPath != null)
            targetPosition = currentPath.GetPosition(currentPathingPointIndex);
    }

    private void Update()
    {
        // do nothing if no path assigned
        if (currentPath == null) return;
        if (currentHealth <= 0f) return;

        // stop moving if enemy is dead and waiting for death animation
        Vector3 dir = (targetPosition - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            // negative direction is used because model faces -Z
            Quaternion targetRot = Quaternion.LookRotation(-dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        // move toward current target path point
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // check if enemy reached the current path point
        float relativeDistance = (transform.position - targetPosition).magnitude;

        if (relativeDistance < 0.1f)
        {
            // move to next path point if one exists
            if (currentPathingPointIndex < currentPath.Pathingpoints.Length - 1)
            {
                currentPathingPointIndex++;
                targetPosition = currentPath.GetPosition(currentPathingPointIndex);
            }
            else
            {
                // no more path points
                ReachEndOfPath();
            }
        }
    }

    private void ReachEndOfPath()
    {
        // damage players base health when enemy completes path
        if (healthBar != null)
            healthBar.DrainHealth(endPathDamage);

        // return enemy to object pool
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        // ignore damage if enemy is already inactive
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
        // notify towers and other systems that this enemy is no longer targetable
        BecameUnavailable?.Invoke(this);
    }
}