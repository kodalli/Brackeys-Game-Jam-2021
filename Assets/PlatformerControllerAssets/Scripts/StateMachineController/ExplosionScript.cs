using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {
    PlayerData playerData;
    int damage = 0;

    public void SetDamageValue(int damage) {
        this.damage = damage;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (this.damage > 0) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerX.Instance.Hit1State.HitSide(transform.position.x > PlayerX.Instance.transform.position.x);
                PlayerX.Instance.Hit1State.enemyDamage = this.damage;
                PlayerX.Instance.StateMachine.ChangeState(PlayerX.Instance.Hit1State);
            }
        }
    }

}
