using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMessenger : MonoBehaviour
{
    public Tower tower;
    private ParticleSystem particles;
    private float dmg;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        dmg = tower.damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        Enemy hitEnemy = other.GetComponent<Enemy>();
        if(hitEnemy != null)
        {
            List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();
            int count = ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisions);
            hitEnemy.Damage(dmg * count, transform.position);
            Debug.Log(count);
        } else
        {
            Debug.Log("Miss");
        }


    }
}
