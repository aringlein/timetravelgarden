using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class growingtree : MonoBehaviour
{
    public GameObject lemonTreePrefabYoung;
    public GameObject lemonTreePrefabMiddle;
    public GameObject lemonTreePrefabOld;
    public double birthTime;
    public double timeToBeYoung;
    public double timeToBeMiddle;

    private GameObject ownedTree;

    private enum TreeState
    {
        Young,
        Middle,
        Old
    }
    private TreeState treeState;


    // Start is called before the first frame update
    void Start()
    {
        // Instantiate a lemon tree prefab at the intersection point
        ownedTree = Instantiate(lemonTreePrefabYoung, transform.position, Quaternion.identity);
        birthTime = Time.time;
        treeState = TreeState.Young;
    }

    // Update is called once per frame
    void Update()
    {
        // Change the treeState from young to middle to old if needed, based on the current time
        // If the treeState changes, destroy ownedTree and reinstantiate it with the new prefab
        if (Time.time - birthTime > timeToBeYoung && treeState == TreeState.Young)
        {
            treeState = TreeState.Middle;
            Destroy(ownedTree);
            ownedTree = Instantiate(lemonTreePrefabMiddle, transform.position, Quaternion.identity);
        }
        else if (Time.time - birthTime > timeToBeMiddle && treeState == TreeState.Middle)
        {
            treeState = TreeState.Old;
            Destroy(ownedTree);
            ownedTree = Instantiate(lemonTreePrefabOld, transform.position, Quaternion.identity);
        }

    }
}
