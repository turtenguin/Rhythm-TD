using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zapper : Targeter
{
    public DigitalRuby.LightningBolt.LightningBoltScript lightning;
    public LayerMask enemyMask;
    public Transform lightningStart;

    protected virtual void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    protected override void Attack(BeatManager.BeatAction beatAction)
    {
        base.Attack(beatAction);

        if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            dir = new Vector3(dir.x, 0, dir.z);
            dir.Normalize();
            lightning.EndPosition = new Vector3(dir.x * range, lightningStart.position.y, dir.z * range);
            lightning.Trigger();

            RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, enemyMask);
            
            foreach(RaycastHit hit in hits)
            {
                Enemy hitEnemy = hit.collider.GetComponent<Enemy>();
                if(hitEnemy != null)
                {
                    hitEnemy.Damage(damage, transform.position);
                }
            }
        }
    }

    public override float Upgrade()
    {
        base.Upgrade();

        switch (level)
        {
            case 1:
                range = 4;
                return 4;
            case 2:
                range = 8;
                return 8;
            case 3:
                damage = 2;
                return 0;
        }
        return 0;
    }

    public override float NextUpgradeRange()
    {
        base.NextUpgradeRange();

        switch (level)
        {
            case 0:
                return 4;
            case 1:
                return 8;
            case 2:
                return 0;
        }
        return 0;
    }
}
