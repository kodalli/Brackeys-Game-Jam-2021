using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum BulletType { TYPE_A, TYPE_B, TYPE_C, TYPE_D }
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
    private void Start() {
        StartCoroutine(DestroyBullet());
    }
    private void Update() {
        if (freezeBullet) return; ;
    }

    public void SetBulletSpeed(float speed) => bulletSpeed = speed;
    public void SetBulletDirection(Vector2 direction) => bulletDirection = direction;
    public void SetDamageValue(float damage) => bulletDamage = damage;

    public void Shoot() {
        SR.flipX = (bulletDirection.x < 0);
        RB.velocity = bulletDirection * bulletSpeed;
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
    IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(bulletDestroyDelay);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        other.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage((int)this.bulletDamage);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(this.gameObject, 0.1f);
    }

}
