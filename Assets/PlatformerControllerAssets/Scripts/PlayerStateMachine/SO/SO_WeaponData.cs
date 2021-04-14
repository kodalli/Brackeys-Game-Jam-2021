using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Player Data/Weapon Data/Sword")]
public class SO_WeaponData : ScriptableObject {

    public float[] movementSpeed;

    public float resetTime = 0.5f;

}
