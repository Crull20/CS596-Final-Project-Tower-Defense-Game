using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class ShopItem : MonoBehaviour
{
    [Header("Inscribed")] 
    public int price = 5;
    public bool isTower = true;

    [Header("UI Elements")]
    public TextMeshProUGUI priceLabel;
    
    void Start()
    {
        priceLabel ??= GetComponentInChildren<TextMeshProUGUI>();
        priceLabel.text = price.ToString();
    }
}
