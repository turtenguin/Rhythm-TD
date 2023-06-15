using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{

    private Image thisImage;
    public float fadeInSpeed = 1f;
    private float moveSpeed;
    public int travelBeats = 16;
    GameManager gameManager;
    void Start()
    {
        transform.localPosition = new Vector3(0, 600, 0);
        thisImage = GetComponent<Image>();
        thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, 0f);

        gameManager = GameManager.instance;
        moveSpeed = 600 / (gameManager.secondsPerBeat * travelBeats);
    }

    void Update()
    {
        if(thisImage.color.a < 1f)
        {
            thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, thisImage.color.a + fadeInSpeed * Time.deltaTime);
            if(thisImage.color.a > 1f)
            {
                thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, 1f);
            }
        }

        transform.localPosition += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
    }
}
