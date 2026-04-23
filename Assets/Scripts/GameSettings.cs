using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int targetFrameRate = 60;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = targetFrameRate; // Target 60 fps
    }
}