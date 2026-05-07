using UnityEngine;

public class BuildTowerButton : MonoBehaviour
{
    [SerializeField] private TowerPlacementManager placementManager;
    [SerializeField] private GameObject towerPrefab;

    public void BuildTower()
    {
        // Check for a valid placement manager and tower prefab reference
        // Allow the player to place if they have towers in their inventory
        if (placementManager != null && towerPrefab != null && placementManager.TowerCount > 0)
            placementManager.BeginPlacement(towerPrefab);
    }
}