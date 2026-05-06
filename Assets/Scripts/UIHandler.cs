using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [Header("UI Elements")] 
    public GameObject ShopPanel;
    public GameObject PauseMenu;

    void Start()
    {
        ShopPanel ??= GameObject.Find("ShopPanel"); // Ideally, ShopPanel should be active
        if (ShopPanel) ShopPanel.SetActive(false); // Hide the shop
        //PauseMenu ??= GameObject.Find("PauseMenu"); TODO: implement pause menu if time permits
    }

    public void OpenShop()
    {
        if (!ShopPanel.activeSelf) ShopPanel.SetActive(true);
    }

    public void PlaceTowerMode()
    {
        // TODO: implement tower button
    }

    public void PauseGame()
    {
        // TODO: implement pause menu if time permits
    }
}
