// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using System;

// public class NPCPath : MonoBehaviour
// {
//     public bool IsFollowing_m { get; private set; }

//     [SerializeField] private GameObject npcPaths;
//     [SerializeField] private bool flipPath;
//     [SerializeField] private float speed = 5f;
//     [SerializeField] private float countDown = 0.3f;
//     [SerializeField] private Vector2 adjust = new Vector2(0.5f, 0.5f);

//     // follow leader
//     [SerializeField] private bool isFollowing = false;
//     [SerializeField] private bool walkPathEnabled = true;
//     [SerializeField] private int numFrames = 5;
//     private Rigidbody2D rb;
//     private Animator anim;
//     private Queue<Vector3> targetMovement;
//     private Vector2 newPos;

//     // animator variables
//     private readonly int moveX = Animator.StringToHash("moveX");
//     private readonly int moveY = Animator.StringToHash("moveY");
//     private readonly int isMoving = Animator.StringToHash("isMoving");

//     private void Awake() {
//         IsFollowing_m = isFollowing;
//     }

//     private void Start() {
//         // WalkThePath(0);
//         targetMovement = new Queue<Vector3>();
//         rb = GetComponent<Rigidbody2D>();
//         anim = GetComponent<Animator>();
//         rb.bodyType = RigidbodyType2D.Kinematic;
//         newPos = transform.position;
//         //Debug.Log("In start " + IsFollowing_m);
//     }

//     public void FollowLeader() {
//         if (!isFollowing) return;

//         if (PlayerController.Instance.DeltaPosition != Vector3.zero) {
//             anim.SetBool(isMoving, true);
//             anim.SetFloat(moveX, (newPos - rb.position).x);
//             anim.SetFloat(moveY, (newPos - rb.position).y);
//             targetMovement.Enqueue(PlayerController.Instance.transform.position);
//         } else {
//             anim.SetBool(isMoving, false);
//         }

//         if (targetMovement.Count > numFrames) {
//             newPos = targetMovement.Dequeue();
//         }

//         rb.MovePosition(rb.position + (newPos - rb.position) * Time.deltaTime * speed);
//     }

//     #region Walk Path Logic
//     public List<Vector3> FindPathInTilemapCoordinates(Tilemap tileMap) {

//         Vector3[,] Points = new Vector3[tileMap.cellBounds.size.x, tileMap.cellBounds.size.y];
//         bool[,] Map = new bool[tileMap.cellBounds.size.x, tileMap.cellBounds.size.y];
//         List<Vector2Int> WalkableIndexes = new List<Vector2Int>();

//         for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; x++) {
//             for (int y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; y++) {
//                 Vector3Int localPlace = new Vector3Int(x, y, (int)tileMap.transform.position.y);
//                 Vector3 worldPlace = tileMap.CellToWorld(localPlace);

//                 int i = x - tileMap.cellBounds.xMin;
//                 int j = y - tileMap.cellBounds.yMin;

//                 if (tileMap.HasTile(localPlace)) {
//                     Points[i, j] = worldPlace;
//                     Map[i, j] = true;
//                     WalkableIndexes.Add(new Vector2Int(i, j));
//                 } else { Map[i, j] = false; }

//             }
//         }

//         // get index of start and end position in Map

//         var res = GetStartEndPos(Map, WalkableIndexes);
//         var startIndex = res.Item1;
//         var endIndex = res.Item2;

//         if (flipPath) {
//             startIndex = res.Item2;
//             endIndex = res.Item1;
//         }

//         var astar = new AStarPathFinding();
//         var path = astar.FindPath(Map, startIndex, endIndex);
//         //Debug.Log(path.Count);

//         var Path = new List<Vector3>();

//         foreach(var loc in path) {
//             Path.Add(Points[loc.x, loc.y]);
//         }

//         return Path;
//     }

//     public void WalkThePath(int index) {
//         if (!walkPathEnabled) return;

//         var tileMaps = npcPaths.GetComponentsInChildren<Tilemap>();

//         Debug.Log($"path count {tileMaps.Length}");

//         if (index < tileMaps.Length) {
//             var tileMap = tileMaps[index];
//             tileMap.CompressBounds();
//             var path = FindPathInTilemapCoordinates(tileMap);
//             StartCoroutine(WalkPath(path));
//         }
//     }

//     IEnumerator WalkPath(List<Vector3> path) {
//         foreach (var coord in path) {
//             var cd = countDown;
//             var coordAdj = (Vector2)coord + adjust;
//             while (cd > 0) {
//                 rb.MovePosition(rb.position + (coordAdj - rb.position) * Time.deltaTime * speed);
//                 if ((coordAdj - rb.position) != Vector2.zero) {
//                     anim.SetBool(isMoving, true);
//                     anim.SetFloat(moveX, (coordAdj - rb.position).x);
//                     anim.SetFloat(moveY, (coordAdj - rb.position).y);
//                 }
//                 cd -= Time.deltaTime;  
//                 yield return default;
//             }
//         }
//         anim.SetBool(isMoving, false);

//         // update values in enmeyPathCounter
//         var dict = PlayerControlSave.Instance.localPlayerData.enemyPathCounter;
//         var name = GetComponent<BattleNPC>().Name;
//         var index = dict[name].Item1 + 1; 
//         dict[name] = new Tuple<int, Vector3>(index, transform.position);
//     }


//     (Vector2Int, Vector2Int) GetStartEndPos(bool[,] map, List<Vector2Int> walkable) {
//         var start = new Vector2Int();
//         var end = new Vector2Int();

//         var astar = new AStarPathFinding();
//         var tile = astar.tiletemp;

//         var count = 0;

//         foreach(var walk in walkable) {
//             tile.location = walk;
//             if (AStarPathFinding.GetWalkable(tile, map).Count < 2) {
//                 if (count == 0) start = walk;
//                 if (count == 1) end = walk;
//                 count++;
//             }
//             if (count == 2) return (start, end);
//         }


//         return (start, end);
//     }
//     #endregion
// }
