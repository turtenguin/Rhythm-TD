using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MusicLoader : MonoBehaviour
{
    RecordManager recordManager;
    public string dataFileName;
    private string dataFilePath;

    void Start()
    {
        recordManager = RecordManager.recordManagerInstance;
        dataFilePath = Application.dataPath + "/Resources/" + dataFileName;
        LoadAudioAndBeats();
    }
    
    private void LoadAudioAndBeats()
    {
        //Setting up the data structures
        string[] audioInput = File.ReadAllLines(dataFilePath);
        string[] firstLineContent = audioInput[0].Split('\t');

        int numTracks = int.Parse(firstLineContent[0]);
        int numVersions = int.Parse(firstLineContent[1]);
        int numKeys = int.Parse(firstLineContent[2]);

        AudioClip[,] tracks = new AudioClip[numTracks, numVersions];

        List<int>[,,] beatLists = new List<int>[numTracks, numVersions, numKeys];

        for (int t = 0; t < numTracks; t++)
        {
            for (int v = 0; v < numVersions; v++)
            {
                for (int k = 0; k < numKeys; k++)
                {
                    beatLists[t, v, k] = new List<int>();
                }
            }
        }

        //Reading the data
        int line = 1;
        for(int t = 0; t < numTracks; t++)
        {
            for(int v = 0; v < numVersions; v++)
            {
                tracks[t, v] = Resources.Load<AudioClip>(audioInput[line]);
                line++;

                for(int k = 0; k < numKeys; k++)
                {
                    string[] beatList = audioInput[line].Split('\t');
                    line++;
                    if(beatList[0] != "")
                    {
                        for (int i = 0; i < beatList.Length; i++)
                        {
                            beatLists[t, v, k].Add(int.Parse(beatList[i]));
                        }
                    }
                }
            }
        }

        recordManager.LoadAudio(tracks);
        recordManager.LoadBeatData(beatLists);
    }
}
