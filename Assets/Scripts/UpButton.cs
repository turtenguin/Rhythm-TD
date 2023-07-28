using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpButton : UpgradeButton
{
    public Text costText;
    public Text effectText;
    public Scaler circleShadow;

    protected override void Start()
    {
        base.Start();
        checkForHighlight = true;
    }

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

        UpdateCoins(shopManager.coins);
    }

    protected override void ActivateButton()
    {
        base.ActivateButton();

        if (shopManager.MakePurchase(cost))
        {
            tower.Upgrade();
            circleShadow.StartScale(tower.range);
        }

        InitButton();
    }

    protected override void OnPress()
    {
        base.OnPress();

        float newRange = tower.NextUpgradeRange();
        if(newRange != 0)
        {
            circleShadow.StartScale(newRange);
        }
    }

    protected override void OnUnpress()
    {
        base.OnUnpress();

        circleShadow.StartScale(tower.range);
    }
}
