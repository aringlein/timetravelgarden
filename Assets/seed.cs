using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seed : MonoBehaviour
{
    public GameObject growingTree;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If already picked up, do nothing.
        if (growingTree.GetComponent<growingtree>().seedPickUpAge != null)
        {
            Debug.Log("Seed already picked up in the future");
            return;
        }
        // When this object is clicked on, set growingTree.seedPickUpTime to the current time
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
                if (Vector3.Distance(candidatePoint, transform.position) < 1.0f)
                {
                    growingTree.GetComponent<growingtree>().pickUpSeed();
                }
            }
        }
    }
}
