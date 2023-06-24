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

    public bool loaded { get; private set; }
    public bool RInitialized { get; private set; }

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
    }

    private void RecordNote(BeatAction beatAction)
    {
        //Create a beatAction for the action registered to this key and track
        BeatAction newAction = new BeatAction(registerMap[beatAction.onTrack, beatAction.onKey], beatAction.onKey, beatAction.onBeat, beatAction.onTrack);
        //Do the action
        newAction.action(newAction);
        //And add it as a passive action
        AddPassiveAction(newAction);

        //Find the corresponding note and activate it
        for(int i = 0; i < noteList.Count; i++)
        {
            if(noteList[i].beatAction.data == beatAction.onBeat && noteList[i].beatAction.onKey == beatAction.onKey)
            {
                noteList[i].OnPlayed();
                break;
            }
        }
    }

    private void finishRecord()
    {
        recordState = 0;
    }

    public void ArmRecord(int track)
    {
        armedTrack = track;
        recordState = 1;

        //Create and add note spawn actions for spawns on or before count 0
        //For each key
        for(int k = 0; k < numKeys; k++)
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
    }

    public void RegisterSelf(Action<BeatAction> action, int track, int key)
    {
        registerMap[track, key] = action;
    }

    public void RegisterNote(Note note)
    {
        noteList.Add(note);
    }

    public void RemoveNote(Note note)
    {
        noteList.Remove(note);
    }
}
