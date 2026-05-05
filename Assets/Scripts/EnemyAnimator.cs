using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    public Animator animator;
    public Enemy enemyScript;

    //how long to wait after triggering death before deactivating the enemy
    [SerializeField] private float deathAnimationDuration = 2f;

    private Vector3 lastPosition;

    private void Awake()
    {
        //auto-grab components if not assigned in the inspector
        if (animator == null)
            animator = GetComponent<Animator>();
        if (enemyScript == null)
            enemyScript = GetComponentInParent<Enemy>();
    }

    private void OnEnable()
{
    // make sure references are set
    if (animator == null)
        animator = GetComponent<Animator>();

    if (enemyScript == null)
        enemyScript = GetComponentInParent<Enemy>();

    // use ROOT enemy position, not the model
    if (enemyScript != null)
        lastPosition = enemyScript.transform.position;
    else
        lastPosition = transform.position;

    // subscribe to death event
    if (enemyScript != null)
        enemyScript.OnDeath += PlayDeath;
}
    private void OnDisable()
    {
        //unsubscribe every time the object is disabled so pooled enemies don't double-fire
        if (enemyScript != null)
            enemyScript.OnDeath -= PlayDeath;
    }

    private void Update()
{
    if (animator == null || enemyScript == null)
        return;

    // check movement using the ROOT enemy object (not the model)
    Vector3 currentPos = enemyScript.transform.position;

    bool isMoving = (currentPos - lastPosition).sqrMagnitude > 0.0001f;

    animator.SetBool("isRunning", isMoving);

    lastPosition = currentPos;
}

    public void PlayDeath()
    {
        //claim deactivation responsibility so Enemy doesn't self-deactivate immediately
        if (enemyScript != null)
            enemyScript.deathHandled = true;

        //fire the death trigger and wait for the clip to finish before returning to the pool
        animator.SetTrigger("Death");
        StartCoroutine(DeactivateAfterDeath());
    }

    private IEnumerator DeactivateAfterDeath()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        //deactivate the root enemy object so BecameUnavailable fires and the pool can reclaim it
        if (enemyScript != null)
            enemyScript.gameObject.SetActive(false);
    }
}
