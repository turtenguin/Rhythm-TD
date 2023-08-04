using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shocker : Targeter
{
    public DigitalRuby.LightningBolt.LightningBoltScript lightning;


    protected override void Attack(BeatManager.BeatAction beatAction)
    {
        base.Attack(beatAction);

        if (target != null)
        {
            lightning.EndPosition = target.transform.position;
            lightning.Trigger();
            target.Damage(damage, transform.position);
        }
    }

    public override float Upgrade()
    {
        base.Upgrade();

        switch (level)
        {
            case 1:
                damage = 2;
                return 0;
            case 2:
                range = 5;
                return 2;
            case 3:
                damage = 3;
                return 0;
        }

        return 0;
    }

    public override float NextUpgradeRange()
    {
        switch (level)
        {
            case 0:
                return 0;
            case 1:
                return 5;
            case 2:
                return 0;
        }
        return 0;
    }
}

