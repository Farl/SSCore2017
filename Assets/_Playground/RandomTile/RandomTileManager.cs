using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTileManager : MonoBehaviour
{
    [SerializeField]
    RandomTile tileData;

    public float tileSize = 1;

    List<RandomTile.Tile> tiles = new List<RandomTile.Tile>();
    List<GameObject> tileGOs = new List<GameObject>();

    public class TileList
    {
        public List<RandomTile.Tile> tiles = new List<RandomTile.Tile>();
    }

    TileList[] tileList = new TileList[4];

    [ContextMenu("Generate")]
    public void Generate()
    {
        foreach (var t in tileGOs)
        {
            if (t)
                Destroy(t);
        }
        if (tileData && tileData.tiles.Length > 0)
        {
            // Generate Tile list
            for (int i = 0; i < 4; i++)
            {
                tileList[i] = new TileList();
                tileList[i].tiles = Get((RandomTile.Direction)(1 << i));
            }

            //

            Vector3 pos = Vector3.zero;

            int idx = Random.Range(0, tileData.tiles.Length);
            var t = tileData.tiles[idx];

            var tCopy = new RandomTile.Tile(t);
            tiles.Add(tCopy);

            tileGOs.Add(Instantiate(tCopy.gameObject, pos, Quaternion.identity));

            for (int i = 0; i < 4; i++)
            {
                if (((int)t.direction & (1 << i)) != 0)
                {
                    var invDir = (((i + 2) % 4));

                    if (tileList[invDir].tiles.Count <= 0)
                        break;

                    int invIdx = Random.Range(0, tileList[invDir].tiles.Count);
                    var adjTile = tileList[invDir].tiles[invIdx];
                    var adjTileCopy = new RandomTile.Tile(adjTile);
                    tiles.Add(adjTileCopy);

                    Vector3 offset = Vector3.left;
                    switch ((RandomTile.Direction)(1 << i))
                    {
                        case RandomTile.Direction.X:
                            offset = new Vector3(1, 0, 0);
                            break;
                        case RandomTile.Direction.Minus_X:
                            offset = new Vector3(-1, 0, 0);
                            break;
                        case RandomTile.Direction.Z:
                            offset = new Vector3(0, 0, 1);
                            break;
                        case RandomTile.Direction.Minus_Z:
                            offset = new Vector3(0, 0, -1);
                            break;
                    }
                    tileGOs.Add(Instantiate(adjTileCopy.gameObject, pos + offset * tileSize, Quaternion.identity));
                }
            }

            // Dump
            foreach (var tile in tiles)
            {

            }
        }
    }

    List<RandomTile.Tile> Get(RandomTile.Direction dir)
    {
        List<RandomTile.Tile> result = new List<RandomTile.Tile>();
        if (tileData && tileData.tiles.Length > 0)
        {
            foreach (var t in tileData.tiles)
            {
                if ( ((int)t.direction & (int)dir) != 0 )
                {
                    result.Add(t);
                }
            }
            return result;
        }
        return null;
    }
}
