using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEventMask : MonoBehaviour
{
    [SerializeField] private LayerMask eventMask;

    void Start()
    {
        Camera.main.eventMask = eventMask;
    }
}
