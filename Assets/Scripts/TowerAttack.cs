using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TowerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private float attacksPerSecond = 1f;
    [SerializeField] private float damagePerHit = 1f;

    [Header("Optional Visuals")]
    [SerializeField] private Transform turretHead;
    [SerializeField] private bool rotateTowardTarget = true;
    [SerializeField] private float turnSpeed = 10f;

    private readonly List<Enemy> enemyQueue = new();
    private readonly Dictionary<Enemy, int> overlapCounts = new();

    private SphereCollider rangeCollider;
    private Enemy currentTarget;
    private float attackCooldown;

    private void Reset()
    {
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = attackRadius;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Awake()
    {
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = attackRadius;
    }

    private void OnValidate()
    {
        if (attackRadius < 0f) attackRadius = 0f;
        if (attacksPerSecond < 0.01f) attacksPerSecond = 0.01f;
        if (damagePerHit < 0f) damagePerHit = 0f;

        if (rangeCollider == null)
            rangeCollider = GetComponent<SphereCollider>();

        if (rangeCollider != null)
        {
            rangeCollider.isTrigger = true;
            rangeCollider.radius = attackRadius;
        }
    }

    private void Update()
    {
        CleanupQueue();
        UpdateCurrentTarget();

        if (currentTarget == null)
            return;

        if (rotateTowardTarget)
            RotateTowardCurrentTarget();

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            AttackCurrentTarget();
            attackCooldown = 1f / attacksPerSecond;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

        if (overlapCounts.TryGetValue(enemy, out int count))
        {
            overlapCounts[enemy] = count + 1;
            return;
        }

        overlapCounts.Add(enemy, 1);
        enemyQueue.Add(enemy);
        enemy.BecameUnavailable += HandleEnemyUnavailable;

        if (currentTarget == null)
            UpdateCurrentTarget();
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

        if (!overlapCounts.TryGetValue(enemy, out int count))
            return;

        count--;

        if (count <= 0)
        {
            RemoveEnemy(enemy);
        }
        else
        {
            overlapCounts[enemy] = count;
        }
    }

    private void HandleEnemyUnavailable(Enemy enemy)
    {
        RemoveEnemy(enemy);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        if (enemy == null)
            return;

        if (overlapCounts.ContainsKey(enemy))
            overlapCounts.Remove(enemy);

        if (enemyQueue.Contains(enemy))
            enemyQueue.Remove(enemy);

        enemy.BecameUnavailable -= HandleEnemyUnavailable;

        if (currentTarget == enemy)
            currentTarget = null;

        UpdateCurrentTarget();
    }

    private void CleanupQueue()
    {
        for (int i = enemyQueue.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemyQueue[i];

            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                if (enemy != null)
                    enemy.BecameUnavailable -= HandleEnemyUnavailable;

                enemyQueue.RemoveAt(i);

                if (enemy != null && overlapCounts.ContainsKey(enemy))
                    overlapCounts.Remove(enemy);
            }
        }
    }

    private void UpdateCurrentTarget()
    {
        currentTarget = null;

        for (int i = 0; i < enemyQueue.Count; i++)
        {
            Enemy enemy = enemyQueue[i];

            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                currentTarget = enemy;
                return;
            }
        }
    }

    private void AttackCurrentTarget()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            UpdateCurrentTarget();
            return;
        }

        currentTarget.TakeDamage(damagePerHit);

        if (!currentTarget.gameObject.activeInHierarchy)
            UpdateCurrentTarget();
    }

    private void RotateTowardCurrentTarget()
    {
        Transform partToRotate = turretHead != null ? turretHead : transform;

        Vector3 direction = currentTarget.transform.position - partToRotate.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        partToRotate.rotation = Quaternion.Slerp(
            partToRotate.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void OnDisable()
    {
        for (int i = 0; i < enemyQueue.Count; i++)
        {
            if (enemyQueue[i] != null)
                enemyQueue[i].BecameUnavailable -= HandleEnemyUnavailable;
        }

        enemyQueue.Clear();
        overlapCounts.Clear();
        currentTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}