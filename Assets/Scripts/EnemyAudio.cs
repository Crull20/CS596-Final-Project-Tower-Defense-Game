using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class EnemyAudio : MonoBehaviour
{
    public Enemy enemyScript;

    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip groundImpactClip;
    [SerializeField] private AudioClip footstepLoopClip;
    [SerializeField] private AudioClip stepClip;

    [SerializeField] private Animator animator;
    [SerializeField] private string runningParam = "isRunning";

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (enemyScript == null)
            enemyScript = GetComponentInParent<Enemy>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (enemyScript == null)
            enemyScript = GetComponentInParent<Enemy>();
        if (animator == null)
            animator = GetComponent<Animator>();

        //subscribe to death and hit events
        if (enemyScript != null)
        {
            enemyScript.OnDeath += PlayDeath;
            enemyScript.OnTakeDamage += PlayHit;
        }
    }

    private void OnDisable()
    {
        //unsubscribe every time the object is disabled so pooled enemies don't double-fire
        if (enemyScript != null)
        {
            enemyScript.OnDeath -= PlayDeath;
            enemyScript.OnTakeDamage -= PlayHit;
        }
    }

    private void Update()
    {
        if (footstepLoopClip == null || animator == null)
            return;

        bool isRunning = animator.GetBool(runningParam);
        bool footstepPlaying = audioSource.clip == footstepLoopClip && audioSource.isPlaying;

        if (isRunning && !footstepPlaying)
        {
            audioSource.clip = footstepLoopClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (!isRunning && footstepPlaying)
        {
            audioSource.Stop();
        }
    }

    private void PlayHit()
    {
        //skip hit sound on the killing blow so the death sound plays clean
        if (enemyScript != null && enemyScript.CurrentHealth <= 0f)
            return;

        if (hitClip != null)
            audioSource.PlayOneShot(hitClip);
    }

    public void PlayDeath(Enemy enemy)
    {
        Debug.Log("PlayDeath called on " + gameObject.name);
        //clear the loop without calling Stop() so in-flight PlayOneShot calls aren't killed
        if (audioSource.clip == footstepLoopClip)
        {
            audioSource.loop = false;
            audioSource.clip = null;
        }

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);
    }

    //called via animation event for individual boss footsteps
    public void PlayStep()
    {
        if (stepClip != null)
            audioSource.PlayOneShot(stepClip);
    }

    //called via animation event when the boss hits the ground
    public void PlayGroundImpact()
    {
        if (groundImpactClip != null)
            audioSource.PlayOneShot(groundImpactClip);
    }
}
