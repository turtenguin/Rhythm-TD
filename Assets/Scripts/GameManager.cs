using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int health = 100;
    public Text healthText;

    public float transitionToShopTime = 1f;
    public float refreshTargetRate = .2f;
    public Tower[] towerPrefabs;
    
    public Plane floorPlane;
    public Shadow shadow;
    public UpgradeMenu upgradeMenu;

    public EnemySpawner enemySpawner;
    public List<Enemy> enemies { get; private set; }

    static public GameManager instance;

    /* Each element contains direction of given track segment
     * 0 = +x
     * 1 = -x
     * 2 = +z
     * 3 = -z
     * 4 = end
     */
    public int[] trackDirs;
    //each element contains the endpoint of given track segment
    public int[] trackEnds;
    public Vector3 trackStart { get; private set; }
    public float UFOHeight;

    //Array storing what is present in each space for fast calculations of building there
    public enum Contents
    {
        ground,
        air,
        tower,
    }
    public Contents[,] buildMap;
    public int buildMapOffset;

    void Awake()
    {
        instance = this;

        floorPlane = new Plane(Vector3.up, -.2f);

        enemySpawner = GetComponent<EnemySpawner>();

        healthText.text = health.ToString();
    }

    private void Start()
    {
        enemies = enemySpawner.enemies;
    }

    public bool Damage(int dmg)
    {
        health -= dmg;

        if(health > 0)
        {
            healthText.text = health.ToString();
            return false;
        }
        else
        {
            healthText.text = "0";
            Invoke("Die", 5);
            return true;
        }
    }

    private void Die()
    {
        SceneManager.LoadScene("Death Menu");
    }

    public bool canBuildHere(int x, int z)
    {
        if(x + buildMapOffset < 0 || z + buildMapOffset < 0 || x + buildMapOffset >= buildMap.GetLength(0) || z + buildMapOffset >= buildMap.GetLength(1))
        {
            return false;

        }
        return buildMap[x + buildMapOffset, z + buildMapOffset] == Contents.ground;
    }

    public void RegisterTowerPosition(Vector3 towerPos)
    {
        int x = Mathf.RoundToInt(towerPos.x);
        int z = Mathf.RoundToInt(towerPos.z);
        buildMap[x + buildMapOffset, z + buildMapOffset] = Contents.tower;
    }

    public void RegisterTowerDestruction(Vector3 towerPos)
    {
        int x = Mathf.RoundToInt(towerPos.x);
        int z = Mathf.RoundToInt(towerPos.z);
        buildMap[x + buildMapOffset, z + buildMapOffset] = Contents.ground;
    }

    public void LoadMapData(Contents[,] map, int xStart, int zStart)
    {
        buildMap = map;
        buildMapOffset = map.GetLength(0) / 2;

        trackStart = new Vector3((float)(xStart - buildMapOffset), UFOHeight, (float)(zStart - buildMapOffset));
    }
}
