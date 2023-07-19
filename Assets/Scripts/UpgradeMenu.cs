using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    private Animator animator;
    private CanvasGroup canvasGroup;

    private ShopManager shopManager;
    private RecordManager recordManager;
    public UpgradeButton recordButton, upgradeButton, trackUpButton, moveButton, targetButton;

    public Tower tower;

    private Canvas parentCanvas;

    private bool waitingToInit = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        shopManager = ShopManager.instance;
        recordManager = RecordManager.recordManagerInstance;

        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (waitingToInit)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                waitingToInit = false;
                finishInit();
            }
        }
    }

    //Animation Controllers
    private void Open()
    {
        animator.SetTrigger("Open");
    }
    private void OnOpened()
    {
        canvasGroup.interactable = true;
    }

    public void Close()
    {
        animator.SetTrigger("Close");
        canvasGroup.interactable = false;
    }

    public void EndInteraction()
    {
        canvasGroup.interactable = false;
    }

    //Init the menu
    public void initMenu(Tower newTower)
    {
        tower = newTower;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            Close();
            waitingToInit = true;
        }
        else
        {
            finishInit();
        }
    }

    public void finishInit()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(tower.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPoint, Camera.main, out Vector2 localPoint);
        transform.position = parentCanvas.transform.TransformPoint(localPoint);

        recordButton.InitButton();
        trackUpButton.InitButton();
        upgradeButton.InitButton();
        moveButton.InitButton();
        targetButton.InitButton();

        UpdateCoins();

        Open();
    }

    //Interface
    public bool IsButtonPressed()
    {
        return recordButton.IsPressed() || upgradeButton.IsPressed() || trackUpButton.IsPressed() || moveButton.IsPressed();
    }

    public void UpdateCoins()
    {
        if(shopManager == null)
        {
            return;
        }

        int coins = shopManager.coins;

        recordButton.UpdateCoins(coins);
        trackUpButton.UpdateCoins(coins);
        upgradeButton.UpdateCoins(coins);
        moveButton.UpdateCoins(coins);
        targetButton.UpdateCoins(coins);
    }
}
