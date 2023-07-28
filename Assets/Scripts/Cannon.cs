using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Targeter
{
    private ParticleSystem blasts;
    private ParticleSystem.EmissionModule emission;

    protected override void Start()
    {
        base.Start();
        blasts = GetComponentInChildren<ParticleSystem>();
        emission = blasts.emission;
    }

    protected virtual void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    protected override void Attack(BeatManager.BeatAction beatAction)
    {
        base.Attack(beatAction);
        if(target != null)
        {
            blasts.Play();
        }
    }

    public override float Upgrade()
    {
        base.Upgrade();

        switch (level)
        {
            case 1:
                ParticleSystem.Burst burst = emission.GetBurst(0);
                burst.cycleCount = 4;
                emission.SetBurst(0, burst);
                break;
            case 2:
                damage *= 2;
                break;
            case 3:
                ParticleSystem.Burst burst2 = emission.GetBurst(0);
                burst2.cycleCount = 8;
                emission.SetBurst(0, burst2);
                break;
        }

        return 0;
    }
}
