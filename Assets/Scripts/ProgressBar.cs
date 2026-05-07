using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    public WaveManager waveManager;
    
    [Header("Progress Bar Elements")]
    public Image fillBar;
    public TextMeshProUGUI waveText;
    public float fillRate = 1.5f;

    private float percentage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the references if they are not assigned in the Editor
        waveManager ??= GameObject.Find("WaveManager").GetComponent<WaveManager>();
        fillBar ??= GameObject.Find("FillProgress").GetComponent<Image>();
        waveText ??= GameObject.Find("WaveNum").GetComponent<TextMeshProUGUI>();

        // Subscribe to WaveManager events
        waveManager.OnWaveStart += OnWaveStart;
        waveManager.OnPauseTick += OnPauseTick;
        waveManager.OnAllWavesComplete += OnAllWavesComplete;
    }
    
    void FixedUpdate()
    {
        // Fill the bar smoothly
        fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, percentage, Time.deltaTime * fillRate);
    }

    void OnWaveStart(int waveNumber)
    {
        // Calculate the fill percentage based on the current wave
        // Subtract the current wave number by 1 to have the bar fill after each wave completes
        percentage = (float) (waveNumber - 1) / waveManager.GetWaveCount();
        
        // Display current wave number, show special text on final wave
        waveText.text = waveNumber == waveManager.GetWaveCount() ? "FINAL\nWAVE" : waveNumber.ToString("WAVE\n0");
    }

    void OnPauseTick(float duration)
    {
        // Show the wait time before the next wave
        waveText.text = duration.ToString("Next\n0.0");
    }

    void OnAllWavesComplete()
    {
        percentage = 1; // Set the fill percentage to 100% and have the bar fully filled
        waveText.text = "You\nwin!"; // Placeholder win text TODO: make win graphic?
    }
}
