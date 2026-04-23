using System;
using UnityEngine;

/*
Enemy moves along a path, can take damage, and notifies listeners when it leaves play.
This works well with pooling because OnEnable resets state and OnDisable fires when the
pooled object is turned off.
*/

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    public Path currentPath;

    [Header("Combat")]
    [SerializeField] private float maxHealth = 10f;

    public event Action<Enemy> BecameUnavailable;

    private Vector3 targetPosition;
    private int currentPathingPointIndex = 0;
    private float currentHealth;

    public bool IsAlive => gameObject.activeInHierarchy && currentHealth > 0f;
    public float CurrentHealth => currentHealth;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        currentPathingPointIndex = 0;

        if (currentPath != null)
            targetPosition = currentPath.GetPosition(currentPathingPointIndex);
    }

    private void Update()
    {
        if (currentPath == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

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

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeInHierarchy)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    private void ReachEndOfPath()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        BecameUnavailable?.Invoke(this);
    }
}