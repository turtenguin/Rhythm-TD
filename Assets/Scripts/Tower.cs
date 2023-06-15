using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected GameManager gameManager;
    public int track;
    public int key;
    static int noteFallBeats = 32;
    float barHeight = .2f;
    static float noteSpawnHeight = 8f;
    private float noteSpeed;
    public bool active = false;
    void Start()
    {
        gameManager = GameManager.instance;
        noteSpeed = (transform.lossyScale.y)*(noteSpawnHeight - barHeight) / (noteFallBeats * gameManager.secondsPerBeat);
        registerTower();
    }

    void registerTower()
    {
        gameManager.registerMap[track, key] = attack;
    }

    protected virtual void attack(GameManager.BeatAction beatAction)
    {
        Debug.Log("Calls general Tower attack");
    }
}
