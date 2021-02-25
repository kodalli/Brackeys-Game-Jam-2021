using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerStatistics {
    public List<MonsterScriptableObject> squad;
    public Dictionary<string, Monster> monstersDict = new Dictionary<string, Monster>();
    public float playerSpeed;
    public List<ItemData> playerItems;
    public bool finishedTutorial = false;
    public Vector3 playerPosition;
    public NPCBattleWrapper enemyData;
    public List<string> defeatedEnemyNames;
    // counter and last position
    // { "oldman" : (0, Vector3(1f, 3.2f, 1.4f)), "girl" : (1, Vector3(1f, 69.69f, 1.4f)) }
    public Dictionary<string, Tuple<int, Vector3>> enemyPathCounter = new Dictionary<string, Tuple<int, Vector3>>();

    public void FillBaseValues() {
        squad.ForEach(m => monstersDict.Add(m.Name, new Monster(m)));
    }
}
