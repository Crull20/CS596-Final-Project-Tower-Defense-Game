using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIHandler : MonoBehaviour
{
    [Header("UI Elements")] 
    public GameObject shopPanel;
    public GameObject pauseMenu;
    public GameObject confirmTowerButton;
    public GameObject cancelTowerButton;

    [Header("Handlers")] 
    public InputHandler inputHandler;
    
    void Start()
    {
        inputHandler ??= GameObject.Find("InputHandler").GetComponent<InputHandler>();
        
        // These UI elements should be active in the scene before runtime so that they are deactivated on start
        // UI handler should also have these references set in the editor
        List<GameObject> uiElements = new List<GameObject>()
        {
            shopPanel,
            pauseMenu,
            confirmTowerButton,
            cancelTowerButton
        };
        foreach (GameObject element in uiElements)
        {
            element.SetActive(false);
        }
    }

    public void OpenShop()
    {
        inputHandler.CancelInput(); // Force cancel player inputs
        if (!shopPanel.activeSelf) shopPanel.SetActive(true); // Ensure shop is inactive before opening it
    }

    public void PauseGame()
    {
        inputHandler.CancelInput(); // Force cancel player inputs
        Time.timeScale = 0f; // Stop game's flow of time
        if (!pauseMenu.activeSelf) pauseMenu.SetActive(true); // Ensure pause menu is inactive before opening it
    }
}
