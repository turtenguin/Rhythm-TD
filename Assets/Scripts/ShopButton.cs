using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    private Button button;
    private Text text;
    private ShopManager shopManager;
    public int tower;

    public DummyTowerDrag dummyPrefab;

    private Color textColor;
    private Color textDisabledColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        textColor = text.color;
        textDisabledColor = textColor * button.colors.disabledColor;
    }
    private void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = .1f;
        button.onClick.AddListener(OnClick);
        shopManager = ShopManager.instance;
    }

    private void OnClick()
    {
        if (shopManager.PurchaseTower(tower))
        {
            SpawnTower();
        }
    }

    private void SpawnTower()
    {
        DummyTowerDrag newTower = Object.Instantiate(dummyPrefab, transform.position, Quaternion.identity);
        newTower.tower = tower;
    }

    public void UpdateState(int state)
    {
        if(state == 0)
        {
            button.interactable = true;
            text.color = textColor;
        } else
        {
            button.interactable = false;
            text.color = textDisabledColor;
        }
    }

    public void UpdateCost(int cost)
    {
        text.text = cost.ToString();
    }
}
