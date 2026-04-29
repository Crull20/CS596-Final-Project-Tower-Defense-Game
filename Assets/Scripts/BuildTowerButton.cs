using UnityEngine;

public class BuildTowerButton : MonoBehaviour
{
    [SerializeField] private TowerPlacementManager placementManager;
    [SerializeField] private GameObject towerPrefab;

    public void BuildTower()
    {
        if (placementManager != null && towerPrefab != null)
            placementManager.BeginPlacement(towerPrefab);
    }
}