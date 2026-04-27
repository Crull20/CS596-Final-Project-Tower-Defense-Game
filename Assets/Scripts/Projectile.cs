using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 4f;

    private Enemy target;
    private float damage;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetTarget(Enemy newTarget, float newDamage)
    {
        target = newTarget;
        damage = newDamage;
    }

    private void Update()
    {
        if (target == null || !target.IsAlive)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < 0.3f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}