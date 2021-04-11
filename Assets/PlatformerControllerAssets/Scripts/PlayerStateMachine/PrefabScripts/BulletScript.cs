using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    Animator animator;
    Rigidbody2D RB;
    SpriteRenderer SR;

    float destroyTime;

    RigidbodyConstraints2D rigidbodyConstraints2D;

    bool freezeBullet = false;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Vector2 bulletDirection;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletDestroyDelay;

    private void Awake() {
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    private void Update() {

        if (freezeBullet) return;

        destroyTime -= Time.deltaTime;
        if (destroyTime < 0) Destroy(gameObject);
    }

    public void SetBulletSpeed(float speed) {
        bulletSpeed = speed;
    }
    public void SetBulletDirection(Vector2 direction) {
        bulletDirection = direction;
    }
    public void SetDamageValue(float damage) {
        bulletDamage = damage;
    }
    public void SetDestroyDelay(float delay) {
        bulletDestroyDelay = delay;
    }
    public void Shoot() {
        SR.flipX = (bulletDirection.x < 0);
        RB.velocity = bulletDirection * bulletSpeed;
        destroyTime = bulletDestroyDelay;
    }

    public void FreezeBullet(bool freeze) {
        if (freeze) {
            freezeBullet = true;
            rigidbodyConstraints2D = RB.constraints;
            animator.speed = 0;
            RB.constraints = RigidbodyConstraints2D.FreezeAll;
            RB.velocity = Vector2.zero;
        } else {
            freezeBullet = false;
            animator.speed = 1;
            RB.constraints = rigidbodyConstraints2D;
            RB.velocity = bulletDirection * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy")) {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy != null) {
                enemy.TakeDamage(this.bulletDamage);
            }
            Destroy(gameObject);
        }
    }

}
