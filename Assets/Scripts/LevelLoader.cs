using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    RecordManager recordManager;
    GameManager gameManager;
    EnemySpawner enemySpawner;
    public string dataFileName;
    private string dataFilePath;

    void Start()
    {
        recordManager = RecordManager.recordManagerInstance;
        gameManager = GameManager.instance;
        enemySpawner = gameManager.enemySpawner;
        dataFilePath = Application.dataPath + "/Resources/" + dataFileName;
        LoadLevel();
    }
    
    private void LoadLevel()
    {
        string[] input = File.ReadAllLines(dataFilePath);
        int line = 0;
        line = LoadAudioAndBeats(input, line);
        line++;
        line = LoadMap(input, line);
        line++;
        LoadEnemies(input, line);
    }
    private int LoadAudioAndBeats(string[] input, int startLine)
    {
        //Setting up the data structures
        string[] firstLineContent = input[startLine].Split('\t');

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
        int line = startLine + 1;
        for(int t = 0; t < numTracks; t++)
        {
            for(int v = 0; v < numVersions; v++)
            {
                tracks[t, v] = Resources.Load<AudioClip>(input[line]);
                line++;

                for(int k = 0; k < numKeys; k++)
                {
                    string[] beatList = input[line].Split('\t');
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

        return line;
    }

    private int LoadMap(string[] input, int startLine)
    {
        int line = startLine;
        int size = int.Parse(input[startLine]);
        line++;

        GameManager.Contents[,] map = new GameManager.Contents[size, size];
        for(int x = 0; x < size; x++)
        {
            for(int z = 0; z < size; z++)
            {
                map[x, z] = GameManager.Contents.ground;
            }
        }

        for (int x = 0; x < size; x++)
        {
            string[] rowList = input[line].Split('\t');
            for(int i = 0; i < rowList.Length; i++)
            {
                map[x, int.Parse(rowList[i])] = GameManager.Contents.air;
            }
            line++;
        }

        string[] lastLineContents = input[line].Split('\t');
        int xStart = int.Parse(lastLineContents[0]);
        int zStart = int.Parse(lastLineContents[1]);
        line++;

        gameManager.LoadMapData(map, xStart, zStart);

        return line;
    }

    private int LoadEnemies(string[] input, int startLine)
    {
        int line = startLine;
        Queue<EnemySpawner.enemySpawn> enemies = new Queue<EnemySpawner.enemySpawn>();

        while(input[line] != "")
        {
            string[] thisSpawnStrings = input[line].Split('\t');
            EnemySpawner.enemySpawn thisSpawn = new EnemySpawner.enemySpawn(int.Parse(thisSpawnStrings[1]), float.Parse(thisSpawnStrings[0]));

            enemies.Enqueue(thisSpawn);
            line++;
        }

        enemySpawner.LoadEnemyData(enemies);
        enemySpawner.RunSpawner();

        return line;
    }
}
