using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTowerDrag : TowerDrag
{
    public int tower;
    
    protected override void OnReturn()
    {
        ShopManager.instance.CancelBuild(tower);
        GameObject.Destroy(gameObject);
    }

    protected override void OnPlace()
    {
        Tower newTower = Object.Instantiate(GameManager.instance.towerPrefabs[tower], transform.position, Quaternion.identity);
        newTower.towerType = tower;
        GameObject.Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        transform.position = screenToPlanePoint(Input.mousePosition, gameManager.floorPlane);
        transform.localScale = Vector3.one * .7f;
        InitDrag();
    }
}
