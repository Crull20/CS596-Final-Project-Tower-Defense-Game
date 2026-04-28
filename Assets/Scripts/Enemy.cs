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

    // combat
    [Header("Combat")]
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    private Vector3 targetPosition;
    private int currentPathingPointIndex = 0;

    // lets towers know this enemy is no longer a valid target
    public event Action<Enemy> BecameUnavailable;

    // needed for scripts
    public bool IsAlive => gameObject.activeInHierarchy && currentHealth > 0f;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void OnEnable()
    {
        //reset the pathing point index to 0 when the enemy is enabled
        currentPathingPointIndex = 0;

        currentHealth = maxHealth;

        //set the target position to the position of the first pathing point
        if (currentPath != null)
            targetPosition = currentPath.GetPosition(currentPathingPointIndex);
    }

    private void Update()
    {
        if (currentPath == null) return;

        //move towards the position of the target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        //check if the enemy has reached the target position, set the target position to the next pathing point if it has
        float relativeDistance = (transform.position - targetPosition).magnitude;

        //if the enemy is within 0.1 units of the target position, consider it reached
        if (relativeDistance < 0.1f)
        {
            //if there are more pathing points, move to the next one
            if (currentPathingPointIndex < currentPath.Pathingpoints.Length - 1)
            {
                //increment the pathing point index and set the target position to the next pathing point
                currentPathingPointIndex++;
                targetPosition = currentPath.GetPosition(currentPathingPointIndex);
            }
            else
            {
                //if there are no more pathing points, the enemy has reached the end of the path
                //deactivate the enemy game object to return it to the pool
                gameObject.SetActive(false);
            }
        }
    }

    // damage function
    public void TakeDamage(float damage)
    {
        if (!gameObject.activeInHierarchy)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        BecameUnavailable?.Invoke(this);
    }
}