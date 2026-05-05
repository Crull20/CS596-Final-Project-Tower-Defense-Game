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
        //reset last position so the run bool doesn't flicker on the first frame
        lastPosition = transform.position;

        //subscribe to death so we can trigger the animation when health hits zero
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
        //compare world position this frame vs last frame to detect movement
        bool isMoving = (transform.position - lastPosition).sqrMagnitude > 0.0001f;
        animator.SetBool("isRunning", isMoving);
        lastPosition = transform.position;
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
