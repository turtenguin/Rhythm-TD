using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetButton : UpgradeButton
{
    private static readonly string[] modes = { "FIRST", "LAST", "STRONG", "CLOSE" };
    public Text modeText;

    public override void InitButton()
    {
        base.InitButton();

        modeText.text = modes[tower.targeting];
    }

    protected override void ActivateButton()
    {
        base.ActivateButton();

        tower.targeting = (tower.targeting + 1) % 4;
        InitButton();
    }
}
