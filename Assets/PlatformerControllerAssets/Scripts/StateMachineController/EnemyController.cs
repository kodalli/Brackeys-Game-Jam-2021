using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    bool isInvincible;

    public int currentHealth;
    public int maxHealth = 1;
    public int contactDamage = 1;

    void Start() {
        currentHealth = maxHealth;
    }
    public void Invincible(bool invincibility) {
        isInvincible = invincibility;
    }
    public void TakeDamage(float damage) {
        if (!isInvincible) {
            currentHealth -= (int)damage;
            Mathf.Clamp(currentHealth, 0, maxHealth);
            if(currentHealth <= 0) {
                Defeat();
            }
        }
    }
    void Defeat() {
        Destroy(gameObject);
    }
    private void OnTriggerStay2D(Collider2D other) {

        //if (other.gameObject.CompareTag("Player")) {
        //    PlayerX player = other.gameObject.GetComponent<PlayerX>();
        //    player.Hit1State.HitSide(transform.position.x > player.transform.position.x);
        //    player.StateMachine.ChangeState(player.Hit1State);
        //}
        if (other.gameObject.CompareTag("Player")) {
            PlayerX.Instance.Hit1State.HitSide(transform.position.x > PlayerX.Instance.transform.position.x);
            PlayerX.Instance.StateMachine.ChangeState(PlayerX.Instance.Hit1State);
        }

    }
}
