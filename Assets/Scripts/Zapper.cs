using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zapper : Tower
{
    Enemy target = null;
    public DigitalRuby.LightningBolt.LightningBoltScript lightning;
    //public Transform lightingStartTrans;
    void Update()
    {
        target = findClosestEnemy();
        if(target != null)
        {
            transform.LookAt(target.transform.position);
        }
    }

    Enemy findClosestEnemy()
    {
        float minDist = 999999f;
        Enemy closestEnemy = null;

        foreach (Enemy enemy in gameManager.enemies)
        {
            float thisDist = Vector3.Distance(transform.position, enemy.transform.position);
            if(thisDist < minDist)
            {
                minDist = thisDist;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
    protected override void attack(GameManager.BeatAction beatAction)
    {
        if(target != null)
        {
            lightning.EndPosition = target.transform.position;
            lightning.Trigger();
        }
    }
}
