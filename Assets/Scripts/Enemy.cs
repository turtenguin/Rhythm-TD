using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{

    GameManager gameManager;
    EnemySpawner enemySpawner;
    ShopManager shopManager;

    //+x, -x, +z, -z
    private int dir;
    public int trackOn { get; private set; } = 0;
    public float speed { get; private set; }
    private Vector3 moveVec;
    public int strength { get; private set; }
    public float health { get; private set;}
    public ParticleSystem particles;
    public ParticleSystem explodeParticles;
    public float explodeSpeed = 5;
    public GameObject model;
    private bool alive = true;

    //Setter values which do not change
    [SerializeField] private float startSpeed;
    [SerializeField] private int StartStrength;
    [SerializeField] private float StartHealth;
    public int type;

    void Start()
    {
        gameManager = GameManager.instance;
        enemySpawner = gameManager.enemySpawner;
        shopManager = ShopManager.instance;

        ResetEnemy();
    }

    public void ResetEnemy()
    {
        alive = true;
        trackOn = 0;

        model.SetActive(true);
        particles.gameObject.SetActive(false);
        explodeParticles.gameObject.SetActive(false);

        speed = startSpeed;
        health = StartHealth;
        strength = StartStrength;

        gameManager.enemies.Add(this);
        dir = gameManager.trackDirs[0];
        switch (dir)
        {
            case 0:
                moveVec = new Vector3(speed * Time.fixedDeltaTime, 0, 0);
                break;
            case 1:
                moveVec = new Vector3(-speed * Time.fixedDeltaTime, 0, 0);
                break;
            case 2:
                moveVec = new Vector3(0, 0, speed * Time.fixedDeltaTime);
                break;
            case 3:
                moveVec = new Vector3(0, 0, -speed * Time.fixedDeltaTime);
                break;
        }
        transform.position = gameManager.trackStart;
    }

    void FixedUpdate()
    {
        if (alive)
        {
            transform.Translate(moveVec);
            updateMove();
        }
    }

    void updateMove()
    {
        switch(dir)
        {
            case 0:
                if (transform.position.x > gameManager.trackEnds[trackOn]) turn();
                break;
            case 1:
                if (transform.position.x < gameManager.trackEnds[trackOn]) turn();
                break;
            case 2:
                if (transform.position.z > gameManager.trackEnds[trackOn]) turn();
                break;
            case 3:
                if (transform.position.z < gameManager.trackEnds[trackOn]) turn();
                break;
            default:
                Debug.Log("Invalid Direction");
                break;
        }
    }

    void turn()
    {
        trackOn++;
        dir = gameManager.trackDirs[trackOn];
        switch (dir)
        {
            case 0:
                moveVec = new Vector3(speed * Time.fixedDeltaTime, 0, 0);
                transform.position.Set(transform.position.x, transform.position.y, gameManager.trackEnds[trackOn - 1]);
                break;
            case 1:
                moveVec = new Vector3(-speed * Time.fixedDeltaTime, 0, 0);
                transform.position.Set(transform.position.x, transform.position.y, gameManager.trackEnds[trackOn - 1]);
                break;
            case 2:
                moveVec = new Vector3(0, 0, speed * Time.fixedDeltaTime);
                transform.position.Set(gameManager.trackEnds[trackOn - 1], transform.position.y, transform.position.z);
                break;
            case 3:
                moveVec = new Vector3(0, 0, -speed * Time.fixedDeltaTime);
                transform.position.Set(gameManager.trackEnds[trackOn - 1], transform.position.y, transform.position.z);
                break;
            default:
                ReachEnd();
                break;
        }
    }

    public bool IsAheadOf(Enemy enemy)
    {
        if (enemy.trackOn > trackOn) return false;
        if (enemy.trackOn < trackOn) return true;
        switch (dir)
        {
            case 0:
                return transform.position.x > enemy.transform.position.x;
            case 1:
                return transform.position.x < enemy.transform.position.x;
            case 2:
                return transform.position.z > enemy.transform.position.z;
            case 3:
                return transform.position.z < enemy.transform.position.z;
            default:
                Debug.Log("Invalid Directino");
                return true;
        }
    }

    public bool Damage(float dmg, Vector3 towerPos)
    {
        if (alive)
        {
            health -= dmg;
            if(health <= 0)
            {
                Die(towerPos);
                return false;
            }
            return true;
        }
        return false;
    }

    private void ReachEnd()
    {
        alive = false;
        model.SetActive(false);
        explodeParticles.gameObject.SetActive(true);

        explodeParticles.Play();
        gameManager.enemies.Remove(this);
        gameManager.Damage(strength);
        Invoke("Destroy", 2);
    }

    private void Die(Vector3 towerPos)
    {
        alive = false;
        model.SetActive(false);
        particles.gameObject.SetActive(true);

        Vector3 explodeDir = Vector3.Normalize(transform.position - towerPos) * explodeSpeed;
        ParticleSystem.VelocityOverLifetimeModule vModule = particles.velocityOverLifetime;
        vModule.x = new ParticleSystem.MinMaxCurve(explodeDir.x, explodeDir.x);
        vModule.z = new ParticleSystem.MinMaxCurve(explodeDir.z, explodeDir.z);

        particles.Play();

        gameManager.enemies.Remove(this);

        shopManager.CollectBounty(type);

        if(type == 10)
        {
            Invoke("Win", 5);
        }
        else
        {
            Invoke("Destroy", 2);
        }
    }

    private void Win()
    {
        SceneManager.LoadScene("Win Menu");
    }

    private void Destroy()
    {
        enemySpawner.AddToPool(this);
        this.gameObject.SetActive(false);
    }
}
