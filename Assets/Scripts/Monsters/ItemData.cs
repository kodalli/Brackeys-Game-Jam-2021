using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemDataScriptableObject", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public List<MonsterScriptableObject> fusableMonsters;
    public List<MonsterScriptableObject> fusedMonsters;
    public string itemDescription;
    public Sprite sprite;

    public Monster FuseMonsterWithItem(Monster monsterToFuse)
    {
        var level = monsterToFuse.GetLevel();
        var fused = fusableMonsters.FirstOrDefault(item => item.Name == monsterToFuse.Name);
        var index = fusableMonsters.IndexOf(fused);
        var monster = new Monster(fusedMonsters[index]);
        monster.LevelUp(level);
        return monster;
    }
}
