using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordButton : UpgradeButton
{
    private RecordManager recordManager;
    [SerializeField] private int disableRecordBeat = 32;

    public Text beats;
    public Text totalBeats;

    protected override void Start()
    {
        base.Start();

        closeOnPress = true;

        recordManager = RecordManager.recordManagerInstance;
        button.interactable = false;

        RecordManager.BeatAction enableAction = new RecordManager.BeatAction(Enable,-1,0,-1,false,true);
        recordManager.AddPassiveAction(enableAction);

        RecordManager.BeatAction disableAction = new RecordManager.BeatAction(Disable, -1, disableRecordBeat, -1, false, true);
        recordManager.AddPassiveAction(disableAction);
    }

    private void Enable(RecordManager.BeatAction beatAction)
    {
        button.interactable = true;
    }

    private void Disable(RecordManager.BeatAction beatAction)
    {
        button.interactable = false;
    }

    protected override void ActivateButton()
    {
        recordManager.ArmRecord(upgradeMenu.tower.towerType);
    }

    public override void InitButton()
    {
        base.InitButton();
        
        beats.text = recordManager.hitsForTower(towerType, tower.key).ToString() + "/" + recordManager.hitsAvailable(towerType, tower.key).ToString();
        totalBeats.text = recordManager.hitsForTower(towerType).ToString() + "/" + recordManager.hitsAvailable(towerType).ToString();
    }
}
