using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    [Header("Coin Amounts")] 
    public int enemyCoinReward = 5;
    public int bossCoinReward = 10;
    
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

    public int GetCoins()
    {
        // allow other scripts to check how many coins player has
        return coins;
    }

    public void RemoveCoins(int amount)
    {
        // used when spending coins, buying/placing towers
        coins -= amount;
    }
}
