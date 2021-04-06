using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Singleton<EnemyController> {

    PlayerData playerData;
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    RigidbodyConstraints2D rigidbodyConstraints2D;

    public int scorePoints = 500;
    public int currentHealth;
    public int maxHealth;
    public int contactDamage;
    public int explosionDamage;
    public bool isInvincible;
    public bool freezeEnemy;
    public float damageTimer = 0.1f;
    GameObject explodeEffect;
    public AudioClip shootBullet;

    [SerializeField] string explodeEffectPrefabName;
    [SerializeField] AudioClip damageClip;
    [SerializeField] AudioClip blockAttackClip;


    void Start() {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        //explodeEffect.SetActive(false);

        currentHealth = maxHealth;
    }
    public void Invincible(bool invincibility) {
        this.isInvincible = invincibility;
    }
    public void TakeDamage(float damage) {
        if (!isInvincible) {
            currentHealth -= (int)damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            SoundManager.Instance.Play(damageClip);
            if (currentHealth <= 0) {
                StartCoroutine(Defeat());
            }
        } else {
            SoundManager.Instance.Play(blockAttackClip);
        }
        StartCoroutine(ChangeColor(Color.red));
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
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
        GameManager.Instance.AddScorePoints(this.scorePoints);
    }
    public void FreezeEnemy(bool freeze) {
        if (freeze) {
            freezeEnemy = true;
            animator.speed = 0;
            rigidbodyConstraints2D = rb2d.constraints;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        } else {
            freezeEnemy = false;
            animator.speed = 1;
            rb2d.constraints = rigidbodyConstraints2D;
        }
    }
    private void OnTriggerStay2D(Collider2D other) {

        if (!isInvincible) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerX.Instance.Hit1State.HitSide(transform.position.x > PlayerX.Instance.transform.position.x);
                PlayerX.Instance.Hit1State.enemyDamage = this.contactDamage;
                PlayerX.Instance.StateMachine.ChangeState(PlayerX.Instance.Hit1State);
            }
        }
    }

    IEnumerator ChangeColor(Color color) {
        var countDown = damageTimer;
        sprite.color = color;
        while (countDown > 0) {
            countDown -= Time.deltaTime;
            yield return default;
        }
        sprite.color = Color.white;
    }
}
