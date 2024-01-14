using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

public class growingtree : MonoBehaviour
{
    public GameObject treePrefabYoung;
    public GameObject treePrefabMiddle;
    public GameObject treePrefabOld;
    public GameObject seedPrefab;

    public double birthTime;
    public double? seedPickUpAge;

    public double timeToBeYoung;
    public double timeToBeMiddle;
    public double timeToBeOld;

    private GameObject ownedTree;

    public GameObject gameManager;

    private enum TreeState
    {
        PreBirth,
        Young,
        Middle,
        Old,
        Seed,
        PickedUp,
    }
    private TreeState treeState;

    public enum TreeType
    {
        Lemon,
        Blue,
        Death,
    }
    public TreeType treeType;

    public float SCALE_RATE = 0.01f;
    public float MAX_SCALE = 2.0f;
    // public float MIN_SCALE = 1.0f;

    private float getCurrentTime()
    {
        return gameManager.GetComponent<gamemanager>().currentTime;
    }

    public void pickUpSeed()
    {
        seedPickUpAge = getCurrentTime() - birthTime;
        gameManager.GetComponent<gamemanager>().pickUpSeed(treeType, 2);
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        // Instantiate a lemon tree prefab at the intersection point
        ownedTree = Instantiate(treePrefabYoung, transform.position, Quaternion.identity);
        birthTime = getCurrentTime();
        treeState = TreeState.Young;
    }

    TreeState expectedState()
    {
        float timeSinceBirth = (float)(getCurrentTime() - birthTime);
        if (timeSinceBirth < 0) return TreeState.PreBirth;
        else if (timeSinceBirth <= timeToBeYoung) return TreeState.Young;
        else if (timeSinceBirth <= timeToBeYoung + timeToBeMiddle) return TreeState.Middle;
        else if (timeSinceBirth <= timeToBeOld + timeToBeMiddle + timeToBeYoung) return TreeState.Old;
        else if (seedPickUpAge == null || timeSinceBirth <= seedPickUpAge) return TreeState.Seed;
        else return TreeState.PickedUp;
    }

    // Update is called once per frame
    void Update()
    {

        float currentTime = getCurrentTime();
        if (ownedTree != null && treeState != TreeState.Seed)
        {
            // Scale the tree based on time
            float scale = 1 + Math.Min((float)(currentTime - birthTime) * SCALE_RATE, MAX_SCALE);
            ownedTree.transform.localScale = new Vector3(scale, scale, scale);
        }

        TreeState expected = expectedState();
        if (expected == treeState)
        {
            return;
        }
        Destroy(ownedTree);
        treeState = expected;

        // Change the treeState from young to middle to old if needed, based on the current time
        // If the treeState changes, destroy ownedTree and reinstantiate it with the new prefab
        if (treeState == TreeState.Young)
        {
            ownedTree = Instantiate(treePrefabYoung, transform.position, Quaternion.identity);
        }
        else if (treeState == TreeState.Middle)
        {
            ownedTree = Instantiate(treePrefabMiddle, transform.position, Quaternion.identity);
        }
        else if (treeState == TreeState.Old)
        {
            ownedTree = Instantiate(treePrefabOld, transform.position, Quaternion.identity);
        }
        else if (treeState == TreeState.Seed)
        {
            // Spawn a seed
            ownedTree = Instantiate(seedPrefab, transform.position, Quaternion.identity);
            ownedTree.GetComponent<seed>().growingTree = gameObject;
        }
    }
}
