using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Shop : MonoBehaviour
{
    [Header("UI Elements")] 
    public GameObject defaultButton;
    public TextMeshProUGUI quantityText;
    
    [Header("Handlers")]
    public TowerPlacementManager placementManager;
    public CoinCounter CoinCounter;

    private ShopItem selectedItem;

    void Start()
    {
        // Have the shop select a default button
        SetSelected(defaultButton);
    }

    void Update()
    {
        ShowQuantity();
    }

    void ShowQuantity()
    {
        if (selectedItem.isTower)
        {
            // Show the item count
            quantityText.text = placementManager.TowerCount.ToString("In inventory: 0");
        }
    }

    public void SetSelected(GameObject button)
    {
        // Select the chosen item
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
        selectedItem = button.GetComponent<ShopItem>();
    }

    public void BuyItem()
    {
        // Buying a tower
        // Check if the player has enough coins
        if (selectedItem.isTower && CoinCounter.GetCoins() >= selectedItem.price)
        {
            // Add the tower to the player's count and charge their coins
            placementManager.AddTower();
            CoinCounter.RemoveCoins(selectedItem.price);
        }

        // Keep the item selected
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(selectedItem.gameObject);
    }

    public void ExitShop()
    {
        gameObject.SetActive(false);
    }

}
