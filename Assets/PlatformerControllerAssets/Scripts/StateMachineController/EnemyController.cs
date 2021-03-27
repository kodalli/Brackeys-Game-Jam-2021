using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class EnemyController : MonoBehaviour {

    PlayerData playerData;
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    GameObject explodeEffect;
    [SerializeField] string explodeEffectPrefabName;

    public int currentHealth;
    public int maxHealth;
    public int contactDamage;
    public int explosionDamage;

    bool isInvincible;

    void Start() {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        //explodeEffect.SetActive(false);

        currentHealth = maxHealth;
    }
    public void Invincible(bool invincibility) {
        isInvincible = invincibility;
    }
    public void TakeDamage(float damage) {
        if (!isInvincible) {
            currentHealth -= (int)damage;
            Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth <= 0) {
                StartCoroutine(Defeat());
            }
        }
    }
    void StartDefeatAnimation() {
        explodeEffect = Instantiate((GameObject)Resources.Load(explodeEffectPrefabName));
        explodeEffect.name = "ExplodeEffect1";
        explodeEffect.transform.position = sprite.bounds.center;
        explodeEffect.GetComponent<ExplosionScript>().SetDamageValue(this.explosionDamage);
        Destroy(explodeEffect, .2f);
    }
    void StopDefeatAnimation() {
        Destroy(explodeEffect);
    }
    IEnumerator Defeat() {
        StartDefeatAnimation();
        // explodeEffect.SetActive(true);
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
    }
    private void OnTriggerStay2D(Collider2D other) {

        //if (other.gameObject.CompareTag("Player")) {
        //    PlayerX player = other.gameObject.GetComponent<PlayerX>();
        //    player.Hit1State.HitSide(transform.position.x > player.transform.position.x);
        //    player.StateMachine.ChangeState(player.Hit1State);
        //}
        if (other.gameObject.CompareTag("Player")) {
            PlayerX.Instance.Hit1State.enemyDamage = this.contactDamage;
            PlayerX.Instance.Hit1State.HitSide(transform.position.x > PlayerX.Instance.transform.position.x);
            PlayerX.Instance.StateMachine.ChangeState(PlayerX.Instance.Hit1State);
        }

    }
}
