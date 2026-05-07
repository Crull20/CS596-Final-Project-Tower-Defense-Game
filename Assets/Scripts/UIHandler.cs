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

    [Header("Input Handler")] 
    public InputHandler inputHandler;

    void Start()
    {
        inputHandler ??= GameObject.Find("InputHandler").GetComponent<InputHandler>();
        
        // Ideally shopPanel and pauseMenu should be active in the scene before runtime
        shopPanel ??= GameObject.Find("ShopPanel"); // Ideally shopPanel should be active in the scene before runtime
        if (shopPanel) shopPanel.SetActive(false); // Hide the shop
        pauseMenu ??= GameObject.Find("PausePanel");
        if (pauseMenu) pauseMenu.SetActive(false); // Hide the pause menu
    }

    public void OpenShop()
    {
        inputHandler.CancelInput(); // Force cancel player inputs
        if (!shopPanel.activeSelf) shopPanel.SetActive(true); // Ensure shop is inactive before opening it
    }

    public void PlaceTowerMode()
    {
        // TODO: implement tower button
    }

    public void PauseGame()
    {
        inputHandler.CancelInput(); // Force cancel player inputs
        Time.timeScale = 0f; // Stop game's flow of time
        if (!pauseMenu.activeSelf) pauseMenu.SetActive(true); // Ensure pause menu is inactive before opening it
    }
}
