using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMeleeAttackStateData", menuName = "Enemy State Data/Melee Attack State Data")]
public class D_MeleeAttackState : ScriptableObject {
    public float attackRadius = 0.5f;
    public int attackDamage = 20;
    public float hitForceX = 10f;
    public float hitForceY = 5f;
    public LayerMask whatIsPlayer;
}
