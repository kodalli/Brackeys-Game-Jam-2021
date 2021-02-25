using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCPath : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private bool flipPath;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float countDown = 0.3f;
    [SerializeField] private Vector2 adjust = new Vector2(0.5f, 0.5f);

    private readonly int moveX = Animator.StringToHash("moveX");
    private readonly int moveY = Animator.StringToHash("moveY");
    private readonly int isMoving = Animator.StringToHash("isMoving");

    //private void Start() {
    //    WalkThePath();
    //}

    public List<Vector3> FindPathInTilemapCoordinates() {

        Vector3[,] Points = new Vector3[tileMap.cellBounds.size.x, tileMap.cellBounds.size.y];
        bool[,] Map = new bool[tileMap.cellBounds.size.x, tileMap.cellBounds.size.y];
        List<Vector2Int> WalkableIndexes = new List<Vector2Int>();

        for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; x++) {
            for (int y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; y++) {
                Vector3Int localPlace = new Vector3Int(x, y, (int)tileMap.transform.position.y);
                Vector3 worldPlace = tileMap.CellToWorld(localPlace);

                int i = x - tileMap.cellBounds.xMin;
                int j = y - tileMap.cellBounds.yMin;

                if (tileMap.HasTile(localPlace)) {
                    Points[i, j] = worldPlace;
                    Map[i, j] = true;
                    WalkableIndexes.Add(new Vector2Int(i, j));
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

        var Path = new List<Vector3>();

        foreach(var loc in path) {
            Path.Add(Points[loc.x, loc.y]);
        }

        return Path;
    }

    public void WalkThePath() {
        tileMap.CompressBounds();
        var path = FindPathInTilemapCoordinates();
        StartCoroutine(WalkPath(path));
    }

    IEnumerator WalkPath(List<Vector3> path) {
        var rb = GetComponent<Rigidbody2D>();
        //var anim = GetComponent<Animator>();
        foreach(var coord in path) {
            var cd = countDown;
            var coordAdj = (Vector2)coord + adjust;
            while (cd > 0) {
                rb.MovePosition(rb.position + (coordAdj - rb.position) * Time.deltaTime * speed);
                if ((coordAdj - rb.position) != Vector2.zero) {
                    //anim.SetBool(isMoving, true);
                    //anim.SetFloat(moveX, (coordAdj - rb.position).x);
                    //anim.SetFloat(moveY, (coordAdj - rb.position).y);
                }
                cd -= Time.deltaTime;
                yield return default;
            }
        }
        //anim.SetBool(isMoving, false); 
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
