using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCPath : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private bool flipPath;

    private List<Vector3> Path = new List<Vector3>();
    private List<Vector3> WalkableTiles = new List<Vector3>();
    private List<Vector2Int> WalkableIndexes = new List<Vector2Int>();

    private void Start() {
        tileMap.CompressBounds();
        var path = FindPathInTilemapCoordinates();
        Path = path;
        StartCoroutine(WalkPath(path));
    }

    public List<Vector3> FindPathInTilemapCoordinates() {

        Vector3[,] Points = new Vector3[tileMap.cellBounds.size.x, tileMap.cellBounds.size.y];
        bool[,] Map = new bool[tileMap.cellBounds.size.x, tileMap.cellBounds.size.y];

        WalkableTiles.Clear();
        //List<Vector3> Path = new List<Vector3>();
        for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; x++) {
            for (int y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; y++) {
                Vector3Int localPlace = new Vector3Int(x, y, (int)tileMap.transform.position.y);
                Vector3 worldPlace = tileMap.CellToWorld(localPlace);

                int i = x - tileMap.cellBounds.xMin;
                int j = y - tileMap.cellBounds.yMin;

                if (tileMap.HasTile(localPlace)) {
                    Points[i, j] = worldPlace;
                    Map[i, j] = true;
                    //Path.Add(worldPlace);
                    WalkableTiles.Add(worldPlace);
                    WalkableIndexes.Add(new Vector2Int(i, j));
                    //Debug.Log(worldPlace);
                } else { Map[i, j] = false; }

            }
        }

        // get index of start and end position in Map

        var res = GetStartEndPos(Map, WalkableIndexes);
        var startIndex = res.Item1;
        var endIndex = res.Item2;

        if (flipPath) {
            startIndex = res.Item2;
            endIndex = res.Item1;
        }

        var astar = new AStarPathFinding();
        var path = astar.FindPath(Map, startIndex, endIndex);
        Debug.Log(path.Count);

        Path.Clear();

        foreach(var loc in path) {
            Path.Add(Points[loc.x, loc.y]);
        }

        return Path;
    }

    IEnumerator WalkPath(List<Vector3> path) {
        var rb = GetComponent<Rigidbody2D>();
        foreach(var coord in path) {
            //transform.position = coord;
            //rb.MovePosition(transform.position + coord * Time.fixedDeltaTime);
            transform.position = Vector3.MoveTowards(transform.position, coord, 1f);
            yield return new WaitForSeconds(0.2f);
        }
    }


    (Vector2Int, Vector2Int) GetStartEndPos(bool[,] map, List<Vector2Int> walkable) {
        var start = new Vector2Int();
        var end = new Vector2Int();

        var astar = new AStarPathFinding();
        var tile = astar.tiletemp;

        var count = 0;

        foreach(var walk in walkable) {
            tile.location = walk;
            if (AStarPathFinding.GetWalkable(tile, map).Count < 2) {
                if (count == 0) start = walk;
                if (count == 1) end = walk;
                count++;
            }
            if (count == 2) return (start, end);
        }


        return (start, end);
    }
}
