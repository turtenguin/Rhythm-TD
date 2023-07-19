using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RecordManager : BeatManager
{
    //TEMPORARY test variables
    public bool play = false;
    public int ArmedTrack = 0;
    public bool armRecord = false;
    public int trackChange = 0;
    public int versionChange = 0;
    public bool changeVersion = false;

    //Singleton instance variable
    public static RecordManager recordManagerInstance;

    //Reference to GameManager
    private GameManager gameManager;

    //Track Versions currently being run by towers (could be different from audio)
    private int[] passiveTrackVersions;

    //BeatActions registered for each track for each key
    private Action<BeatManager.BeatAction>[,] registerMap;

    //Which track is to be recorded next
    public int armedTrack { get; private set; } = 1;

    //0 if not recording, 1 if armed for next loop, 2 if currently recording
    public int recordState { get; private set; }

    //List of BeatActions that were hit and should be moved to passive actions
    List<BeatManager.BeatAction> hitActions;

    //List of beats present on each track for for each version for each key
    List<int>[,,] beatLists;

    //Reference to the Tab
    public Tab tab;

    //List of notes exitsing now
    private List<Note> noteList;

    //Array mapping tower type to track such that towerTrackMap[towerType] = track
    public int[] towerTrackMap;

    //Callback list for arming, starting, and ending record
    public List<Action> onArmed = new List<Action>();
    public List<Action> onStartRecord = new List<Action>();
    public List<Action> onEndRecord = new List<Action>();

    //Variables related to audio fade
    public float fadeAmount = .5f;
    private float fadeOutSpeed = 1;
    private float fadeInSpeed = 1;
    private bool fadingOut = false;
    private bool fadingIn = false;
    private float fadeInTracker = 0;
    private float fadeOutTracker = 0;
    private int fadeInTrack = 0;

    public bool loaded { get; private set; }
    public bool RInitialized { get; private set; }

    //Array storing the number of hits on each track for each key
    private int[,] hits;

    void FixedUpdate()
    {
        if (play)
        {
            Play();
            play = false;
        }
        if (armRecord)
        {
            ArmRecord(ArmedTrack);
            armRecord = false;
        }
        if (changeVersion)
        {
            ChangeTrackVersion(trackChange, versionChange);
            changeVersion = false;
        }
    }
    private void Awake()
    {
#       if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (RecordManager.instance != null)
            {
                Debug.Log("Cannot instantiate BeatManager when another BeatManager already exists");
            }
#       endif
        recordManagerInstance = this;

        BeatManagerAwake();
        RInitialize();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        fadeInSpeed = fadeAmount / gameManager.transitionToShopTime;
        Play();
    }

    protected override void Update()
    {
        base.Update();

        ProcessFades();
    }

    private void RInitialize()
    {
        registerMap = new Action<BeatManager.BeatAction>[numTracks, numKeys];
        hitActions = new List<BeatManager.BeatAction>();

        BeatAction onBeatZeroAction = new BeatAction(OnBeatZero, 0, 0, -1, false, true);
        AddPassiveAction(onBeatZeroAction);

        passiveTrackVersions = new int[numTracks];
        for(int i = 0; i < numTracks; i++)
        {
            passiveTrackVersions[i] = 0;
        }

        noteList = new List<Note>();

        hits = new int[numTracks,numKeys];
        for(int i = 0; i < numTracks; i++)
        {
            for(int k = 0; k < numKeys; k++)
            {
                hits[i, k] = 0;
            }
        }
    }

    public void LoadBeatData(List<int>[,,] BeatLists)
    {
        beatLists = BeatLists;

        loaded = true;
    }

    private void OnBeatZero(BeatAction beatAction)
    {
        if(recordState == 1)
        {
            InitRecord();
        } else if (recordState == 2)
        {
            finishRecord();
        }
    }

    private void InitRecord()
    {
        recordState = 2;
        hitActions.Clear();
        
        //Reset hit count for this track
        for(int k = 0; k < numKeys; k++)
        {
            hits[armedTrack,k] = 0;
        }

        //For each key
        for(int k = 0; k < numKeys; k++)
        {
            //Take the beat list for that key on the track currently loaded into those towers
            List<int> beatList = beatLists[armedTrack, passiveTrackVersions[armedTrack], k];
            //And for each beat in that list
            foreach(int beat in beatList)
            {
                //Remove all passive actions from that beat that belong to this track
                RemoveAllPassive(beat, armedTrack);
            }
        }
        //Then set the passive track versions to the new track
        passiveTrackVersions[armedTrack] = TrackVersion(armedTrack);

        //Create and add all active actions for this track and all spawn note actions that occur after count 0
        //For each key
        for(int k = 0; k < numKeys; k++)
        {
            //If there is an action registered to that key on this track
            if (registerMap[armedTrack, k] != null)
            {
                //Take the beat list for that key
                List<int> beatList = beatLists[armedTrack, TrackVersion(armedTrack), k];
                //And for each beat in that list
                foreach (int beat in beatList)
                {
                    if(beat == 62 || beat == 63)
                    {
                        Debug.Log("Place");
                    }
                    //Create a Record BeatAction
                    BeatAction newAction = new BeatAction(RecordNote, k, beat, armedTrack, true);
                    //And add it as an active action
                    AddActiveAction(newAction);

                    //Also, if the note for this beat must spawn after the first count
                    if (beat - tab.travelBeats > 0)
                    {
                        //Calculate the beat in which the note is spawned
                        int spawnBeat = Mod(beat - tab.travelBeats, totalBeats);
                        //Create a BeatAction for spawning a note
                        BeatAction noteAction = new BeatAction(tab.SpawnNote, k, spawnBeat, -1, true, false, beat);
                        //And add it as an active action
                        AddPassiveAction(noteAction);
                    }
                }
            }
        }

        foreach(Action action in onStartRecord)
        {
            action();
        }
    }

    private void RecordNote(BeatAction beatAction)
    {
        //Create a beatAction for the action registered to this key and track
        BeatAction newAction = new BeatAction(registerMap[beatAction.onTrack, beatAction.onKey], beatAction.onKey, beatAction.onBeat, beatAction.onTrack);
        //Do the action
        newAction.action(newAction);
        //And add it as a passive action
        AddPassiveAction(newAction);
        //And track this hit
        hits[armedTrack, beatAction.onKey]++;

        //Find the corresponding note and activate it
        for(int i = 0; i < noteList.Count; i++)
        {
            if(noteList[i].beatAction.data == beatAction.onBeat && noteList[i].beatAction.onKey == beatAction.onKey)
            {
                noteList[i].OnPlayed();
                break;
            }
        }

        Debug.Log(beatAction.onTrack.ToString() + "/" + beatAction.onKey.ToString() + "/" + beatAction.onBeat.ToString());
    }

    private void finishRecord()
    {
        recordState = 0;

        foreach(Action action in onEndRecord)
        {
            action();
        }

        //Initialize Fade In
        fadingIn = true;
        fadeInTracker = 0;
        fadeInTrack = armedTrack;
    }

    public void ArmRecord(int towerType)
    {
        armedTrack = towerTrackMap[towerType];
        recordState = 1;

        //Initialize Fade
        fadeOutSpeed = (fadeAmount / (totalBeats - currentBeat)) / secondsPerBeat;
        fadingOut = true;
        fadeOutTracker = 0;

        //Create and add note spawn actions for spawns on or before count 0
        //For each key
        for (int k = 0; k < numKeys; k++)
        {
            //If an action exists for this key on this track
            if(registerMap[armedTrack, k] != null)
            {
                //Take the beat list for that key
                List<int> beatList = beatLists[armedTrack, TrackVersion(armedTrack), k];
                //And for each beat in that list
                foreach (int beat in beatList)
                {
                    //If the beat must spawn before the first count:
                    if(beat - tab.travelBeats <= 0)
                    {
                        //Calculate the beat in which the note is spawned
                        int spawnBeat = Mod(beat - tab.travelBeats, totalBeats);
                        //Create a BeatAction for spawning a note
                        BeatAction noteAction = new BeatAction(tab.SpawnNote, k, spawnBeat, -1, true, false, beat);
                        //And add it as a passive action
                        AddPassiveAction(noteAction);
                    }
                }
            }
        }

        foreach(Action action in onArmed)
        {
            action();
        }
    }

    public void RegisterSelf(Action<BeatAction> action, int track, int key)
    {
        registerMap[track, key] = action;
    }

    public bool RegisterTower(Tower tower)
    {
        int track = towerTrackMap[tower.towerType];
        tower.track = track;

        if (TrackVersion(track) < 0)
        {
            ChangeTrackVersion(track, 0);
        }

        //Find appropriate key
        for (int k = 0; k < numKeys; k++)
        {
            if(registerMap[track, k] == null)
            {
                registerMap[track, k] = tower.attackAction;
                tower.key = k;
                return true;
            }
        }

        return false;
    }

    public void RegisterNote(Note note)
    {
        noteList.Add(note);
    }

    public void RemoveNote(Note note)
    {
        noteList.Remove(note);
    }

    public int hitsForTower(int towerType, int key)
    {
        return hits[towerTrackMap[towerType], key];
    }

    public int hitsForTower(int towerType)
    {
        int track = towerTrackMap[towerType];
        int sum = 0;
        for(int k = 0; k < numKeys; k++)
        {
            sum += hits[track, k];
        }
        return sum;
    }

    public int hitsAvailable(int towerType, int key)
    {
        int track = towerTrackMap[towerType];
        int trackVersion = TrackVersion(track);
        if(trackVersion < 0)
        {
            return 0;
        }
        return beatLists[track, trackVersion, key].Count;
    }

    public int hitsAvailable(int towerType)
    {
        int track = towerTrackMap[towerType];
        int sum = 0;
        int trackVersion = TrackVersion(track);
        if (trackVersion < 0)
        {
            return 0;
        }
        for (int k = 0; k < numKeys; k++)
        {
            sum += beatLists[track, trackVersion, k].Count;
        }
        return sum;
    }

    public int hitsAvailableOnVersion(int towerType, int trackVersion, int key)
    {
        int track = towerTrackMap[towerType];
        return beatLists[track, trackVersion, key].Count;
    }
    public int hitsAvailableOnVersion(int towerType, int trackVersion)
    {
        int track = towerTrackMap[towerType];
        int sum = 0;
        if (trackVersion < 0)
        {
            return 0;
        }
        for (int k = 0; k < numKeys; k++)
        {
            sum += beatLists[track, trackVersion, k].Count;
        }
        return sum;
    }


    private void ProcessFades()
    {
        if (fadingOut)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (i != armedTrack)
                {
                    audioSources[i].volume -= fadeOutSpeed * Time.deltaTime;
                }
            }
            fadeOutTracker += fadeOutSpeed * Time.deltaTime;
            if (fadeOutTracker >= fadeAmount)
            {
                fadingOut = false;
            }
        }
        else if (fadingIn)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (i != fadeInTrack && audioSources[i].volume < currentVolumes[i])
                {
                    audioSources[i].volume += fadeInSpeed * Time.deltaTime;
                }
            }
            fadeInTracker += fadeInSpeed * Time.deltaTime;
            if (fadeInTracker >= fadeAmount)
            {
                fadingIn = false;
            }
        }
    }

    public int TowerTypeToTrack(int towerType)
    {
        return towerTrackMap[towerType];
    }
}
