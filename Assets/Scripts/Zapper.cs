using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zapper : Shocker
{
    protected override void Update()
    {
        base.Update();
        if (target != null)
        {
            transform.LookAt(target.transform.position);
        }
    }
}
