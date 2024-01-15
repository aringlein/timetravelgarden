using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public GameObject growingLemonTreePrefab;
    public GameObject growingBlueTreePrefab;
    public GameObject growingDeathTreePrefab;

    public Slider slider;

    public TextMeshProUGUI clock;

    public TMP_Dropdown seedSelector;
    public TextMeshProUGUI seedCounts;

    public int lemonSeeds = 10;
    public int blueSeeds = 10;
    public int deathSeeds = 10;

    public float gridToWorldScaleFactor = 4.0f;
    public static int GRID_SIZE = 12;

    public float daysPerTimeUnit = 1.0f;
    private int[,] usedGridLocations = new int[GRID_SIZE, GRID_SIZE];

    public float currentTime = 0; // units: TIME
    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0; //Time.time;
                         // usedGridLocations = new int[GRID_SIZE, GRID_SIZE];
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
        usedGridLocations[gridLocation.x, gridLocation.y] = used ? 1 : 0;
    }

    private Vector2Int getGridLocation(Vector3 position)
    {
        int x = (int)Math.Round((position.x) / gridToWorldScaleFactor);
        int z = (int)Math.Round((position.z) / gridToWorldScaleFactor);
        // convert to all positive coordinates
        return new Vector2Int(x + GRID_SIZE / 2, z + GRID_SIZE / 2);
    }

    private Vector3 getFinalPosition(Vector2Int gridLocation)
    {
        return new Vector3((gridLocation.x - GRID_SIZE / 2) * gridToWorldScaleFactor, 0, (gridLocation.y - GRID_SIZE / 2) * gridToWorldScaleFactor);
    }

    void spawnTreeOnClick()
    {
        // Disallow planting trees while time is reversing
        if (slider.value < 0) return;
        // Spawn a cube when mouse is clicked, where the mouse intersects with the plane
        if (Input.GetMouseButtonDown(0))
        {
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
                if (usedGridLocations[gridLocation.x, gridLocation.y] == 1) return;
                usedGridLocations[gridLocation.x, gridLocation.y] = 1;
                Vector3 finalPoint = getFinalPosition(gridLocation);

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

                GameObject tree = Instantiate(prefabToInstantiate, finalPoint, Quaternion.identity);


            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawnTreeOnClick();

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
    }
}
