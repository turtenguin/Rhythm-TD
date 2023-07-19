using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : UpgradeButton
{
    private TowerDrag towerDrag;
    public Text costText;

    protected override void Start()
    {
        base.Start();
        cost = shopManager.moveCost;
        costText.text = cost.ToString();
        closeOnPress = true;
    }
    protected override void ActivateButton()
    {
        if (shopManager.MakePurchase(cost))
        {
            tower.towerDrag.InitDrag();
        }
    }
}
