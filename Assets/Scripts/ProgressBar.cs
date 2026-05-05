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
    }
    
    void FixedUpdate()
    {
        // Fill the bar smoothly
        fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, percentage, Time.deltaTime);
    }

    void OnWaveStart(int waveNumber)
    {
        // Calculate the fill percentage based on the current wave
        percentage = (float) waveNumber / waveManager.GetWaveCount();
        
        // Display current wave number, show special text on final wave
        waveText.text = waveNumber == waveManager.GetWaveCount() ? "FINAL WAVE" : waveNumber.ToString("WAVE: 0");
    }

    void OnPauseTick(float duration)
    {
        // Show the wait time before the next wave
        waveText.text = duration.ToString("Next wave in 0.0");
    }
}
