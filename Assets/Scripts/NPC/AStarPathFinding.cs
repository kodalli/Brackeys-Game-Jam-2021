using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarPathFinding
{
    public Tile tiletemp = new Tile();

    public class Tile {
        public Vector2Int location;
        public int F;
        public int G;
        public int H;
        public Tile parent;
    }

    public List<Vector2Int> FindPath(bool[,] grid, Vector2Int beg, Vector2Int end) {
        bool[,] map = grid;
        Tile start = new Tile { location = beg };
        Tile target = new Tile { location = end };
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        Tile current = null;
        int g = 0;

        openList.Add(start);

        while (openList.Count > 0) {
            int lowest = openList.Min(item => item.F);
            current = openList.First(item => item.F == lowest);
            closedList.Add(current);
            openList.Remove(current);

            if (closedList.FirstOrDefault(item => item.location == target.location) != null)
                break;

            List<Tile> adjSquares = GetWalkable(current, map);
            g++;

            foreach (var adjSq in adjSquares) {
                if (closedList.FirstOrDefault(item => item.location == adjSq.location) != null)
                    continue;

                if (openList.FirstOrDefault(item => item.location == adjSq.location) == null) {
                    adjSq.G = g;
                    adjSq.H = HScore(adjSq.location, target.location);
                    adjSq.F = adjSq.G + adjSq.H;
                    adjSq.parent = current;
                    openList.Insert(0, adjSq);
                }
                else if (g + adjSq.H < adjSq.F) {
                    adjSq.G = g;
                    adjSq.F = adjSq.G + adjSq.H;
                    adjSq.parent = current;
                }
            }
        }

        List<Vector2Int> path = new List<Vector2Int>();

        while (current != null) {
            path.Add(current.location);
            current = current.parent;
        }

        path.Reverse();

        return path;
    }

    public static List<Tile> GetWalkable(Tile cur, bool[,] map) {
        List<Tile> adj = new List<Tile>
        {
            new Tile { location = new Vector2Int(0, -1) + cur.location },
            new Tile { location = new Vector2Int(0, 1) + cur.location },
            new Tile { location = new Vector2Int(-1, 0) + cur.location },
            new Tile { location = new Vector2Int(1, 0) + cur.location },
        };

        adj = adj.Where(item => item.location.x >= 0 && item.location.x < map.GetLength(0) && item.location.y >= 0 && item.location.y < map.GetLength(1)).ToList();

        return adj.Where(item => map[item.location.x, item.location.y] == true).ToList();

    }

    public virtual int HScore(Vector2Int cur, Vector2Int target) {
        return Mathf.Abs(cur.x - target.x) + Mathf.Abs(cur.y - target.y);
    }

}
