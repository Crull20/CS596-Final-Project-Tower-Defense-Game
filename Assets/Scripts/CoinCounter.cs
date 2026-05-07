using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    [Header("Coin Amounts")] 
    public int enemyCoinReward = 10;
    public int bossCoinReward = 25;
    
    [Header("References")]
    public TextMeshProUGUI coinText;

    private int coins; // Keep track of player coins
    
    void Start()
    {
        coinText ??= gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        coinText.text = coins.ToString();
    }

    public void OnDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnDeath; // Unsubscribe to avoid gaining coins unexpectedly
        coins += enemy.gameObject.CompareTag("Boss") ? bossCoinReward : enemyCoinReward; // Bosses should give more
    }
}
