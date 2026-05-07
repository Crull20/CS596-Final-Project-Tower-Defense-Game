using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaveAudio : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private AudioClip waveStartClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (waveManager != null)
            waveManager.OnWaveStart += OnWaveStart;
    }

    private void OnDisable()
    {
        if (waveManager != null)
            waveManager.OnWaveStart -= OnWaveStart;
    }

    private void OnWaveStart(int waveNumber)
    {
        if (waveStartClip != null)
            audioSource.PlayOneShot(waveStartClip);
    }
}
