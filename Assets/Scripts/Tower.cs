using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected GameManager gameManager;
    public List<int> attackBeats;
    public string key;
    static int noteFallBeats = 32;
    float barHeight = .2f;
    static float noteSpawnHeight = 8f;
    private float noteSpeed;
    public bool active = false;

    public Note notePrefab;
    void Start()
    {
        gameManager = GameManager.instance;
        noteSpeed = (transform.lossyScale.y)*(noteSpawnHeight - barHeight) / (noteFallBeats * gameManager.secondsPerBeat); 
        registerBeats();
    }
    void registerBeats()
    {
        //Register attacks
        foreach(int beat in attackBeats)
        {
            if (active)
            {
                gameManager.actionMap[beat].Add(new GameManager.BeatAction(attack, key));
            } else
            {
                gameManager.passiveMap[beat].Add(attack);
            }
            
        }

        //Register note spawns
        foreach(int beat in attackBeats)
        {
            int spawnBeat = gameManager.mod(beat - noteFallBeats + gameManager.inputDelay, gameManager.totalBeats);
            gameManager.passiveMap[spawnBeat].Add(spawnNote);
        }
    }

    protected virtual void spawnNote()
    {
        Note note = Object.Instantiate(notePrefab, transform);
        note.transform.localPosition = new Vector3(0, noteSpawnHeight, 0);
        note.initialize(noteSpeed, noteFallBeats);
    }
    protected virtual void attack()
    {
        Debug.Log("Calls general Tower attack");
    }
}
