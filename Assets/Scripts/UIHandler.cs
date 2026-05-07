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
        
        shopPanel ??= GameObject.Find("ShopPanel"); // Ideally shopPanel should be active in the scene before runtime
        if (shopPanel) shopPanel.SetActive(false); // Hide the shop
        //pauseMenu ??= GameObject.Find("pauseMenu"); TODO: implement pause menu if time permits
    }

    public void OpenShop()
    {
        inputHandler.CancelInput(); // Force cancel player inputs
        if (!shopPanel.activeSelf) shopPanel.SetActive(true);
    }

    public void PlaceTowerMode()
    {
        // TODO: implement tower button
    }

    public void PauseGame()
    {
        inputHandler.CancelInput(); // Force cancel player inputs
        // TODO: implement pause menu if time permits
    }
}
