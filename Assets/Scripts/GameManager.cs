using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public float secondsPerBeat { get; private set; } = 0.125f;
    public int totalBeats { get; private set; } = 64;
    public int currentBeat = 0;
    public int beatBuffer = 1;
    public int inputDelay = 0;

    public AudioSource mainAudio;

    public struct BeatAction
    {
        public BeatAction(Action Action, string OnKey)
        {
            action = Action;
            onKey = OnKey;
        }
        public Action action { get; private set; }
        public string onKey { get; private set; }
    }
    
    /*
     * Array of list of BeatActions which occur on certain beats given certain keys are active
     */
    public List<BeatAction>[] actionMap;
    public List<Action>[] passiveMap;

    /*
     * Array storing drop values for each key
     */
    private int[] keyDrops;
    private const int NUM_KEYS = 4;

    public List<Enemy> enemies;

    static public GameManager instance;

    /* Each element contains direction of given track segment
     * 0 = +x
     * 1 = -x
     * 2 = +z
     * 3 = -z
     * 4 = end
     */
    public int[] trackDirs;
    //each element contains the endpoint of given track segment
    public int[] trackEnds;
    void Awake()
    {
        instance = this;

        keyDrops = new int[NUM_KEYS];

        actionMap = new List<BeatAction>[totalBeats];
        passiveMap = new List<Action>[totalBeats];

        enemies = new List<Enemy>();

        for(int i = 0; i < totalBeats; i++)
        {
            actionMap[i] = new List<BeatAction>();
            passiveMap[i] = new List<Action>();
        }
    }

    void Update()
    {
        updateBeat();

        onKey();
    }

    void updateBeat()
    {
        if(currentBeat >= (totalBeats - 1) && mainAudio.time <= (secondsPerBeat * (totalBeats - 3)))
        {
            currentBeat = 0;
            onBeat();
        } 
        else if(mainAudio.time >= (currentBeat + 1) * secondsPerBeat)
        {
            currentBeat++;
            onBeat();
        }
    }

    void onBeat()
    {
        //Call passive actions
        foreach (Action action in passiveMap[currentBeat])
        {
            action();
        }

        //Drop hit keys
        for(int i = 0; i < NUM_KEYS; i++)
        {
            if(keyDrops[i] > -beatBuffer - inputDelay)
            {
                keyDrops[i]--;
            }
        }
    }

    void onKey()
    {
        if (Input.anyKeyDown)
        {
            for(int i = currentBeat - beatBuffer + 1 - inputDelay; i <= currentBeat + beatBuffer - inputDelay; i++)
            {
                foreach(BeatAction beatAction in actionMap[mod(i,totalBeats)])
                {
                    if (Input.GetKeyDown(beatAction.onKey))
                    {
                        if (i > (currentBeat + keyDrops[keyToInt(beatAction.onKey)]))
                        {
                            beatAction.action();
                            keyDrops[keyToInt(beatAction.onKey)] = beatBuffer - inputDelay;
                        }
                    }
                }
            }
        }
    }

    public int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    string intToKey(int num)
    {
        switch (num)
        {
            case 0:
                return "space";
            case 1:
                return "e";
            case 2:
                return "f";
            case 3:
                return "j";
            default:
                Debug.Log("Invalid int. Cannot map to key");
                return null;
        }
    }
    int keyToInt(string key)
    {
        switch (key)
        {
            case "space":
                return 0;
            case "e":
                return 1;
            case "f":
                return 2;
            case "j":
                return 3;
            default:
                Debug.Log("Invalid key. Cannot map to int");
                return -1;
        }

    }
}
