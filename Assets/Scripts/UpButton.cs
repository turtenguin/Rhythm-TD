using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpButton : UpgradeButton
{
    public Text costText;
    public Text effectText;

    public override void InitButton()
    {
        base.InitButton();

        if(tower.level >= shopManager.numUpgrades)
        {
            button.interactable = false;
            return;
        }

        button.interactable = true;
        cost = shopManager.upgradeCosts[towerType].upgradeCosts[tower.level];
        costText.text = cost.ToString();
        effectText.text = tower.upgradeText[tower.level];
    }

    protected override void ActivateButton()
    {
        base.ActivateButton();

        if (shopManager.MakePurchase(cost))
        {
            tower.Upgrade();
        }

        InitButton();
    }
}
