using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    public Enemy enemyScript;

    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip groundImpactClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (enemyScript == null)
            enemyScript = GetComponentInParent<Enemy>();
    }

    private void OnEnable()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (enemyScript == null)
            enemyScript = GetComponentInParent<Enemy>();

        //subscribe to death event
        if (enemyScript != null)
            enemyScript.OnDeath += PlayDeath;
    }

    private void OnDisable()
    {
        //unsubscribe every time the object is disabled so pooled enemies don't double-fire
        if (enemyScript != null)
            enemyScript.OnDeath -= PlayDeath;
    }

    public void PlayDeath()
    {
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);
    }

    //called via animation event when the boss hits the ground
    public void PlayGroundImpact()
    {
        if (groundImpactClip != null)
            audioSource.PlayOneShot(groundImpactClip);
    }
}
