using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab : MonoBehaviour
{

    public int travelBeats = 16;
    public Transform[] rowTrans;
    public Note notePrefab;
    public void spawnNote(GameManager.BeatAction beatAction)
    {
        Object.Instantiate(notePrefab, rowTrans[beatAction.onKey]);
    }
}
