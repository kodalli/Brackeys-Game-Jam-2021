using UnityEngine;

//public enum Status
//{
//    Neutral,
//    Stunned,
//    Enraged,
//    Depressed,
//    Traumatized,
//    High,
//    Sad,
//    Happy,
//    Horny,
//    Confused,
//    Woke,
//}

[CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObjects/MonsterScriptableObject", order = 1)]
public class MonsterScriptableObject : ScriptableObject
{
    // current hp, current attack, current defense, current mp
    public string Name;
    public int BaseHP = 15;
    public int BaseMP = 10;
    public int BaseLevel = 0;
    public int BaseAttack = 5;
    public int BaseDefense  = 5;
    //public Status CurrentStatus;
    public GameObject Prefab;
}
