using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected GameManager gameManager;
    protected RecordManager recordManager;
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
        recordManager = RecordManager.recordManagerInstance;
        noteSpeed = (transform.lossyScale.y)*(noteSpawnHeight - barHeight) / (noteFallBeats * recordManager.secondsPerBeat);
        registerTower();
    }

    void registerTower()
    {
        //gameManager.registerMap[track, key] = attack;
        recordManager.RegisterSelf(attack, track, key);
        gameManager.RegisterTowerPosition(transform.position);
    }

    protected virtual void attack(BeatManager.BeatAction beatAction)
    {
        Debug.Log("Calls general Tower attack");
    }
}
