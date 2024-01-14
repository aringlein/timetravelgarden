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

    public int GRID_SIZE = 4;
    public int PLANE_SIZE = 6;

    public float daysPerTimeUnit = 1.0f;

    public float currentTime = 0; // units: TIME
    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0; //Time.time;
    }

    public void pickUpSeed(growingtree.TreeType treeType, int count)
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
    }

    void spawnTreeOnClick()
    {
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

                // // Instantiate a cube at the intersection point

                //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 candidatePoint = ray.GetPoint(distance);
                int x = (int)Math.Round((candidatePoint.x) / GRID_SIZE);
                int z = (int)Math.Round((candidatePoint.z) / GRID_SIZE);
                if (x < -PLANE_SIZE || x > PLANE_SIZE || z > PLANE_SIZE || z < -PLANE_SIZE) return;
                Vector3 finalPoint = new Vector3(x * GRID_SIZE, candidatePoint.y, z * GRID_SIZE);

                // Debug.Log("finalPoint: " + finalPoint);
                // Debug.Log("candidatePoint: " + candidatePoint);

                //  Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

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
