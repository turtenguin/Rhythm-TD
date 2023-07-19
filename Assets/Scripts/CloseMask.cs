using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CloseMask : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UpgradeMenu upgradeMenu;

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        upgradeMenu.Close();
    }
}
