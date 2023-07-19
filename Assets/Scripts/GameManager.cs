using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public float transitionToShopTime = 1f;

    public Tower[] towerPrefabs;
    
    public Plane floorPlane;
    public Shadow shadow;
    public UpgradeMenu upgradeMenu;

    public List<Enemy> enemies;

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

    //Array storing what is present in each space for fast calculations of building there
    public enum Contents
    {
        ground,
        air,
        path,
        tower,
    }
    public Contents[,] buildMap;
    public int buildMapOffset;

    void Awake()
    {
        instance = this;

        floorPlane = new Plane(Vector3.up, -.2f);

        enemies = new List<Enemy>();

        loadMapData();
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

    private void loadMapData()
    {
        buildMapOffset = 4;
        buildMap = new Contents[9,9];

        buildMap[0, 0] = Contents.air;
        buildMap[0, 1] = Contents.air;
        buildMap[0, 2] = Contents.air;
        buildMap[0, 3] = Contents.air;
        buildMap[0, 4] = Contents.air;
        buildMap[0, 5] = Contents.air;
        buildMap[0, 6] = Contents.air;
        buildMap[0, 7] = Contents.air;
        buildMap[0, 8] = Contents.air;

        buildMap[1, 0] = Contents.air;
        buildMap[1, 1] = Contents.air;
        buildMap[1, 2] = Contents.air;
        buildMap[1, 3] = Contents.air;
        buildMap[1, 4] = Contents.air;
        buildMap[1, 5] = Contents.air;
        buildMap[1, 6] = Contents.air;
        buildMap[1, 7] = Contents.air;
        buildMap[1, 8] = Contents.air;

        buildMap[2, 0] =  Contents.ground;
        buildMap[2, 1] =  Contents.ground;
        buildMap[2, 2] =  Contents.ground;
        buildMap[2, 3] =  Contents.ground;
        buildMap[2, 4] =  Contents.ground;
        buildMap[2, 5] =  Contents.ground;
        buildMap[2, 6] =  Contents.ground;
        buildMap[2, 7] = Contents.air;
        buildMap[2, 8] =  Contents.ground;

        buildMap[3, 0] =  Contents.ground;
        buildMap[3, 1] = Contents.air;
        buildMap[3, 2] = Contents.air;
        buildMap[3, 3] = Contents.air;
        buildMap[3, 4] = Contents.air;
        buildMap[3, 5] = Contents.air;
        buildMap[3, 6] = Contents.air;
        buildMap[3, 7] = Contents.air;
        buildMap[3, 8] =  Contents.ground;

        buildMap[4, 0] =  Contents.ground;
        buildMap[4, 1] = Contents.air;
        buildMap[4, 2] =  Contents.ground;
        buildMap[4, 3] =  Contents.ground;
        buildMap[4, 4] =  Contents.ground;
        buildMap[4, 5] =  Contents.ground;
        buildMap[4, 6] =  Contents.ground;
        buildMap[4, 7] =  Contents.ground;
        buildMap[4, 8] =  Contents.ground;

        buildMap[5, 0] =  Contents.ground;
        buildMap[5, 1] = Contents.air;
        buildMap[5, 2] = Contents.air;
        buildMap[5, 3] = Contents.air;
        buildMap[5, 4] = Contents.air;
        buildMap[5, 5] = Contents.air;
        buildMap[5, 6] = Contents.air;
        buildMap[5, 7] = Contents.air;
        buildMap[5, 8] =  Contents.ground;

        buildMap[6, 0] =  Contents.ground;
        buildMap[6, 1] =  Contents.ground;
        buildMap[6, 2] =  Contents.ground;
        buildMap[6, 3] =  Contents.ground;
        buildMap[6, 4] =  Contents.ground;
        buildMap[6, 5] =  Contents.ground;
        buildMap[6, 6] =  Contents.ground;
        buildMap[6, 7] = Contents.air;
        buildMap[6, 8] =  Contents.ground;

        buildMap[7, 0] = Contents.air;
        buildMap[7, 1] = Contents.air;
        buildMap[7, 2] = Contents.air;
        buildMap[7, 3] = Contents.air;
        buildMap[7, 4] = Contents.air;
        buildMap[7, 5] = Contents.air;
        buildMap[7, 6] = Contents.air;
        buildMap[7, 7] = Contents.air;
        buildMap[7, 8] = Contents.air;

        buildMap[8, 0] = Contents.air;
        buildMap[8, 1] = Contents.air;
        buildMap[8, 2] = Contents.air;
        buildMap[8, 3] = Contents.air;
        buildMap[8, 4] = Contents.air;
        buildMap[8, 5] = Contents.air;
        buildMap[8, 6] = Contents.air;
        buildMap[8, 7] = Contents.air;
        buildMap[8, 8] = Contents.air;
    }
}
