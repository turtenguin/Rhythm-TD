using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : Tower
{
    protected Enemy target = null;
    private List<Enemy> enemies;

    protected override void Start()
    {
        base.Start();
        enemies = gameManager.enemies;
        routineUpdateTarget();
    }

    protected override void Attack(BeatManager.BeatAction beatAction)
    {
        base.Attack(beatAction);

        if(target == null)
        {
            UpdateTarget();
        }
    }

    protected void UpdateTarget()
    {
        switch (targeting)
        {
            case 0:
                target = findFirstEnemy();
                break;
            case 1:
                target = findLastEnemy();
                break;
            case 2:
                target = findStrongestEnemy();
                break;
            case 3:
                target = findClosestEnemy();
                break;
            case 4:
                target = findFastestEnemy();
                break;
        }
    }

    private void routineUpdateTarget()
    {
        UpdateTarget();
        Invoke("routineUpdateTarget", gameManager.refreshTargetRate);
    }

    private Enemy findStrongestEnemy()
    {
        Enemy strongestEnemy = null;
        int maxStrength = 0;

        foreach(Enemy enemy in enemies)
        {
            if(enemy.strength > maxStrength && Vector3.Distance(enemy.transform.position, transform.position) <= range)
            {
                strongestEnemy = enemy;
                maxStrength = enemy.strength;
            }
        }

        return strongestEnemy;
    }

    private Enemy findLastEnemy()
    {
        for(int i = enemies.Count - 1; i >= 0; i--)
        {
            if(Vector3.Distance(enemies[i].transform.position, transform.position) <= range)
            {
                return enemies[i];
            }
        }
        return null;
    }

    private Enemy findFirstEnemy()
    {
        foreach (Enemy enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, transform.position);
            if (dist <= range){
                return enemy;
            }
        }
        return null;
    }

    private Enemy findClosestEnemy()
    {
        float minDist = 999999f;
        Enemy closestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            float thisDist = Vector3.Distance(transform.position, enemy.transform.position);
            if (thisDist < minDist)
            {
                minDist = thisDist;
                closestEnemy = enemy;
            }
        }

        if(minDist <= range)
        {
            return closestEnemy;
        }
        else
        {
            return null;
        }
    }

    private Enemy findFastestEnemy()
    {
        Enemy fastestEnemy = null;
        float maxSpeed = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.speed > maxSpeed && Vector3.Distance(enemy.transform.position, transform.position) <= range)
            {
                fastestEnemy = enemy;
                maxSpeed = enemy.speed;
            }
        }

        return fastestEnemy;
    }
}
