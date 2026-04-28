using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TowerAttack : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float attackRadius = 5f;

    [Header("Projectile Attack")]
    [SerializeField] private float shotsPerSecond = 1f;
    [SerializeField] private float projectileDamage = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Optional Rotation")]
    [SerializeField] private Transform turretHead;
    [SerializeField] private float turnSpeed = 10f;

    private readonly List<Enemy> enemiesInRange = new List<Enemy>();

    private Enemy currentTarget;
    private float attackTimer;
    private SphereCollider rangeCollider;

    private void Awake()
    {
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = attackRadius;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnValidate()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.isTrigger = true;
            col.radius = attackRadius;
        }
    }

    private void Update()
    {
        CleanupInvalidEnemies();
        SelectTarget();

        if (currentTarget == null)
        {
            attackTimer = 0f;
            return;
        }

        RotateTowardTarget();

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            FireProjectile(currentTarget);
            attackTimer = 1f / shotsPerSecond;
        }
    }

    private void FireProjectile(Enemy target)
    {
        if (target == null || !target.IsAlive)
            return;

        if (projectilePrefab == null)
            return;

        Transform spawn = projectileSpawnPoint != null ? projectileSpawnPoint : transform;

        Vector3 direction = target.transform.position - spawn.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            direction = transform.forward;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);

        GameObject projectileObj = Instantiate(
            projectilePrefab,
            spawn.position,
            lookRotation
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetTarget(target, projectileDamage);
        }
        else
        {
            Debug.LogWarning("Tower projectile prefab is missing a Projectile component.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null || enemiesInRange.Contains(enemy))
            return;

        enemiesInRange.Add(enemy);
        enemy.BecameUnavailable += OnEnemyUnavailable;
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

        RemoveEnemy(enemy);
    }

    private void OnEnemyUnavailable(Enemy enemy)
    {
        RemoveEnemy(enemy);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        if (enemy == null)
            return;

        enemy.BecameUnavailable -= OnEnemyUnavailable;
        enemiesInRange.Remove(enemy);

        if (currentTarget == enemy)
            currentTarget = null;
    }

    private void CleanupInvalidEnemies()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemiesInRange[i];

            if (enemy == null || !enemy.gameObject.activeInHierarchy || !enemy.IsAlive)
            {
                if (enemy != null)
                    enemy.BecameUnavailable -= OnEnemyUnavailable;

                enemiesInRange.RemoveAt(i);
            }
        }
    }

    private void SelectTarget()
    {
        if (currentTarget != null && currentTarget.IsAlive && currentTarget.gameObject.activeInHierarchy)
            return;

        currentTarget = null;

        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            Enemy enemy = enemiesInRange[i];
            if (enemy != null && enemy.IsAlive && enemy.gameObject.activeInHierarchy)
            {
                currentTarget = enemy;
                break;
            }
        }
    }

    private void RotateTowardTarget()
    {
        Transform partToRotate = turretHead != null ? turretHead : transform;

        Vector3 direction = currentTarget.transform.position - partToRotate.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        partToRotate.rotation = Quaternion.Slerp(
            partToRotate.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void OnDisable()
    {
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            if (enemiesInRange[i] != null)
                enemiesInRange[i].BecameUnavailable -= OnEnemyUnavailable;
        }

        enemiesInRange.Clear();
        currentTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}