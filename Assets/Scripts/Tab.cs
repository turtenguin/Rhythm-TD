using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab : MonoBehaviour
{
    public float noteOffset = 5;
    public int travelBeats = 8;
    public Transform[] rowTrans;
    public Note notePrefab;
    public void SpawnNote(BeatManager.BeatAction beatAction)
    {
        Note note = Object.Instantiate(notePrefab, rowTrans[beatAction.onKey]);
        note.beatAction = beatAction;
    }
}
