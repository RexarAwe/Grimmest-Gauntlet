// managers the game flow and all other components

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject penguinViking;
    [SerializeField] private GameObject skeleton;

    public int turn;
    [SerializeField] public List<GameObject> units;
    private int unitID = 0;

    private MapManager mapManager;
    private GameObject StartCombatBT;
    private GameObject EndCombatBT;

    void Start()
    {
        mapManager = GameObject.Find("Map Manager").GetComponent<MapManager>();
        StartCombatBT = GameObject.Find("Start Combat BT");
        EndCombatBT = GameObject.Find("End Combat BT");
        StartCombatBT.SetActive(false);
        EndCombatBT.SetActive(false);

        mapManager.Init();
        SpawnFangedDeserter();

        SpawnSkeleton(new Vector3(3f, 0, 0), 1);

        units[0].GetComponent<UnitMovement>().Move();
        units[0].GetComponent<UnitCombat>().Attack();
    }

    public void InitBattle()
    {
        // roll to see initiative
    }

    public void EndBattle()
    {
        units[0].GetComponent<UnitGeneral>().SetOOC(true);
        StartCombatBT.SetActive(false);
        EndCombatBT.SetActive(false);

        units[0].GetComponent<UnitMovement>().Move();
    }

    private void SpawnFangedDeserter()
    {
        GameObject unit;
        UnitGeneral generalUnit;
        UnitMovement movementUnit;
        UnitCombat combatUnit;

        unit = Instantiate(penguinViking, new Vector3(0, 0, 0), transform.rotation);

        movementUnit = unit.GetComponent<UnitMovement>();
        movementUnit.Init(2);

        Debug.Log("Unit location: " + movementUnit.GetLoc());

        generalUnit = unit.GetComponent<UnitGeneral>();
        generalUnit.Init(unitID, 0, 2, 1, 1, 1, 12); // stats should be rolled 3d6?
        generalUnit.InitHP(1, 10, movementUnit.GetWorldLoc()); // toughness + 1d10
        Debug.Log("FD HP: " + generalUnit.GetHP());

        combatUnit = unit.GetComponent<UnitCombat>();
        combatUnit.Init();

        AddUnit(unit);
        unitID++;
    }

    private void SpawnSkeleton(Vector3 location, int allegiance)
    {
        GameObject unit;
        UnitGeneral generalUnit;
        UnitMovement movementUnit;
        UnitCombat combatUnit;

        unit = Instantiate(skeleton, location, transform.rotation);

        movementUnit = unit.GetComponent<UnitMovement>();
        movementUnit.Init(2);

        Debug.Log("Unit location: " + movementUnit.GetLoc());

        generalUnit = unit.GetComponent<UnitGeneral>();
        generalUnit.Init(unitID, 1, 2, 1, 1, 1, 6);
        generalUnit.InitHP(1, 4, movementUnit.GetWorldLoc());
        Debug.Log("Skeleton HP: " + generalUnit.GetHP());

        combatUnit = unit.GetComponent<UnitCombat>();
        combatUnit.Init();

        AddUnit(unit);
        unitID++;
    }

    public void AddUnit(GameObject unit) // add the unit to the unit tracking list
    {
        units.Add(unit);
    }

    public void CombatTrigger()
    {
        units[0].GetComponent<UnitGeneral>().SetOOC(false);
        StartCombatBT.SetActive(true);
        EndCombatBT.SetActive(true);
    }

    // return the unit occupying the location, if not occupied, print warning and return null
    public GameObject GetUnit(Vector3Int location)
    {
        // go through each unit to find whether any unit shares the same location
        for (int i = 0; i < units.Count; i++)
        {
            UnitMovement movementUnit = units[i].GetComponent<UnitMovement>();
            if (movementUnit.GetLoc() == location)
            {
                return units[i];
            }
        }

        return null;
    }

    public void ClearUnitList()
    {
        // remove any dead units
        units = units.Where(unit => unit != null).ToList();
    }
}
