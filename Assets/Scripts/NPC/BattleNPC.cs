using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCStatus { Standby, Defeated, Fighting }

public class BattleNPC : MonoBehaviour
{
    public string Name;
    public NPCStatus State;
    public int level;
    public List<MonsterScriptableObject> squad;
    public Dictionary<string, Monster> monstersDict = new Dictionary<string, Monster>();

    private void Start()
    {
        FillBaseValues();
    }

    public void FillBaseValues()
    {
        foreach(var monster in squad)
        {
            var mObj = new Monster(monster);
            mObj.LevelUp(level);
            monstersDict.Add(monster.Name, mObj);
        }
    }
}
