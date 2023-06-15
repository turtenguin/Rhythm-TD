using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public Color unblockedColor;
    public Color blockedColor;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        gameManager = GameManager.instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }
    public void updatePos(int x, int z)
    {
        transform.position = new Vector3((float)x, transform.position.y, (float)z);

        if (gameManager.canBuildHere(x, z))
        {
            spriteRenderer.color = unblockedColor;
        } else
        {
            spriteRenderer.color = blockedColor;
        }
    }
}
