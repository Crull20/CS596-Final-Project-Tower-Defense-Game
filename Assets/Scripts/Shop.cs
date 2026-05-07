using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("UI Elements")] 
    public GameObject TowerView;
    public GameObject UpgradeView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExitShop()
    {
        gameObject.SetActive(false);
    }

}
