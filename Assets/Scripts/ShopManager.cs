using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int coins { get; private set; } = 0;
    public int[] towerCosts { get; private set; }
    static public ShopManager instance;
    private RecordManager recordManager;
    private GameManager gameManager;

    void Start()
    {
        instance = this;
        gameManager = GameManager.instance;
        recordManager = RecordManager.recordManagerInstance;
    }
}
