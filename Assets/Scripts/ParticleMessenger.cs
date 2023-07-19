using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMessenger : MonoBehaviour
{
    public float dmg = .1f;
    private ParticleSystem particles;

    private void Start()
    {
        
        particles = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("hit");
    }
}
