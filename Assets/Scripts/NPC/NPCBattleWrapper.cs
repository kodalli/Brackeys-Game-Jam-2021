using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBattleWrapper {
    public string Name;
    public NPCStatus State;
    public int level;
    public List<MonsterScriptableObject> squad;
    public Dictionary<string, Monster> monstersDict = new Dictionary<string, Monster>();

    public void Wrap(BattleNPC other) {
        Name = other.Name;
        State = other.State;
        level = other.level;
        squad = other.squad;
        monstersDict = other.monstersDict;
    }
}
