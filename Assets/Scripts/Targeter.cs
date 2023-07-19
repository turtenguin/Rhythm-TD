using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : Tower
{
    protected Enemy target = null;

    protected virtual void Update()
    {
        target = findClosestEnemy();
    }

    Enemy findClosestEnemy()
    {
        float minDist = 999999f;
        Enemy closestEnemy = null;

        foreach (Enemy enemy in gameManager.enemies)
        {
            float thisDist = Vector3.Distance(transform.position, enemy.transform.position);
            if (thisDist < minDist)
            {
                minDist = thisDist;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
