using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeatManager : MonoBehaviour
{
    //Singleton instance variable
    public static BeatManager instance { get; private set; }

    //Wrapper variables for setting initial values in the editor
    [SerializeField]
    private float SecondsPerBeat = .125f;
    [SerializeField]
    private int TotalBeats = 64, BeatBuffer = 1, InputDelay = 1, NumTracks = 5, NumTrackVersions = 4;
    [SerializeField]
    int[] InitialTrackVersions;

    //Variables set before running the system:
    public float secondsPerBeat { get; private set; }
    public int totalBeats { get; private set; }
    public int numKeys { get; private set; }
    public int beatBuffer { get; private set; }
    //# of beats of forgiveness in both directions
    public int inputDelay { get; private set; }
    //# of beats input is shifted by

    public int currentBeat { get; private set; } = 0;

    //One audio source for each track in order, set in inspector
    protected AudioSource[] audioSources;
    //The keys used as input in order
    public string[] keys;

    //Data about the tracks
    public int numTracks { get; private set; }
    public int numTrackVersions { get; private set; }
    private AudioClip[,] tracks;
    private int[] currentTrackVersions;

    //Data about the state of the component
    public bool isRunning { get; private set; } = false;
    public bool initialized { get; private set; } = false;
    public bool audioLoaded { get; private set; } = false;

    //Lists of actions
    private List<BeatAction>[] actionMap;
    private List<BeatAction>[] passiveMap;

    //Number of beats left unti a key can be input again for each key
    private int[] keyDrops;

    //Array holding the max volume and current unaltered volume for each track
    public float[] volumes;
    protected float[] currentVolumes;

    //List of active actions that need to be added once their active range has passed
    private List<BeatAction> beatActionsToAdd;

    //Struct holding actions and information about them
    public struct BeatAction
    {
        public BeatAction(Action<BeatAction> Action, int OnKey = -1, int OnBeat = -1, int OnTrack = -1, bool OnlyOnce = false, bool IsManaging = false, int Data = 0)
        {
            action = Action;
            onKey = OnKey;
            onBeat = OnBeat;

            //Special OnTrack values: -1 = note spawners
            onTrack = OnTrack;
            onlyOnce = OnlyOnce;
            isManaging = IsManaging;
            data = Data;
        }
        public Action<BeatAction> action { get; private set; }
        public int onKey { get; private set; }
        public int onBeat { get; private set; }
        public int onTrack { get; private set; }
        public bool onlyOnce { get; private set; }
        public bool isManaging { get; private set; }
        public int data { get; private set; }
    }

    private void Awake()
    {
        BeatManagerAwake();
    }

    protected void BeatManagerAwake()
    {
#       if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (BeatManager.instance != null)
                {
                    Debug.Log("Cannot instantiate BeatManager when another BeatManager already exists");
                }
#       endif
        instance = this;

        Initialize(SecondsPerBeat, TotalBeats, NumTracks, NumTrackVersions, BeatBuffer, InputDelay);
    }

    private void Initialize(float SecondsPerBeat, int TotalBeats, int NumTracks, int NumTrackVersions, int BeatBuffer = 1, int InputDelay = 1)
    {
#if     UNITY_EDITOR || DEVELOPMENT_BUILD
            if (initialized || isRunning || audioLoaded)
            {
                Debug.Log("BeatManager can only initialize once and must initialize first");
            }
#       endif

        //Set parameters
        secondsPerBeat = SecondsPerBeat;
        totalBeats = TotalBeats;
        numKeys = keys.Length;
        numTracks = NumTracks;
        numTrackVersions = NumTrackVersions;
        beatBuffer = BeatBuffer;
        inputDelay = InputDelay;

        //Instatiate arrays and lists
        currentBeat = 0;
        currentTrackVersions = new int[numTracks];
        audioSources = new AudioSource[numTracks];
        currentVolumes = new float[numTracks];
        for(int i = 0; i < numTracks; i++)
        {
            currentTrackVersions[i] = InitialTrackVersions[i];
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].loop = true;
            if(InitialTrackVersions[i] < 0)
            {
                audioSources[i].volume = 0;
                currentVolumes[i] = 0;
            }
            else
            {
                currentVolumes[i] = volumes[i];
            }
        }
        keyDrops = new int[numKeys];
        actionMap = new List<BeatAction>[totalBeats];
        passiveMap = new List<BeatAction>[totalBeats];
        for(int i = 0; i < totalBeats; i++)
        {
            actionMap[i] = new List<BeatAction>();
            passiveMap[i] = new List<BeatAction>();
        }

        beatActionsToAdd = new List<BeatAction>();

        initialized = true;

        //Debug checks
#       if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(audioSources == null)
                {
                    Debug.Log("Audio source references missing");
                }
            if(keys == null)
                {
                    Debug.Log("Input key list missing");
                }
#       endif
    }

    public void LoadAudio(AudioClip[,] Tracks)
    {
#       if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (!initialized)
                {
                    Debug.Log("BeatManager must be initialized before audio is loaded");
                }
#       endif

        audioLoaded = true;

        tracks = Tracks;
    }
    
    public void setTrackVersion(int track, int version)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!initialized || !audioLoaded)
            {
                Debug.Log("BeatManager must be initialized and audio loaded before setting track versions");
            }
            if (track < 0 || track >= numTracks)
            {
                Debug.Log("Invalid track #");
            }
            if (version < 0 || version >= numTrackVersions)
            {
                Debug.Log("Invalid track version #");
            }
#       endif

        if(version < 0)
        {
            currentTrackVersions[track] = version;
            audioSources[track].volume = 0f;
        } else
        {
            audioSources[track].clip = tracks[track, version];
            if (currentTrackVersions[track] < 0)
            {
                audioSources[track].volume = 1f;
            }
            currentTrackVersions[track] = version;
        }
    }
    public void Play()
    {
#       if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!audioLoaded || !initialized)
            {
                Debug.Log("You must load audio and initialize before playing BeatManager");
            }
            if (isRunning)
            {
                Debug.Log("You can not play BeatManager if it is already playing");
            }
#       endif

        for (int i = 0; i < numTracks; i++)
        {
            if(currentTrackVersions[i] < 0)
            {
                audioSources[i].clip = tracks[i, 0];

            }
            else
            {
                audioSources[i].clip = tracks[i, currentTrackVersions[i]];
            }
            audioSources[i].Play();
        }

        isRunning = true;
    }

    protected virtual void Update()
    {
        if (isRunning)
        {
            UpdateBeat();
            ProcessKey();
        }
    }

    private void UpdateBeat()
    {
        if (currentBeat >= (totalBeats - 1) && audioSources[0].time <= (secondsPerBeat * (totalBeats - 3)))
        {
            currentBeat = 0;
            OnBeat();
        }
        else if (audioSources[0].time >= (currentBeat + 1) * secondsPerBeat)
        {
            currentBeat++;
            OnBeat();
        }
    }

    private void OnBeat()
    {
        //Call passive actions (managers first) and remove onlyOnce actions
        List<BeatAction> toRemove = new List<BeatAction>();

        //Managers
        for(int i = 0; i < passiveMap[currentBeat].Count && passiveMap[currentBeat][i].isManaging; i++)
        {
            BeatAction beatAction = passiveMap[currentBeat][i];
            beatAction.action(beatAction);
            if (beatAction.onlyOnce)
            {
                toRemove.Add(beatAction);
            }
        }
        //Non Managers that are left
        foreach (BeatAction beatAction in passiveMap[currentBeat])
        {
            if (!beatAction.isManaging)
            {
                beatAction.action(beatAction);
                if (beatAction.onlyOnce)
                {
                    toRemove.Add(beatAction);
                }
            }
        }
        foreach (BeatAction beatAction in toRemove)
        {
            passiveMap[currentBeat].Remove(beatAction);
        }

        //Remove active actions from the beat who just became out of range
        int clearBeat = Mod(currentBeat - beatBuffer - inputDelay, totalBeats);
        for(int i = actionMap[clearBeat].Count - 1; i >= 0 ; i--)
        {
            if (actionMap[clearBeat][i].onlyOnce)
            {
                actionMap[clearBeat].RemoveAt(i);
            }
        }

        //Drop hit keys
        for (int i = 0; i < numKeys; i++)
        {
            if (keyDrops[i] > -beatBuffer - inputDelay)
            {
                keyDrops[i]--;
            }
        }
    }

    private void ProcessKey()
    {
        //TODO CHECK FOR MORE MODS NEEDED
        //If a key is pressed
        if (Input.anyKeyDown)
        {
            //For each beat within the active range
            for (int i = currentBeat - beatBuffer + 1 - inputDelay; i <= currentBeat + beatBuffer - inputDelay; i++)
            {
                List<BeatAction> toRemove = new List<BeatAction>();
                //For each active action on that beat
                foreach (BeatAction beatAction in actionMap[Mod(i, totalBeats)])
                {
                    //If the key is pressed for that action
                    if (Input.GetKeyDown(IntToKey(beatAction.onKey)))
                    {
                        //If the key isn't on cooldown
                        if (i > (currentBeat + keyDrops[beatAction.onKey]))
                        {
                            //Call the action
                            beatAction.action(beatAction);
                            //Set the cooldown for that key
                            keyDrops[beatAction.onKey] = beatBuffer - inputDelay;
                            //If the action is only once add it to the list
                            if (beatAction.onlyOnce)
                            {
                                toRemove.Add(beatAction);
                            }
                        }
                    }
                }
                foreach(BeatAction beatAction in toRemove)
                {
                    actionMap[Mod(i, totalBeats)].Remove(beatAction);
                }
            }
        }
    }

    public void AddActiveAction(BeatAction beatAction)
    {
        if(beatAction.onBeat >= Mod(currentBeat - beatBuffer - inputDelay - 1, totalBeats))
        {
            beatActionsToAdd.Add(beatAction);
            Invoke("ActuallyAddActiveAction", beatBuffer * 4 * secondsPerBeat);
        }
        else
        {
            actionMap[beatAction.onBeat].Add(beatAction);
        }
    }

    private void ActuallyAddActiveAction()
    {
        actionMap[beatActionsToAdd[0].onBeat].Add(beatActionsToAdd[0]);
        beatActionsToAdd.RemoveAt(0);
    }

    public void AddPassiveAction(BeatAction beatAction)
    {
        if (beatAction.isManaging)
        {
            passiveMap[beatAction.onBeat].Insert(0, beatAction);
        }
        else
        {
            passiveMap[beatAction.onBeat].Add(beatAction);
        }
    }

    public void RemoveActiveAction(BeatAction beatAction)
    {
        actionMap[beatAction.onKey].Remove(beatAction);
    }

    public void RemovePassiveAction(BeatAction beatAction)
    {
        passiveMap[beatAction.onKey].Remove(beatAction);
    }

    public void RemoveAllPassive(int beat, int track)
    {
        List<BeatAction> actionList = passiveMap[beat];
        for(int i = actionList.Count - 1; i >= 0; i--)
        {
            if(actionList[i].onTrack == track)
            {
                actionList.RemoveAt(i);
            }
        }
    }

    public void ChangeTrackVersion(int track, int version)
    {
        if (version < 0)
        {
            audioSources[track].volume = 0;
            currentVolumes[track] = 0;
        }
        else 
        {
            if (currentTrackVersions[track] < 0)
            {
                audioSources[track].volume = volumes[track];
                currentVolumes[track] = volumes[track];
            }
            audioSources[track].clip = tracks[track, version];

            if (isRunning)
            {
                audioSources[track].Play();
                audioSources[track].time = audioSources[Mod(track + 1, numTracks)].time;
            }
        }
        currentTrackVersions[track] = version;
    }

    public int TrackVersion(int track)
    {
        return currentTrackVersions[track];
    }

    static public int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public string IntToKey(int num)
    {
#if     UNITY_EDITOR || DEVELOPMENT_BUILD
            if (num >= keys.Length || num < 0)
            {
                Debug.Log("Cannot convert int to key. Invalid int.");
            }
#       endif
        return keys[num];
    }
    public int KeyToInt(string key)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if(keys[i] == key)
            {
                return i;
            }
        }

#if     UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Cannot convert key to int. Invalid key.");
#       endif

        return 0;
    }
}
