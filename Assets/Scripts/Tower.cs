using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tower : MonoBehaviour
{
    protected GameManager gameManager;
    protected RecordManager recordManager;
    [HideInInspector] public int track;
    [HideInInspector] public int key;
    public int towerType;
    public int level { get; private set; }
    public float range = 5;
    public Action<BeatManager.BeatAction> attackAction { get; private set; }
    private Material normMat;
    public Material flashMat;
    private List<Renderer> flashParts;
    public float flashTime = .1f;

    public ModelData model;

    public string[] upgradeText;
    public ModelData[] modelPrefabs;

    private UpgradeMenu upgradeMenu;
    public TowerDrag towerDrag { get; private set; }
    
    //0 = First, 1 = Last, 2 = Strong, 3 = Close
    public int targeting = 0;
    public float damage = 1;
    protected virtual void Start()
    {
        gameManager = GameManager.instance;
        recordManager = RecordManager.recordManagerInstance;
        towerDrag = GetComponent<TowerDrag>();

        flashParts = model.flashParts;

        upgradeMenu = gameManager.upgradeMenu;

        normMat = flashParts[0].material;

        level = 0;

        transform.localScale = Vector3.one * .7f;

        registerTower();
    }

    void registerTower()
    {
        attackAction = Attack;
        recordManager.RegisterTower(this);
        gameManager.RegisterTowerPosition(transform.position);
    }

    void UnFlash()
    {
        foreach(Renderer part in flashParts)
        {
            part.material = normMat;
        }
    }

    private void OnMouseUp()
    {
        if (!upgradeMenu.IsButtonPressed())
        {
            upgradeMenu.initMenu(this);
        }
    }

    protected virtual void Attack(BeatManager.BeatAction beatAction)
    {
        foreach(Renderer part in flashParts)
        {
            part.material = flashMat;
        }

        Invoke("UnFlash", flashTime);
    }

    public virtual float Upgrade()
    {
        level++;
        ModelData newModel = UnityEngine.Object.Instantiate(modelPrefabs[level - 1], transform);
        newModel.transform.position = model.transform.position;
        newModel.transform.rotation = model.transform.rotation;
        GameObject.Destroy(model.gameObject);
        model = newModel;
        flashParts = model.flashParts;
        transform.localScale = Vector3.one * (.7f + .1f * level);

        return 0;
    }

    public virtual float NextUpgradeRange()
    {
        return 0;
    }
}
