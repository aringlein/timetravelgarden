using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

public class gamemanager : MonoBehaviour
{
    public GameObject lemonTreePrefab;


    public int GRID_SIZE = 4;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
                Vector3 finalPoint = new Vector3(x * GRID_SIZE, candidatePoint.y, z * GRID_SIZE);

                // Debug.Log("finalPoint: " + finalPoint);
                // Debug.Log("candidatePoint: " + candidatePoint);

                //  Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                GameObject lemonTree = Instantiate(lemonTreePrefab, finalPoint, Quaternion.identity);

            }
        }

    }
}
