using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 shopPos;
    [SerializeField] private Vector3 recordPos;

    [SerializeField] private Vector3 slideShopPos;
    [SerializeField] private Vector3 slideRecordPos;

    public RectTransform slide;

    private float time = 1f;

    private bool slideLeft = false;
    private bool slideRight = false;
    private float startTime;

    private RecordManager recordManager;
    private GameManager gameManager;
    void Start()
    {
        recordManager = RecordManager.recordManagerInstance;
        gameManager = GameManager.instance;
        time = gameManager.transitionToShopTime;
        recordManager.onArmed.Add(SlideLeft);
        recordManager.onEndRecord.Add(SlideRight);
    }

    void Update()
    {
        if (slideLeft)
        {
            if(Time.time < startTime + time)
            {
                float position = (Time.time - startTime) / time;
                transform.position = Vector3.Lerp(shopPos, recordPos, position);
                slide.anchoredPosition = Vector3.Lerp(slideShopPos, slideRecordPos, position);

            } else
            {
                transform.position = recordPos;
                slide.anchoredPosition = slideRecordPos;
                slideLeft = false;
            }
        } else if (slideRight)
        {
            if (Time.time < startTime + time)
            {
                float position = (Time.time - startTime) / time;
                transform.position = Vector3.Lerp(recordPos, shopPos, position);
                slide.anchoredPosition = Vector3.Lerp(slideRecordPos, slideShopPos, position);

            }
            else
            {
                transform.position = shopPos;
                slide.anchoredPosition = slideShopPos;
                slideRight = false;
            }
        }
    }

    private void SlideLeft()
    {
        slideLeft = true;
        startTime = Time.time;
    }

    private void SlideRight()
    {
        slideRight = true;
        startTime = Time.time;
    }
}
