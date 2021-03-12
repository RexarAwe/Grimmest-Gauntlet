// managers the game flow and all other components

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject penguinViking;
    [SerializeField] private GameObject skeleton;

    public int turn;
    [SerializeField] public List<GameObject> units; // units to consider for gameplay
    private int unitID = 0;
    [SerializeField] public List<GameObject> enemies; // enemies in the battle

    private MapManager mapManager;
    private CameraControl camControl;

    private GameObject StartCombatBT;
    private GameObject EndCombatBT;
    private GameObject EndTurnBT;

    private GameObject currentUnit;
    private UnitGeneral cGeneralUnit;
    private UnitMovement cMovementUnit;
    private UnitCombat cCombatUnit;

    private GameObject playerUnit;

    void Start()
    {
        mapManager = GameObject.Find("Map Manager").GetComponent<MapManager>();
        camControl = GameObject.Find("Main Camera").GetComponent<CameraControl>();

        StartCombatBT = GameObject.Find("Start Combat BT");
        EndCombatBT = GameObject.Find("End Combat BT");
        // EndTurnBT = GameObject.Find("End Turn BT");
        StartCombatBT.SetActive(true);
        EndCombatBT.SetActive(false);
        // EndTurnBT.SetActive(false);

        mapManager.Init();
        camControl.Init();

        camControl.SetCameraPos(7.9f, 4.8f);
        camControl.SetCameraSize(4.3f);
        camControl.SetMoveLock(true);
        camControl.SetZoomLock(true);

        SpawnFangedDeserter(mapManager.CellToWorld(new Vector3Int(3, 6, 0)));

        SpawnSkeleton(mapManager.CellToWorld(new Vector3Int(0, 3, 0)), 1);
        SpawnSkeleton(mapManager.CellToWorld(new Vector3Int(-2, 3, 0)), 1);

        playerUnit.GetComponent<UnitMovement>().Move();
    }

    void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            SwordAttack();
        }

        if (Input.GetKeyDown("space"))
        {
            EndTurn();
        }
    }

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // Debug.Log("PointerOverUIObject count: " + results.Count);

        return results.Count > 0;
    }

    public void InitBattle() // Initiate turn based rounds
    {
        mapManager.ClearHLTiles();

        // disallow movement of the unit before moving on
        playerUnit.GetComponent<UnitMovement>().SetMovable(false);
        playerUnit.GetComponent<UnitCombat>().SetAtkable(false);

        // compute each unit's initiative (agility + d6) and fill them up on action points
        for (int i = 0; i < units.Count(); i++)
        {
            units[i].GetComponent<UnitCombat>().RollIni();
            units[i].GetComponent<UnitGeneral>().RestoreActions();
            units[i].GetComponent<UnitGeneral>().SetOOC(false);
        }

        // order units on list based on init stat
        units = units.OrderByDescending(e => e.GetComponent<UnitCombat>().GetIni()).ToList();

        turn = 0;

        currentUnit = units[turn];
        cMovementUnit = currentUnit.GetComponent<UnitMovement>();
        cGeneralUnit = currentUnit.GetComponent<UnitGeneral>();
        cCombatUnit = currentUnit.GetComponent<UnitCombat>();

        PlayTurn();

        StartCombatBT.SetActive(false);

        // EndTurnBT.SetActive(true); 
    }

    public void PlayTurn()
    {
        Debug.Log("Play Unit " + cGeneralUnit.GetID() + "'s Turn");

        cGeneralUnit.Focus();
        cMovementUnit.Move();
        // cCombatUnit.Attack();

        // if player unit, display unit actions and display end turn

        // if npc unit, run ai
    }

    public void SwordAttack()
    {
        if(currentUnit != null)
        {
            if (cGeneralUnit.GetRemActions() > 0)
            {
                cCombatUnit.Attack();
            }
            else
            {
                Debug.Log("Out of Action Points.");
            }
        }
    }

    public void EndTurn()
    {
        if(currentUnit != null)
        {
            mapManager.ClearHLTiles();

            // disallow movement of the unit before moving on
            cMovementUnit.SetMovable(false);
            cCombatUnit.SetAtkable(false);

            // restore action points for next turn
            cGeneralUnit.RestoreActions();

            // remove any dead units
            //units = units.Where(unit => unit != null).ToList();
            ClearUnitList();

            // go to the next unit's turn
            if (turn >= units.Count - 1)
            {
                turn = 0;
            }
            else
            {
                turn++;
            }

            currentUnit = units[turn]; // keep track of the current turn's unit
            cMovementUnit = currentUnit.GetComponent<UnitMovement>();
            cGeneralUnit = currentUnit.GetComponent<UnitGeneral>();
            cCombatUnit = currentUnit.GetComponent<UnitCombat>();

            if (playerUnit == null)
            {
                Debug.Log("player unit is dead!");
                GameOver();
            }

            if (enemies.Count > 0)
            {
                PlayTurn(); // move on to next turn
            }
            else
            {
                Debug.Log("Battle Ended");
                EndBattle();
            }
        }
        else
        {
            Debug.Log("current unit is null");
        }
    }
        

    public void EndBattle() // shows up after combat is resolved
    {
        ClearUnitList(); // clear out any dead units

        playerUnit.GetComponent<UnitGeneral>().SetOOC(true);
        StartCombatBT.SetActive(true);
        EndCombatBT.SetActive(false);
        // EndTurnBT.SetActive(false);

        // back to moving normally
        playerUnit.GetComponent<UnitMovement>().Move();
    }

    private void SpawnFangedDeserter(Vector3 location)
    {
        UnitGeneral generalUnit;
        UnitMovement movementUnit;
        UnitCombat combatUnit;

        playerUnit = Instantiate(penguinViking, location, transform.rotation); // -4, -7

        movementUnit = playerUnit.GetComponent<UnitMovement>();
        movementUnit.Init(2);

        Debug.Log("Unit location: " + movementUnit.GetLoc());

        generalUnit = playerUnit.GetComponent<UnitGeneral>();
        generalUnit.Init(unitID, 0, 2, 1, 1, 1, 12); // stats should be rolled 3d6?
        generalUnit.InitHP(1, 10, movementUnit.GetWorldLoc()); // toughness + 1d10
        Debug.Log("FD HP: " + generalUnit.GetHP());

        combatUnit = playerUnit.GetComponent<UnitCombat>();
        combatUnit.Init();

        AddUnit(playerUnit);
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
        generalUnit.Init(unitID, 1, 2, 10, 1, 1, 6);
        generalUnit.InitHP(1, 4, movementUnit.GetWorldLoc());
        Debug.Log("Skeleton HP: " + generalUnit.GetHP());

        combatUnit = unit.GetComponent<UnitCombat>();
        combatUnit.Init();

        AddUnit(unit);
        AddEnemy(unit);
        unitID++;
    }

    public void AddUnit(GameObject unit) // add the unit to the unit tracking list
    {
        units.Add(unit);
    }

    public void AddEnemy(GameObject unit) // add the unit to the unit tracking list
    {
        enemies.Add(unit);
    }

    // return the unit occupying the location, if not occupied, print warning and return null
    public GameObject GetUnit(Vector3Int location)
    {
        // go through each unit to find whether any unit shares the same location
        for (int i = 0; i < units.Count; i++)
        {
            if(units[i] != null)
            {
                UnitMovement movementUnit = units[i].GetComponent<UnitMovement>();
                if (movementUnit.GetLoc() == location)
                {
                    return units[i];
                }
            }
        }

        return null;
    }

    public void ClearUnitList()
    {
        // remove any dead units
        units = units.Where(unit => unit != null).ToList();
        enemies = enemies.Where(unit => unit != null).ToList();
    }

    private void GameOver() // NOT DONE, present menu to load a save or quit?
    {
        Debug.Log("GAME OVER!");
    }
}
