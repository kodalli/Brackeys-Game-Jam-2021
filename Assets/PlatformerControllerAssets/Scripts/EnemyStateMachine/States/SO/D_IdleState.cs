using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newIdleStateData", menuName = "Enemy State Data/Idle State Data")]
public class D_IdleState : ScriptableObject {

    public float minIdleTime = 1f;
    public float maxIdleTime = 2f;

}
