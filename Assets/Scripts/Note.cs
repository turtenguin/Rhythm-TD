using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{

    private Image thisImage;
    public float fadeInSpeed = 1f;
    public float fadeOutSpeed = 50f;
    private float moveSpeed;
    GameManager gameManager;
    RecordManager recordManager;
    Tab tab;
    bool fadedIn = false;
    bool fadeOut = false;
    public RecordManager.BeatAction beatAction;
    public Sprite whiteVersion;
    void Start()
    {
        gameManager = GameManager.instance;
        recordManager = RecordManager.recordManagerInstance;
        tab = recordManager.tab;

        transform.localPosition = new Vector3(0, 600 + tab.noteOffset, 0);
        thisImage = GetComponent<Image>();
        thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, 0f);

        
        moveSpeed = 600 / (recordManager.secondsPerBeat * tab.travelBeats);

        recordManager.RegisterNote(this);
    }

    void Update()
    {
        if (!fadedIn)
        {
            thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, thisImage.color.a + fadeInSpeed * Time.deltaTime);
            if (thisImage.color.a > 1f)
            {
                thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, 1f);
                fadedIn = true;
            }
        }

        if (fadeOut)
        {
            thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, thisImage.color.a - fadeOutSpeed * Time.deltaTime);
            if (thisImage.color.a <= 0)
            {
                recordManager.RemoveNote(this);
                GameObject.Destroy(gameObject);
            }
        }

        if(transform.localPosition.y < -10)
        {
            fadeOut = true;
        }

        transform.localPosition += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
    }

    public void OnPlayed()
    {
        thisImage.sprite = whiteVersion;
        fadeOut = true;
    }
}
