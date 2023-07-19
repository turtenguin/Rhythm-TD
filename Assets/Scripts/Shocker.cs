using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shocker : Targeter
{
    public DigitalRuby.LightningBolt.LightningBoltScript lightning;
    
    
    protected override void Attack(BeatManager.BeatAction beatAction)
    {
        base.Attack(beatAction);

        if(target != null)
        {
            lightning.EndPosition = target.transform.position;
            lightning.Trigger();
        }
    }
}
