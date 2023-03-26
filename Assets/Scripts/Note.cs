using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Note : MonoBehaviour
{
    private Vector3 speed;
    private GameManager gameManager;
    private int destroyBeat;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed*Time.deltaTime);
    }

    public void initialize(float ySpeed, int noteFallBeats)
    {
        speed = new Vector3(0, -ySpeed, 0);
        gameManager = GameManager.instance;
        gameManager.passiveMap[gameManager.mod(gameManager.currentBeat + noteFallBeats, gameManager.totalBeats)].Add(destroy);
    }
    public void destroy()
    {
        destroyBeat = gameManager.currentBeat;
        Invoke("delete", gameManager.secondsPerBeat*2);
        gameObject.SetActive(false);
    }

    private void delete()
    {
        gameManager.passiveMap[destroyBeat].Remove(destroy);
        Destroy(gameObject);
    }
}
