using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public Plane floorPlane;
    public Shadow shadow;
    public float secondsPerBeat { get; private set; } = 0.125f;
    public int totalBeats { get; private set; } = 64;
    public int currentBeat = 0;
    public int beatBuffer = 1;
    public int inputDelay = 0;
    private const int NUM_KEYS = 4;

    //Array containing audio sources. One for each track
    public AudioSource[] audioSources;

    public struct BeatAction
    {
        public BeatAction(Action<BeatAction> Action, int OnKey = -1, int OnBeat = -1, int OnTrack = -1, bool OnlyOnce = false)
        {
            action = Action;
            onKey = OnKey;
            onBeat = OnBeat;

            //Special OnTrack values: -1 = note spawners
            onTrack = OnTrack;
            onlyOnce = OnlyOnce;
        }
        public Action<BeatAction> action { get; private set; }
        public int onKey { get; private set; }
        public int onBeat { get; private set; }
        public int onTrack { get; private set; }
        public bool onlyOnce { get; private set; }
    }

    //2d array containing audio clips
    public static int numTracks = 4;
    public static int numTrackVersions = 1;
    public AudioClip[,] tracks = new AudioClip[numTracks,numTrackVersions];

    //3d array containing beat lists for each key for each track. 1st dim - track, 2nd dim - version, 3rd dim - key
    public List<int>[,,] beatLists;
    public int[] currentTrackVersions = new int[numTracks];

    //2d Array containing tower actions for each key of each track
    public Action<BeatAction>[,] registerMap = new Action<BeatAction>[numTracks, NUM_KEYS];

    //Track armed for recording
    private int armedTrack = 0;

    //Current state of recording: 0 = not recording, 1 = set to record next chorus, 2 = currently recording
    private int recordState = 0;
    private int TEMPcounter = 0;

    /*
     * Array of list of BeatActions which occur on certain beats given certain keys are active
     */
    public List<BeatAction>[] actionMap;
    public List<BeatAction>[] passiveMap;

    /*
     * Array storing drop values for each key
     */
    private int[] keyDrops;

    /* List of BeatActions that were successfully hit and need to be made passive
     */
    private List<BeatAction> hitActions;

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

    //Bool array storing True if (x - buildMapOffset, y - buildMapOffset) is a position available for building towers
    public bool[,] buildMap;
    public int buildMapOffset;

    //Reference to the Tab
    public Tab tab;

    void Awake()
    {
        instance = this;

        floorPlane = new Plane(Vector3.up, -.2f);

        keyDrops = new int[NUM_KEYS];

        actionMap = new List<BeatAction>[totalBeats];
        passiveMap = new List<BeatAction>[totalBeats];

        enemies = new List<Enemy>();

        hitActions = new List<BeatAction>();

        beatLists = new List<int>[numTracks, numTrackVersions, NUM_KEYS];
        for (int i = 0; i < numTracks; i++)
        {
            for(int j = 0; j < numTrackVersions; j++)
            {
                for(int k = 0; k < NUM_KEYS; k++)
                {
                    beatLists[i, j, k] = new List<int>();
                }
            }
            currentTrackVersions[i] = 0;
        }

        loadAudio();
        loadBeatData();
        loadMapData();

        for(int i = 0; i < totalBeats; i++)
        {
            actionMap[i] = new List<BeatAction>();
            passiveMap[i] = new List<BeatAction>();
        }
    }

    void Update()
    {
        updateBeat();
        processKey();
    }

    void updateBeat()
    {
        if(currentBeat >= (totalBeats - 1) && audioSources[0].time <= (secondsPerBeat * (totalBeats - 3)))
        {
            currentBeat = 0;
            onBeat();
        } 
        else if(audioSources[0].time >= (currentBeat + 1) * secondsPerBeat)
        {
            currentBeat++;
            onBeat();
        }
    }

    void onBeat()
    {
        //Set up next track recording
        if (currentBeat == 0)
        {
            if (recordState == 1)
            {
                initRecord();
            }
            else if (recordState == 2)
            {
                finishRecord();
            }
        }

        //TEMPORARY Lock in recording for next chorus
        if (currentBeat == 32)
        {
            if(recordState == 0 && TEMPcounter == 0)
            {
                armRecord();
                TEMPcounter = 1;
            }
        }

        //Call passive actions and remove onlyOnce actions
        List<BeatAction> toRemove = new List<BeatAction>();
        foreach (BeatAction beatAction in passiveMap[currentBeat])
        {
            beatAction.action(beatAction);
            if (beatAction.onlyOnce)
            {
                toRemove.Add(beatAction);
            }
        }
        foreach (BeatAction beatAction in toRemove)
        {
            passiveMap[currentBeat].Remove(beatAction);
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

    void processKey()
    {
        if (Input.anyKeyDown)
        {
            for(int i = currentBeat - beatBuffer + 1 - inputDelay; i <= currentBeat + beatBuffer - inputDelay; i++)
            {
                foreach(BeatAction beatAction in actionMap[mod(i,totalBeats)])
                {
                    if (Input.GetKeyDown(intToKey(beatAction.onKey)))
                    {
                        if (i > (currentBeat + keyDrops[beatAction.onKey]))
                        {
                            //Call the action and add it to the hit actions
                            beatAction.action(beatAction);
                            hitActions.Add(beatAction);
                            keyDrops[beatAction.onKey] = beatBuffer - inputDelay;
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
                return "e";
            case 1:
                return "f";
            case 2:
                return "j";
            case 3:
                return "i";
            default:
                Debug.Log("Invalid int. Cannot map to key");
                return null;
        }
    }
    int keyToInt(string key)
    {
        switch (key)
        {
            case "e":
                return 0;
            case "f":
                return 1;
            case "j":
                return 2;
            case "i":
                return 3;
            default:
                Debug.Log("Invalid key. Cannot map to int");
                return -1;
        }

    }

    private void armRecord()
    {
        recordState = 1;

        //Create and add all BeatActions as only once passive actions for the note spawns for the next recording which spawn on or before the first count
        //For each key
        for (int k = 0; k < NUM_KEYS; k++)
        {
            //Check that an action exists for this key on this track
            Action<BeatAction> nextAction = registerMap[armedTrack, k];
            //If such an action exists
            if (nextAction != null)
            {
                //Take the beat list for that key
                List<int> nextList = beatLists[armedTrack, currentTrackVersions[armedTrack], k];
                //And for each beat in that list
                foreach (int beat in nextList)
                {
                    //If the beat must spawn before the first count:
                    if(beat - tab.travelBeats <= 0)
                    {
                        //Create a BeatAction for spawning a note
                        BeatAction nextBeatAction = new BeatAction(tab.spawnNote, k, beat, -1, true);
                        //Calculate the beat in which the note is spawned
                        int spawnBeat = mod(beat - tab.travelBeats, totalBeats);
                        //And add it as an active action
                        passiveMap[spawnBeat].Add(nextBeatAction);
                    }
                }
            }

        }
    }
    private void initRecord()
    {
        recordState = 2;

        hitActions.Clear();

        //Remove all passive actions for this track
        //For each key
        for(int k = 0; k < NUM_KEYS; k++)
        {
            //Take the beat list for that key on the track to be recorded
            List<int> nextList = beatLists[armedTrack, currentTrackVersions[armedTrack], k];
            //And for each beat in that list
            foreach(int beat in nextList)
            {
                //Go to each action in the passive action list for that beat
                for(int i = 0; i < passiveMap[beat].Count; i++)
                {
                    //And remove it if it belongs to this track
                    if(passiveMap[beat][i].onTrack == armedTrack)
                    {
                        passiveMap[beat].RemoveAt(i);
                    }
                }
            }
        }

        //Create and add all BeatActions for this track as active actions and add all spawn actions that occurr after the first count
        //For each key
        for(int k = 0; k < NUM_KEYS; k++)
        {
            //Find the action registered to that key on this track
            Action<BeatAction> nextAction = registerMap[armedTrack, k];
            //If that action exists
            if(nextAction != null)
            {
                //Take the beat list for that key
                List<int> nextList = beatLists[armedTrack, currentTrackVersions[armedTrack], k];
                //And for each beat in that list
                foreach (int beat in nextList)
                {
                    //Create a BeatAction for that action
                    BeatAction nextBeatAction = new BeatAction(nextAction, k, beat, armedTrack);
                    //And add it as an active action
                    actionMap[beat].Add(nextBeatAction);

                    //If the beat must spawn after the first count:
                    if (beat - tab.travelBeats > 0)
                    {
                        //Create a BeatAction for spawning a note
                        nextBeatAction = new BeatAction(tab.spawnNote, k, beat, -1, true);
                        //Calculate the beat in which the note is spawned
                        int spawnBeat = mod(beat - tab.travelBeats, totalBeats);
                        //And add it as an active action
                        passiveMap[spawnBeat].Add(nextBeatAction);
                    }
                }
            }
            
        }
    }

    private void finishRecord()
    {
        //Add all hit actions to the passiveMap
        foreach(BeatAction beatAction in hitActions)
        {
            passiveMap[beatAction.onBeat].Add(beatAction);
        }

        //Remove all active actions for this track
        //For each key
        for (int k = 0; k < NUM_KEYS; k++)
        {
            //Take the beat list for that key on this track 
            List<int> nextList = beatLists[armedTrack, currentTrackVersions[armedTrack], k];
            //And for each beat in that list
            foreach (int beat in nextList)
            {
                //Go to each action in the active action list for that beat
                for (int i = 0; i < actionMap[beat].Count; i++)
                {
                    //And remove it if it belongs to this track
                    if (actionMap[beat][i].onTrack == armedTrack)
                    {
                        actionMap[beat].RemoveAt(i);
                    }
                }
            }
        }

        recordState = 0;
    }

    private void loadAudio()
    {
        tracks[0, 0] = (AudioClip)Resources.Load("Audio/blignal_bass_0");
        tracks[1, 0] = (AudioClip)Resources.Load("Audio/blignal_back_0");
        tracks[2, 0] = (AudioClip)Resources.Load("Audio/blignal_back_1");
        tracks[3, 0] = (AudioClip)Resources.Load("Audio/blignal_lead_0");
    }

    private void loadBeatData()
    {
        beatLists[0, 0, 0].Add(0);
        beatLists[0, 0, 0].Add(8);
        beatLists[0, 0, 0].Add(16);
        beatLists[0, 0, 0].Add(24);
        beatLists[0, 0, 3].Add(32);
        beatLists[0, 0, 3].Add(40);
        beatLists[0, 0, 2].Add(48);
        beatLists[0, 0, 1].Add(56);
    }

    public bool canBuildHere(int x, int z)
    {
        if(x + buildMapOffset < 0 || z + buildMapOffset < 0 || x + buildMapOffset >= buildMap.GetLength(0) || z + buildMapOffset >= buildMap.GetLength(1))
        {
            return false;
        }
        return buildMap[x + buildMapOffset, z + buildMapOffset];
    }
    private void loadMapData()
    {
        buildMapOffset = 4;
        buildMap = new bool[9,9];

        buildMap[0, 0] = false;
        buildMap[0, 1] = false;
        buildMap[0, 2] = false;
        buildMap[0, 3] = false;
        buildMap[0, 4] = false;
        buildMap[0, 5] = false;
        buildMap[0, 6] = false;
        buildMap[0, 7] = false;
        buildMap[0, 8] = false;

        buildMap[1, 0] = false;
        buildMap[1, 1] = false;
        buildMap[1, 2] = false;
        buildMap[1, 3] = false;
        buildMap[1, 4] = false;
        buildMap[1, 5] = false;
        buildMap[1, 6] = false;
        buildMap[1, 7] = false;
        buildMap[1, 8] = false;

        buildMap[2, 0] = true;
        buildMap[2, 1] = true;
        buildMap[2, 2] = true;
        buildMap[2, 3] = true;
        buildMap[2, 4] = true;
        buildMap[2, 5] = true;
        buildMap[2, 6] = true;
        buildMap[2, 7] = false;
        buildMap[2, 8] = true;

        buildMap[3, 0] = true;
        buildMap[3, 1] = false;
        buildMap[3, 2] = false;
        buildMap[3, 3] = false;
        buildMap[3, 4] = false;
        buildMap[3, 5] = false;
        buildMap[3, 6] = false;
        buildMap[3, 7] = false;
        buildMap[3, 8] = true;

        buildMap[4, 0] = true;
        buildMap[4, 1] = false;
        buildMap[4, 2] = true;
        buildMap[4, 3] = true;
        buildMap[4, 4] = true;
        buildMap[4, 5] = true;
        buildMap[4, 6] = true;
        buildMap[4, 7] = true;
        buildMap[4, 8] = true;

        buildMap[5, 0] = true;
        buildMap[5, 1] = false;
        buildMap[5, 2] = false;
        buildMap[5, 3] = false;
        buildMap[5, 4] = false;
        buildMap[5, 5] = false;
        buildMap[5, 6] = false;
        buildMap[5, 7] = false;
        buildMap[5, 8] = true;

        buildMap[6, 0] = true;
        buildMap[6, 1] = true;
        buildMap[6, 2] = true;
        buildMap[6, 3] = true;
        buildMap[6, 4] = true;
        buildMap[6, 5] = true;
        buildMap[6, 6] = true;
        buildMap[6, 7] = false;
        buildMap[6, 8] = true;

        buildMap[7, 0] = false;
        buildMap[7, 1] = false;
        buildMap[7, 2] = false;
        buildMap[7, 3] = false;
        buildMap[7, 4] = false;
        buildMap[7, 5] = false;
        buildMap[7, 6] = false;
        buildMap[7, 7] = false;
        buildMap[7, 8] = false;

        buildMap[8, 0] = false;
        buildMap[8, 1] = false;
        buildMap[8, 2] = false;
        buildMap[8, 3] = false;
        buildMap[8, 4] = false;
        buildMap[8, 5] = false;
        buildMap[8, 6] = false;
        buildMap[8, 7] = false;
        buildMap[8, 8] = false;
    }
}
