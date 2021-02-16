using UnityEngine;

public enum Status
{
    Neutral,
    Stunned,
    Enraged,
    Depressed,
    Traumatized,
    High,
    Sad,
    Happy,
    Horny,
    Confused,
    Woke,
}

[CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObjects/MonsterScriptableObject", order = 1)]
public class MonsterScriptableObject : ScriptableObject
{
    // current hp, current attack, current defense, current mp
    public string Name;
    public int HP;
    public int MP;
    public int Level;
    public int XP;
    public int Attack;
    public int Defense;
    public bool Fainted = false;
    public Status CurrentStatus;
}
