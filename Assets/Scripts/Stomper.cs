using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomper : Tower
{
    private List<Enemy> inRange;
    private float closedPos;
    [SerializeField] private float openDist;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float openSpeed;
    [SerializeField] private float closeSpeed;
    private Transform topTransform;
    private bool closing = false;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Transform ring;

    protected override void Start()
    {
        base.Start();
        
        if(model is StomperData stompModel)
        {
            topTransform = stompModel.top;
        }

        inRange = new List<Enemy>();
        closedPos = topTransform.localPosition.y;
    }

    private void Update()
    {
        topTransform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
        if (closing)
        {
            topTransform.localPosition -= Vector3.up * closeSpeed * Time.deltaTime;
            if(topTransform.localPosition.y <= closedPos)
            {
                topTransform.localPosition = Vector3.up * closedPos;
                closing = false;
            }
        } 
        else if(topTransform.localPosition.y < closedPos + openDist)
        {
            topTransform.localPosition += Vector3.up * openSpeed * Time.deltaTime;
        } 
        else if (topTransform.localPosition.y > closedPos + openDist)
        {
            topTransform.localPosition = Vector3.up * (closedPos + openDist);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy != null)
        {
            inRange.Add(enemy);
        }
    }

    public override float Upgrade()
    {
        base.Upgrade();
        
        if(model is StomperData stompData)
        {
            topTransform = stompData.top;
        }

        switch (level)
        {
            case 1:
                ring.localScale = 1.25f * Vector3.one;
                particles.transform.localScale = 1.25f * Vector3.one;
                range = 1.42f;
                return 1.42f;
            case 2:
                damage = 2;
                range = 1.60f;
                return 1.60f;
            case 3:
                ring.localScale = 1.5f * Vector3.one;
                particles.transform.localScale = 1.5f * Vector3.one;
                range = 2.67f;
                return 2.67f;
        }

        return 0;
    }

    public override float NextUpgradeRange()
    {
        base.NextUpgradeRange();

        switch (level)
        {
            case 0:
                return 1.42f;
            case 1:
                return 1.60f;
            case 2: 
                return 2.67f;
        }
        return 0;
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy != null)
        {
            inRange.Remove(enemy);
        }
    }

    protected override void Attack(BeatManager.BeatAction beatAction)
    {
        base.Attack(beatAction);

        for(int i = inRange.Count - 1; i >= 0; i--)
        {
            if(inRange[i] == null)
            {
                inRange.RemoveAt(i);
            }
            else
            {
                if(!inRange[i].Damage(damage, transform.position))
                {
                    inRange.RemoveAt(i);
                }
            }
        }

        particles.Play();

        closing = true;
    }
}
