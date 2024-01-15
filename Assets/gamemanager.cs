using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;
using UnityEngine.UI;
using TMPro;
using EventSystem = UnityEngine.EventSystems.EventSystem;

public class gamemanager : MonoBehaviour
{
    public GameObject growingLemonTreePrefab;
    public GameObject growingBlueTreePrefab;
    public GameObject growingDeathTreePrefab;

    public GameObject gardenTilePrefab;

    public Slider slider;
    public TextMeshProUGUI clock;
    public TMP_Dropdown seedSelector;
    public TextMeshProUGUI seedCounts;
    public Button sellLemonSeed;
    public Button buyLemonSeed;
    public TextMeshProUGUI money;

    public EventSystem eventSystem;

    public int lemonSeeds = 10;
    public int blueSeeds = 5;
    public int deathSeeds = 1;

    public float gridToWorldScaleFactor = 4.0f;
    public static int GRID_SIZE = 16;

    public float daysPerTimeUnit = 1.0f;

    public int dollars = 10;

    public enum TileState
    {
        Unavailable,
        Available,
        InUse,
    };

    private Dictionary<Vector2Int, GameObject> tileObjects = new Dictionary<Vector2Int, GameObject>();


    public float currentTime = 0; // units: TIME
                                  // Start is called before the first frame update
    void Start()
    {
        currentTime = 0; //Time.time;

        initGrid();

        sellLemonSeed.onClick.AddListener(sellLemonSeedOnClick);
        buyLemonSeed.onClick.AddListener(buyLemonSeedOnClick);

        var canvas = GameObject.Find("Canvas");
        eventSystem = canvas.GetComponent<EventSystem>();
    }

    public void pickUpSeed(growingtree.TreeType treeType, int count, Vector3 point)
    {
        switch (treeType)
        {
            case growingtree.TreeType.Lemon:
                lemonSeeds += count;
                break;
            case growingtree.TreeType.Blue:
                blueSeeds += count;
                break;
            case growingtree.TreeType.Death:
                deathSeeds += count;
                break;
        }
        markGridLocation(point, false);
    }

    public void markGridLocation(Vector3 point, bool used)
    {
        Vector2Int gridLocation = getGridLocation(point);
        var tile = tileObjects[gridLocation].GetComponent<gardentile>();
        if (tile.tileState == TileState.Unavailable)
        {
            Debug.Log("Tried to mark unavailable grid location as used");
            return;
        }
        tile.updateTileState(used ? TileState.InUse : TileState.Available);
    }

    private Vector2Int getGridLocation(Vector3 position)
    {
        int x = (int)Math.Round((position.x) / gridToWorldScaleFactor);
        int z = (int)Math.Round((position.z) / gridToWorldScaleFactor);
        // convert to all positive coordinates
        return new Vector2Int(x + GRID_SIZE / 2, z + GRID_SIZE / 2);
    }

    private Vector3 getPositionForGridLocation(Vector2Int gridLocation)
    {
        return new Vector3((gridLocation.x - GRID_SIZE / 2) * gridToWorldScaleFactor, 0, (gridLocation.y - GRID_SIZE / 2) * gridToWorldScaleFactor);
    }

    void spawnTreeOnClick()
    {

        // Spawn a cube when mouse is clicked, where the mouse intersects with the plane
        if (!Input.GetMouseButtonDown(0) || eventSystem.IsPointerOverGameObject())
        {
            return;
        }

        // Disallow planting trees while time is reversing
        if (slider.value < 0)
        {
            Debug.Log("Can't plant tree while time reversing");
            return;
        }

        // Create a ray from the camera to the mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Create a plane at y=0
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        // Find the point where the ray intersects the plane
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            // Instantiate a lemon tree prefab at the intersection point
            Vector3 candidatePoint = ray.GetPoint(distance);
            Vector2Int gridLocation = getGridLocation(candidatePoint);

            // Log
            // Debug.Log("Grid location: " + gridLocation.x + ", " + gridLocation.y);
            // Debug.Log("Candidate point: " + candidatePoint.x + ", " + candidatePoint.z);
            if (gridLocation.x < 0 || gridLocation.x >= GRID_SIZE || gridLocation.y >= GRID_SIZE || gridLocation.y < 0) return;
            var tile = tileObjects[gridLocation].GetComponent<gardentile>();
            if (tile.tileState != TileState.Available)
            {
                // Debug.Log("Tried to plant tree on unavailable grid location");
                return;
            }

            // Simple version of preventing trees from being planted on top of each other due to time travel:
            // dont' allow planting a tree in the past on a square which will have a tree on it in the future
            foreach (KeyValuePair<float, GameObject> kvp in tile.treesByBirthTime)
            {
                if (kvp.Key >= currentTime)
                {
                    Debug.Log("Tried to plant tree in the past on a square which will have a tree on it in the future");
                    return;
                }
            }
            // A better version of this might involve allowing a tree to be planted as long as it will be dead by the time
            // the future tree is planted, and auto-harvesting the seed when the future tree is planted.


            GameObject prefabToInstantiate;
            switch (seedSelector.value)
            {
                case 0:
                    if (lemonSeeds <= 0) return;
                    prefabToInstantiate = growingLemonTreePrefab;
                    lemonSeeds -= 1;
                    break;
                case 1:
                    if (blueSeeds <= 0) return;
                    prefabToInstantiate = growingBlueTreePrefab;
                    blueSeeds -= 1;
                    break;
                case 2:
                default:
                    if (deathSeeds <= 0) return;
                    prefabToInstantiate = growingDeathTreePrefab;
                    deathSeeds -= 1;
                    break;
            }

            // Randomly rotate around x axis
            float rotation = UnityEngine.Random.Range(0, 360);
            Quaternion rotationQuaternion = Quaternion.Euler(rotation, 0, 0);

            Vector3 finalPoint = getPositionForGridLocation(gridLocation);
            GameObject tree = Instantiate(prefabToInstantiate, finalPoint, rotationQuaternion);
            var gardentile = tileObjects[gridLocation].GetComponent<gardentile>();
            gardentile.updateTileState(TileState.InUse);
            gardentile.treesByBirthTime[currentTime] = tree;

        }
    }

    private void initGrid()
    {
        // Create tiles for all allowed grid locations
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                Vector2Int gridLocation = new Vector2Int(x, y);
                Vector3 position = getPositionForGridLocation(gridLocation);
                var tile = Instantiate(gardenTilePrefab, position, Quaternion.identity);
                tileObjects[gridLocation] = tile;

                // Mark the border as Unavailable, for testing out this functionality. My idea is that we can use this sort of tile state
                // to implement buying/unlocking more land as the game progresses.
                if (x != 0 && x != GRID_SIZE - 1 && y != 0 && y != GRID_SIZE - 1)
                {
                    tile.GetComponent<gardentile>().updateTileState(TileState.Available);
                }
                else
                {
                    tile.GetComponent<gardentile>().updateTileState(TileState.Unavailable);
                }
            }
        }
    }


    private void updateUI()
    {
        // Update current time
        currentTime = currentTime + (Time.deltaTime) * slider.value;
        float daysToDisplay = currentTime * (daysPerTimeUnit);
        // convert floating point days to hours, minutes, seconds
        int daysInt = (int)Math.Floor(daysToDisplay);
        float hoursToDisplay = (daysToDisplay - daysInt) * 24;
        int hours = (int)Math.Floor(hoursToDisplay);
        //float minutesToDisplay = (hoursToDisplay - hours) * 60;
        //int minutes = (int)Math.Floor(minutesToDisplay);
        clock.text = daysInt + " days, " + hours + " hours";

        seedCounts.text = "Lemon Seeds: " + lemonSeeds + "\nBlue Seeds: " + blueSeeds + "\nDeath Seeds: " + deathSeeds;

        money.text = "Money: $" + dollars;
    }

    public void sellLemonSeedOnClick()
    {
        lemonSeeds -= 1;
        dollars += 1;
    }
    static int LEMON_SEED_COST = 2;
    public void buyLemonSeedOnClick()
    {
        if (dollars - LEMON_SEED_COST < 0) return;
        dollars -= LEMON_SEED_COST;
        lemonSeeds += 1;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTreeOnClick();

        updateUI();
    }
}
