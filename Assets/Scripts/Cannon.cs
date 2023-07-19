using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Targeter
{
    [SerializeField] private Transform aim;
    private ParticleSystem blasts;

    protected override void Start()
    {
        base.Start();
        blasts = GetComponentInChildren<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            aim.position = target.transform.position;
        }
    }

    protected override void Attack(BeatManager.BeatAction beatAction)
    {
        base.Attack(beatAction);
        blasts.Play();
    }
}
