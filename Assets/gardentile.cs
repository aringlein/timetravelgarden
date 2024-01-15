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
    public TileState queuedTileState = TileState.Available;
    public GameObject ownedTile;

    public Dictionary<float, GameObject> treesByBirthTime = new Dictionary<float, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        ownedTile = Instantiate(tileAvailablePrefab, transform.position, Quaternion.identity);
    }

    public void queueTileStateChange(TileState newTileState)
    {
        queuedTileState = newTileState;
    }
    // I'm not sure why this is necessary, but without queuing the state and updating within Update(), 
    // the tile doesn't change state properly.
    // I'm guessing that updates to member GameObjects need to happen in Update()? Or I am doing something dumb.
    public void updateTileStateFromQueuedState()
    {
        var newTileState = queuedTileState;
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

    public void Update()
    {
        updateTileStateFromQueuedState();
    }
}
