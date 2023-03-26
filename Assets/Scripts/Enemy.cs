using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    GameManager gameManager;
    private int dir;
    private int trackOn = 0;
    public float speed = 1;
    private Vector3 moveVec;
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.enemies.Add(this);
        dir = gameManager.trackDirs[0];
        moveVec = new Vector3(speed * Time.fixedDeltaTime, 0, 0);
    }

    void FixedUpdate()
    {
        transform.Translate(moveVec);
        updateMove();
        
    }

    void updateMove()
    {
        switch(dir)
        {
            case 0:
                if (transform.position.x > gameManager.trackEnds[trackOn]) turn();
                break;
            case 1:
                if (transform.position.x < gameManager.trackEnds[trackOn]) turn();
                break;
            case 2:
                if (transform.position.z > gameManager.trackEnds[trackOn]) turn();
                break;
            case 3:
                if (transform.position.z < gameManager.trackEnds[trackOn]) turn();
                break;
            default:
                Debug.Log("Invalid Direction");
                break;
        }
    }

    void turn()
    {
        trackOn++;
        dir = gameManager.trackDirs[trackOn];
        switch (dir)
        {
            case 0:
                moveVec = new Vector3(speed * Time.fixedDeltaTime, 0, 0);
                transform.position.Set(transform.position.x, transform.position.y, gameManager.trackEnds[trackOn - 1]);
                break;
            case 1:
                moveVec = new Vector3(-speed * Time.fixedDeltaTime, 0, 0);
                transform.position.Set(transform.position.x, transform.position.y, gameManager.trackEnds[trackOn - 1]);
                break;
            case 2:
                moveVec = new Vector3(0, 0, speed * Time.fixedDeltaTime);
                transform.position.Set(gameManager.trackEnds[trackOn - 1], transform.position.y, transform.position.z);
                break;
            case 3:
                moveVec = new Vector3(0, 0, -speed * Time.fixedDeltaTime);
                transform.position.Set(gameManager.trackEnds[trackOn - 1], transform.position.y, transform.position.z);
                break;
            default:
                onReachEnd();
                break;
        }
    }

    void onReachEnd()
    {
        gameManager.enemies.Remove(this);
        Destroy(this.gameObject);
    }
}
