using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    private Animator animator;
    private bool justPressed;
    public UpgradeMenu upgradeMenu;
    protected Button button;
    protected ShopManager shopManager;
    protected int cost = 0;
    protected Tower tower;
    protected int towerType;
    protected bool justClicked = true;
    protected bool closeOnPress = false;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
        shopManager = ShopManager.instance;
    }

    private void Update()
    {
        if (justPressed)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
            {
                if (closeOnPress)
                {
                    upgradeMenu.Close();
                    justPressed = false;
                }
            }
        }
    }

    private void OnPressed()
    {
        justPressed = true;
    }

    public virtual void InitButton() {
        tower = upgradeMenu.tower;
        towerType = tower.towerType;
        justClicked = false;
    }

    public void OnClick() {
        if (!justClicked)
        {
            justClicked = true;
            ActivateButton();
        }
    }

    protected virtual void ActivateButton()
    {

    }

    public bool IsPressed()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Pressed");
    }

    public void UpdateCoins(int coins)
    {
        if(coins < cost)
        {
            animator.SetBool("CantBuy", true);
        }
        else
        {
            animator.SetBool("CantBuy", false);
        }
    }
}
