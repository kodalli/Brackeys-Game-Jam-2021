using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Enemy State Data/Entity Data")]
public class SO_Entity : ScriptableObject {

    public float wallCheckDistance = 0.7f;
    public float ledgeCheckDistance = 0.6f;

    public float minAgroDistance = 3f;
    public float maxAgroDistance = 4f;

    public float closeRangeActionDistance = 1f;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

}
