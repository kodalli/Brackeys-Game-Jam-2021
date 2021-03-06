using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Temp Data")]
public class PlayerDataX : ScriptableObject{

    [Header("Move State")]
    public float movementVelocity = 10f;
}
