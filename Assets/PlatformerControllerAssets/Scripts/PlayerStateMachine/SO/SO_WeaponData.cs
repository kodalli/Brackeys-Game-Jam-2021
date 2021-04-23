using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Player Data/Weapon Data/Sword")]
public class SO_WeaponData : ScriptableObject {

    public float[] movementSpeed;

    public float meleeAttackDamage = 10f;

    public float resetTime = 0.5f;

}
