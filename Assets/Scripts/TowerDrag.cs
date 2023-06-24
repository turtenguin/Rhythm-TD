using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDrag : MonoBehaviour
{
    Vector3 mouseOffset;
    GameManager gameManager;
    Shadow shadow;
    Vector3 originalPosition;

    private void Start()
    {
        gameManager = GameManager.instance;
        shadow = gameManager.shadow;
    }

    private void OnMouseDown()
    {
        mouseOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        shadow.updatePos(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        shadow.gameObject.SetActive(true);
        originalPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        Vector3 draggedPoint = screenToPlanePoint(Input.mousePosition - mouseOffset, gameManager.floorPlane);
        transform.position = draggedPoint;
        shadow.updatePos(Mathf.RoundToInt(draggedPoint.x), Mathf.RoundToInt(draggedPoint.z));
    }

    private void OnMouseUp()
    {
        shadow.gameObject.SetActive(false);

        Vector3 draggedPoint = screenToPlanePoint(Input.mousePosition - mouseOffset, gameManager.floorPlane);
        int newX = Mathf.RoundToInt(draggedPoint.x);
        int newY = Mathf.RoundToInt(draggedPoint.z);

        if (gameManager.canBuildHere(newX, newY))
        {
            gameManager.RegisterTowerDestruction(originalPosition);
            transform.position = new Vector3((float)newX, transform.position.y, (float)newY);
            gameManager.RegisterTowerPosition(transform.position);
        } else
        {
            transform.position = originalPosition;
        }
    }

    //Returns point on given plane which main camera screen point intersects
    private Vector3 screenToPlanePoint(Vector3 screenPoint, Plane plane)
    {
        Ray screenPointRay = Camera.main.ScreenPointToRay(Input.mousePosition - mouseOffset);
        plane.Raycast(screenPointRay, out float rayDist);
        return screenPointRay.GetPoint(rayDist);
    }
}
