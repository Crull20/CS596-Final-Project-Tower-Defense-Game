using UnityEngine;

public class GameSettings : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        // Target the highest framerate supported by the device's refresh rate
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
    }
}