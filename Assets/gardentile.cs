using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TileState = gamemanager.TileState;

public class gardentile : MonoBehaviour
{
    public GameObject tileUnavailablePrefab;
    public GameObject tileAvailablePrefab;
    public GameObject activeTilePrefab;

    public TileState tileState = TileState.Available;
    public GameObject ownedTile;

    public Dictionary<float, GameObject> treesByBirthTime = new Dictionary<float, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        ownedTile = Instantiate(tileAvailablePrefab, transform.position, Quaternion.identity);
    }

    public void updateTileState(TileState newTileState)
    {
        if (newTileState == tileState)
        {
            return;
        }
        switch (newTileState)
        {
            case TileState.Unavailable:
                Destroy(ownedTile);
                ownedTile = Instantiate(tileUnavailablePrefab, transform.position, Quaternion.identity);
                break;
            case TileState.Available:
                Destroy(ownedTile);
                ownedTile = Instantiate(tileAvailablePrefab, transform.position, Quaternion.identity);
                break;
            case TileState.InUse:
                Destroy(ownedTile);
                ownedTile = Instantiate(activeTilePrefab, transform.position, Quaternion.identity);
                break;
        }
        tileState = newTileState;

    }
}
