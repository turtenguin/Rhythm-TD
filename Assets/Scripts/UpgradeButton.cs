using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    protected Animator animator;
    private bool justPressed;
    public UpgradeMenu upgradeMenu;
    protected Button button;
    protected ShopManager shopManager;
    protected int cost = 0;
    protected Tower tower;
    protected int towerType;
    protected bool justClicked = true;
    protected bool closeOnPress = false;
    protected bool checkForHighlight = false;
    private bool highlighted = false;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
        shopManager = ShopManager.instance;
    }

    protected virtual void Update()
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

        if (checkForHighlight)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!highlighted && state.IsName("Highlighted"))
            {
                highlighted = true;
                OnPress();
            }
            else if (highlighted && !state.IsName("Highlighted") && !state.IsName("Pressed") && !state.IsName("Disabled"))
            {
                highlighted = false;
                OnUnpress();
            }
        }
        
    }

    protected virtual void OnPress()
    {

    }

    protected virtual void OnUnpress()
    {

    }

    private void OnPressed()
    {
        justPressed = true;
        highlighted = false;
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
            Invoke("SetCantBuy", .25f);
        }
        else
        {
            animator.SetBool("CantBuy", false);
        }
    }

    private void SetCantBuy()
    {
        animator.SetBool("CantBuy", true);
    }
}
