using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    Rigidbody2D RB;
    SpriteRenderer SR;

    float destroyTime;

    [SerializeField] private float bulletSpeed;
    [SerializeField] private Vector2 bulletDirection;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletDestroyDelay;

    private void Awake() {
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
    }
    private void Update() {
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
    private void OnTriggerEnter2D(Collider2D other) {
       Destroy(gameObject);
    }


}
