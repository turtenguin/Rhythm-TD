using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackUpButton : UpgradeButton
{
    public Text costText;
    public Text beatIncrease;
    public Text totalBeatIncrease;
    private RecordManager recordManager;
    private int track;
    private int trackVersion;
    public UpgradeButton recordButton;

    protected override void Start()
    {
        base.Start();

        recordManager = RecordManager.recordManagerInstance;
    }

    public override void InitButton()
    {
        base.InitButton();

        track = recordManager.TowerTypeToTrack(towerType);
        trackVersion = recordManager.TrackVersion(track);

        if((trackVersion >= recordManager.numTrackVersions - 1) || (shopManager.numTowersOut[towerType] < shopManager.numTowersPerType))
        {
            button.interactable = false;
            return;
        }
        else
        {
            button.interactable = true;
        }

        cost = shopManager.trackUpgradeCosts[track].upgradeCosts[trackVersion];

        costText.text = cost.ToString();
        totalBeatIncrease.text = "+" + (recordManager.hitsAvailableOnVersion(towerType, trackVersion + 1) - recordManager.hitsAvailableOnVersion(towerType, trackVersion)).ToString();
        beatIncrease.text = "+" + (recordManager.hitsAvailableOnVersion(towerType, trackVersion + 1, tower.key) - recordManager.hitsAvailableOnVersion(towerType, trackVersion, tower.key)).ToString();

        UpdateCoins(shopManager.coins);
    }


    protected override void ActivateButton()
    {
        if ((shopManager.numTowersOut[towerType] >= shopManager.numTowersPerType) && shopManager.MakePurchase(cost))
        {
            recordManager.ChangeTrackVersion(track, trackVersion + 1);
        }

        InitButton();
        recordButton.InitButton();
    }
}
