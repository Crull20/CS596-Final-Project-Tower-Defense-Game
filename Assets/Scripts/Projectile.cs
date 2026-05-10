using UnityEngine;

public class Projectile : MonoBehaviour
{
    // projectile movement and lifetime settings
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 4f;

    // target and damage assigned when the projectile is fired
    private Enemy target;
    private float damage;

    private void Start()
    {
        // destroys the projectile after its lifetime ends
        Destroy(gameObject, lifetime);
    }

    public void SetTarget(Enemy newTarget, float newDamage)
    {
        // stores the enemy target and damage amount
        target = newTarget;
        damage = newDamage;
    }

    private void Update()
    {
        // rremoves projectile if target is missing or dead
        if (target == null || !target.IsAlive)
        {
            Destroy(gameObject);
            return;
        }

        // moves projectile toward the target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        // check if projectile has reached the target
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < 0.3f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}