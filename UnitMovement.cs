using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitMovement : MonoBehaviour
{
    private Vector3Int location; //get closest tile coordinate, or just set default to tilemap origin
    [SerializeField] private int spd; // determines movement range
    [SerializeField] protected bool movable = false;

    private MapManager mapManager;
    private GameManager gameManager;
    private UnitGeneral generalUnit;

    Tilemap tilemap;

    public void Init(int spd_val)
    {
        mapManager = GameObject.Find("Map Manager").GetComponent<MapManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        generalUnit = gameObject.GetComponent<UnitGeneral>();

        location = tilemap.WorldToCell(transform.position);
        spd = spd_val;

        // set occupancy state at unit location in mapManager to true
        mapManager.SetOccupancy(true, location);
        Debug.Log("Set occupancy at " + location);
    }

    void Update()
    {
        // onclick, if tile is movable, moves unit there and updates location
        if (Input.GetMouseButtonDown(0) && !gameManager.IsPointerOverUIObject())
        {
            if (movable)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int tileCoordinate = tilemap.WorldToCell(mouseWorldPos);

                if (mapManager.GetMovableStatus(tileCoordinate))
                {
                    mapManager.SetOccupancy(false, location);

                    // move the unit there // need to add animation
                    location = tileCoordinate;
                    transform.position = tilemap.CellToWorld(location);

                    // change occupancy of tiles
                    mapManager.SetOccupancy(true, location);

                    movable = false;

                    mapManager.ClearHLTiles();

                    // decrease action point by 1 if not OOC (in combat)
                    if(!generalUnit.GetOOC())
                    {
                        generalUnit.SetRemActions(generalUnit.GetRemActions() - 1);
                    }

                    // Debug.Log("OOC status: " + generalUnit.GetOOC());

                    if(generalUnit.GetOOC() || generalUnit.GetRemActions() > 0)
                    {
                        Move();
                        gameObject.GetComponent<UnitCombat>().Attack();
                    }

                    // gameManager.ClearUnitList(); // needed here?
                }
            }
        }
    }

    // allows movement for the unit
    public void Move()
    {
        // "highlight" movement range
        mapManager.CheckMovement(location, spd);

        // allows movement for this unit
        movable = true;
    }

    public Vector3Int GetLoc() // cell location
    {
        return location;
    }

    public Vector3 GetWorldLoc()
    {
        return tilemap.CellToWorld(location);
    }

    // set occupancy value at unit location
    public void SetOcc(bool val)
    {
        mapManager.SetOccupancy(val, location);
    }

    public void SetMovable(bool val)
    {
        movable = val;
    }
}
