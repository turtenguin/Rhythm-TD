using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public int coins;
    public int numTowerTypes;
    public int numTowersPerType;
    public int numUpgrades { get; private set; }
    public int[] towerCosts;
    public int[] numTowersOut { get; private set; }

    public Text coinText;
    public ShopButton[] buttons;
    public UpgradeMenu upgradeMenu;

    static public ShopManager instance;

    [System.Serializable]
    public struct costList
    {
        [SerializeField] public int[] upgradeCosts;
    }

    public costList[] upgradeCosts;
    public costList[] trackUpgradeCosts;
    public int moveCost;

    public int[] bounties;

    void Awake()
    {
        instance = this;
        numTowersOut = new int[numTowerTypes];
        numUpgrades = upgradeCosts[0].upgradeCosts.Length;
    }

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].UpdateCost(towerCosts[i]);
        }
        UpdateShop();
    }

    public int IsTowerNotAvailable(int tower)
    {
        if(tower >= numTowerTypes || tower < 0)
        {
            return 3;
        }
        if(coins < towerCosts[tower])
        {
            return 1;
        }
        if(numTowersOut[tower] >= numTowersPerType)
        {
            return 2;
        }
        return 0;
    }

    public bool PurchaseTower(int tower)
    {
        if (IsTowerNotAvailable(tower) != 0)
        {
            return false;
        }

        coins -= towerCosts[tower];
        numTowersOut[tower]++;

        UpdateShop();

        return true;
    }

    public bool DestroyTower(int tower)
    {
        if(numTowersOut[tower] <= 0)
        {
            return false;
        }

        numTowersOut[tower]--;

        UpdateShop();

        return true;
    }

    public bool CancelBuild(int tower)
    {
        coins += towerCosts[tower];
        return DestroyTower(tower);
    }

    public bool MakePurchase(int cost)
    {
        if(cost <= coins)
        {
            coins -= cost;
            UpdateShop();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EarnCoins(int earnedCoins)
    {
        coins += earnedCoins;
        UpdateShop();
    }

    public void CollectBounty(int enemyType)
    {
        EarnCoins(bounties[enemyType]);
    }

    private void UpdateShop()
    {
        coinText.text = coins.ToString();
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].UpdateState(IsTowerNotAvailable(i));
        }

        upgradeMenu.UpdateCoins();
    }
}
