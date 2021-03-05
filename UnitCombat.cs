using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitCombat : MonoBehaviour
{
    private int str;
    private int pre;
    private int agi;

    private int rng;

    private MapManager mapManager;
    private GameManager gameManager;
    private UnitGeneral generalUnit;
    private UnitMovement movementUnit;

    Tilemap tilemap;

    [SerializeField] private bool atkAble = false;

    public void Init()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        mapManager = GameObject.Find("Map Manager").GetComponent<MapManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        generalUnit = gameObject.GetComponent<UnitGeneral>();
        movementUnit = gameObject.GetComponent<UnitMovement>();

        UpdateStats();

        rng = 1; // temporary, weapon type usually determines attack range
    }

    public void UpdateStats()
    {
        str = generalUnit.GetStr();
        pre = generalUnit.GetPre();
        agi = generalUnit.GetAgi();
    }

    private void Update()
    {
        // onclick, if tile is atkable, perform an attack on the target
        if (Input.GetMouseButtonDown(0))
        {
            if (atkAble)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int tileCoordinate = tilemap.WorldToCell(mouseWorldPos);

                if (mapManager.GetAtkableStatus(tileCoordinate))
                {
                    // perform attack and defend rolls
                    if (gameManager.GetUnit(tileCoordinate))
                    {
                        //int targetID = gameManager.GetUnit(tileCoordinate).GetComponent<UnitGeneral>().GetID();
                        //Debug.Log("Target ID: " + targetID);

                        RollAtk(gameManager.GetUnit(tileCoordinate));
                        atkAble = false;

                        generalUnit.SetRemActions(generalUnit.GetRemActions() - 1);

                        mapManager.ClearHLTiles();

                        if (generalUnit.GetOOC() || generalUnit.GetRemActions() > 0)
                        {
                            gameObject.GetComponent<UnitMovement>().Move();
                            Attack();
                        }

                        gameManager.ClearUnitList(); // needed here?
                    }
                }
            }
        }
    }

    // Given a location, indicate all enemies on tiles that can be attacked
    public void Attack()
    {
        UnitGeneral generalUnit = gameObject.GetComponent<UnitGeneral>();
        mapManager.CheckAttack(movementUnit.GetLoc(), rng, generalUnit.GetAllegiance());
        atkAble = true;
    }

    public void RollAtk(GameObject target) // NOT DONE, return int based on result?
    {
        // strength roll DR12 (d20 + str >= 12)
        int roll = Random.Range(1, 21); // roll d20 + str
        int total = roll + str;

        if (roll == 20) // double damage, reduce  armor by 1 tier
        {
            Debug.Log("CRITICAL HIT");
            InflictDmg(target, RollDmg(1, 6, 0), true);
        }
        else if (roll == 1) // weapon breaks and misses
        {
            Debug.Log("CRITICAL MISS");
        }
        else if (total >= 12) // hit
        {
            Debug.Log("HIT");
            InflictDmg(target, RollDmg(1, 6, 0), true);

        }
        else // miss
        {
            Debug.Log("MISS");
        }
    }

    // roll dmg e.g. 1d6+2, 1 is num, dice is 6, mod is 2
    private int RollDmg(int num, int dice, int mod)
    {
        int total = 0;

        for (int i = 0; i < num; i++)
        {
            total += Random.Range(1, dice + 1);
        }

        return total += mod;
    }

    private void InflictDmg(GameObject target, int dmg, bool crit)
    {
        UnitGeneral generalUnit = target.GetComponent<UnitGeneral>();

        if (crit)
        {
            dmg = dmg * 2;
        }

        generalUnit.TakeDmg(dmg);

        Debug.Log("Inflicted " + dmg + " to target.");
    }
}
