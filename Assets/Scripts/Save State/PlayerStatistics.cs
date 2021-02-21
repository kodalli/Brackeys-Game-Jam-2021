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

    public void FillBaseValues() {
        squad.ForEach(m => monstersDict.Add(m.Name, new Monster(m)));
    }
}
