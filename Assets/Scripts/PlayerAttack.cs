using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Sword Auto Attack")]
    [SerializeField] private float swordRange = 2f;
    [SerializeField] private float swordDamage = 2f;
    [SerializeField] private float swordCooldown = 0.75f;

    [Header("Projectile Attack")]
    [SerializeField] private float projectileRange = 8f;
    [SerializeField] private float projectileDamage = 1f;
    [SerializeField] private float projectileCooldown = 1.25f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Sword Visual")]
    [SerializeField] private GameObject swordObject;
    [SerializeField] private float swordShowTime = 0.25f;

    [Header("Gun Visual")]
    [SerializeField] private GameObject gunObject;
    [SerializeField] private float gunShowTime = 0.2f;
    [SerializeField] private Vector3 gunRotationOffset = new Vector3(0f, 90f, 0f);

    private float nextSwordAttackTime;
    private float nextProjectileAttackTime;
    private Coroutine swordRoutine;
    private Coroutine gunRoutine;

    private void Start()
    {
        if (swordObject != null)
            swordObject.SetActive(false);

        if (gunObject != null)
            gunObject.SetActive(false);
    }

    private void Update()
    {
        Enemy closeEnemy = FindNearestEnemy(swordRange);

        if (closeEnemy != null)
        {
            TrySwordAttack(closeEnemy);
            return;
        }

        Enemy farEnemy = FindNearestEnemy(projectileRange);

        if (farEnemy != null)
        {
            TryProjectileAttack(farEnemy);
        }
    }

    private void TrySwordAttack(Enemy enemy)
    {
        if (Time.time < nextSwordAttackTime)
            return;

        enemy.TakeDamage(swordDamage);

        if (swordRoutine != null)
            StopCoroutine(swordRoutine);

        swordRoutine = StartCoroutine(SwingSword());

        nextSwordAttackTime = Time.time + swordCooldown;
    }

    private void TryProjectileAttack(Enemy enemy)
    {
        if (Time.time < nextProjectileAttackTime)
            return;

        if (projectilePrefab == null || projectileSpawnPoint == null)
            return;

        Vector3 direction = enemy.transform.position - projectileSpawnPoint.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);

        if (gunObject != null)
            gunObject.transform.rotation = lookRotation * Quaternion.Euler(gunRotationOffset);

        GameObject proj = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            lookRotation
        );

        Projectile projectile = proj.GetComponent<Projectile>();

        if (projectile != null)
            projectile.SetTarget(enemy, projectileDamage);

        if (gunRoutine != null)
            StopCoroutine(gunRoutine);

        gunRoutine = StartCoroutine(ShowGun());

        nextProjectileAttackTime = Time.time + projectileCooldown;
    }

    private Enemy FindNearestEnemy(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        Enemy nearest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy == null || !enemy.IsAlive)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private IEnumerator SwingSword()
    {
        if (swordObject == null)
            yield break;

        swordObject.SetActive(true);

        Transform pivot = swordObject.transform.parent != null
            ? swordObject.transform.parent
            : swordObject.transform;

        Quaternion startRot = Quaternion.Euler(0f, -60f, -30f);
        Quaternion endRot = Quaternion.Euler(0f, 60f, 30f);

        float timer = 0f;

        while (timer < swordShowTime)
        {
            timer += Time.deltaTime;
            float t = timer / swordShowTime;

            pivot.localRotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        pivot.localRotation = Quaternion.identity;
        swordObject.SetActive(false);
    }

    private IEnumerator ShowGun()
    {
        if (gunObject == null)
            yield break;

        gunObject.SetActive(true);
        yield return new WaitForSeconds(gunShowTime);
        gunObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, swordRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, projectileRange);
    }
}