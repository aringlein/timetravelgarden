using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

public class growingtree : MonoBehaviour
{
    public GameObject treePrefabYoung;
    public GameObject treePrefabMiddle;
    public GameObject treePrefabOld;

    public double birthTime;
    public double timeToBeYoung;
    public double timeToBeMiddle;

    private GameObject ownedTree;

    public GameObject gameManager;

    private enum TreeState
    {
        Young,
        Middle,
        Old
    }
    private TreeState treeState;



    public float SCALE_RATE = 0.01f;
    public float MAX_SCALE = 2.0f;
    // public float MIN_SCALE = 1.0f;

    private float getCurrentTime()
    {
        return gameManager.GetComponent<gamemanager>().currentTime;
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

    // Update is called once per frame
    void Update()
    {

        float currentTime = getCurrentTime();
        // Scale the tree based on time
        float scale = 1 + Math.Min((float)(currentTime - birthTime) * SCALE_RATE, MAX_SCALE);
        ownedTree.transform.localScale = new Vector3(scale, scale, scale);

        float timeSinceBirth = (float)(currentTime - birthTime);


        if (timeSinceBirth < 0)
        {
            ownedTree.active = false;
        }
        else
        {
            ownedTree.active = true;
        }

        // Change the treeState from young to middle to old if needed, based on the current time
        // If the treeState changes, destroy ownedTree and reinstantiate it with the new prefab
        if (timeSinceBirth <= timeToBeYoung && treeState != TreeState.Young)
        {
            treeState = TreeState.Young;
            Destroy(ownedTree);
            ownedTree = Instantiate(treePrefabYoung, transform.position, Quaternion.identity);
        }
        else if (timeSinceBirth > timeToBeYoung && timeSinceBirth <= timeToBeYoung + timeToBeMiddle && treeState != TreeState.Middle)
        {
            treeState = TreeState.Middle;
            Destroy(ownedTree);
            ownedTree = Instantiate(treePrefabMiddle, transform.position, Quaternion.identity);
        }
        else if (timeSinceBirth > timeToBeMiddle + timeToBeYoung && treeState != TreeState.Old)
        {
            treeState = TreeState.Old;
            Destroy(ownedTree);
            ownedTree = Instantiate(treePrefabOld, transform.position, Quaternion.identity);
        }

    }
}
