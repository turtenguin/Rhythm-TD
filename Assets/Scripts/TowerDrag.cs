using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDrag : MonoBehaviour
{
    private Vector3 mouseOffset;
    protected GameManager gameManager;
    private ShopManager shopManager;
    private Shadow shadow;
    private Vector3 originalPosition;

    bool beingDragged = false;
    bool releaseBuffer = false;

    protected virtual void Start()
    {
        gameManager = GameManager.instance;
        shopManager = ShopManager.instance;
        shadow = gameManager.shadow;
    }

    private void Update()
    {
        if (beingDragged)
        {
            //Vector3 draggedPoint = screenToPlanePoint(Input.mousePosition - mouseOffset, gameManager.floorPlane);
            Vector3 draggedPoint = screenToPlanePoint(Input.mousePosition, gameManager.floorPlane);
            transform.position = draggedPoint;
            shadow.updatePos(Mathf.RoundToInt(draggedPoint.x), Mathf.RoundToInt(draggedPoint.z));

            if (Input.GetMouseButtonUp(0) && releaseBuffer)
            {
                shadow.gameObject.SetActive(false);

                draggedPoint = screenToPlanePoint(Input.mousePosition - mouseOffset, gameManager.floorPlane);
                int newX = Mathf.RoundToInt(draggedPoint.x);
                int newY = Mathf.RoundToInt(draggedPoint.z);

                if (gameManager.canBuildHere(newX, newY))
                {
                    transform.position = new Vector3((float)newX, transform.position.y, (float)newY);
                    gameManager.RegisterTowerPosition(transform.position);
                    OnPlace();
                }
                else
                {
                    OnReturn();
                }
                beingDragged = false;
                releaseBuffer = false;
            }
            else
            {
                releaseBuffer = true;
            }
        }
    }

    public void InitDrag()
    {
        mouseOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        shadow.updatePos(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        shadow.gameObject.SetActive(true);
        originalPosition = transform.position;
        beingDragged = true;
    }

    protected virtual void OnPlace()
    {
        gameManager.RegisterTowerDestruction(originalPosition);
    }
    protected virtual void OnReturn()
    {
        transform.position = originalPosition;
        shopManager.EarnCoins(shopManager.moveCost);
    }

    //Returns point on given plane which main camera screen point intersects
    protected Vector3 screenToPlanePoint(Vector3 screenPoint, Plane plane)
    {
        Ray screenPointRay = Camera.main.ScreenPointToRay(Input.mousePosition - mouseOffset);
        plane.Raycast(screenPointRay, out float rayDist);
        return screenPointRay.GetPoint(rayDist);
    }
}
