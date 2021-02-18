using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerStatistics
{
    public List<MonsterScriptableObject> squad;
    public Dictionary<string, Monster> monstersDict = new Dictionary<string, Monster>();
    public float playerSpeed = 5f;

    public void FillBaseValues()
    {
        squad.ForEach(m => monstersDict.Add(m.Name, new Monster(m)));
    }
}
