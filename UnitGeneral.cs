// Unit stats, status effects, hp, and exp

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGeneral : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private int allegiance; // friend or enemy
    [SerializeField] private int hp;
    [SerializeField] private int remHp;
    [SerializeField] private int actions;
    [SerializeField] private int remActions;
    [SerializeField] private int status; // conditions

    [SerializeField] private int agility;
    [SerializeField] private int presence;
    [SerializeField] private int strength;
    [SerializeField] private int toughness;

    [SerializeField] private bool OOC; // out of combat

    UnitMovement movementUnit;

    [SerializeField] private GameObject hpBar; // prefab storage
    private GameObject healthBar; // actual reference
    private HpBar hpBarScript;
    private bool initiated = false;

    public void Init(int id_val, int allegiance_val, int actions_val, int agi_val, int pre_val, int str_val, int tou_val)
    {
        id = id_val;
        allegiance = allegiance_val;
        actions = actions_val;
        remActions = actions_val;
        OOC = true;

        agility = agi_val;
        presence = pre_val;
        strength = str_val;
        toughness = tou_val;

        movementUnit = gameObject.GetComponent<UnitMovement>();
    }

    public void InitHP(int dNum, int dType, Vector3 location)
    {
        // toughness + dice roll
        hp = toughness;

        for (int i = 0; i < dNum; i++)
        {
            hp += Random.Range(1, dType + 1);
        }

        remHp = hp;

        // create hp bar at correct position
        Vector3 worldPos = location;
        worldPos.y -= 0.7f;
        healthBar = Instantiate(hpBar, Camera.main.WorldToScreenPoint(worldPos), transform.rotation);
        hpBarScript = healthBar.GetComponent<HpBar>();
        hpBarScript.Init();

        GameObject canvas = GameObject.Find("Canvas");
        healthBar.transform.SetParent(canvas.transform);

        hpBarScript.SetMaxHp(hp);
        hpBarScript.SetValue(remHp);

        initiated = true;
    }

    void Update()
    {
        if (initiated)
        {
            Vector3 worldPos = movementUnit.GetWorldLoc();
            worldPos.y -= 0.7f;
            healthBar.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }
    }

    // focus camera on this unit
    public void Focus()
    {
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Camera.main.transform.position.z);
    }

    // adjust hp based on dmg received, kill unit if hp reaches 0
    public void TakeDmg(int dmg)
    {
        remHp -= dmg;
        hpBarScript.SetValue(remHp);

        if (remHp <= 0)
        {
            Debug.Log("Dead");
            Death();
        }
    }

    // the unit is dead
    public void Death()
    {
        // set its occupancy to false
        UnitMovement movementUnit = gameObject.GetComponent<UnitMovement>();
        movementUnit.SetOcc(false);

        SetStatus(-1);

        // remove unit from lists
        Destroy(healthBar);
        Destroy(gameObject);
    }

    public int GetHP()
    {
        return hp;
    }

    public int GetID()
    {
        return id;
    }

    public int GetStatus()
    {
        return status;
    }

    public void SetStatus(int val)
    {
        status = val;
    }

    public int GetActions()
    {
        return actions;
    }

    public int GetAllegiance()
    {
        return allegiance;
    }

    public void SetActions(int actions_val)
    {
        actions = actions_val;
    }

    public int GetRemActions()
    {
        return remActions;
    }

    public void SetRemActions(int act_val)
    {
        remActions = act_val;
    }

    public void RestoreActions()
    {
        remActions = actions;
    }

    public bool GetOOC()
    {
        return OOC;
    }

    public void SetOOC(bool val)
    {
        OOC = val;
    }

    public int GetAgi()
    {
        return agility;
    }

    public int GetPre()
    {
        return presence;
    }

    public int GetStr()
    {
        return strength;
    }

    public int GetTou()
    {
        return toughness;
    }
}
