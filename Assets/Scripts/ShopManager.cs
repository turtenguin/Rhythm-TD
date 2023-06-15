using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int coins = 0;
    static public ShopManager instance;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        gameManager = GameManager.instance;
    }

    
}
