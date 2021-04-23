using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        // Enemy1.enemyDelegate += ass;

        Enemy1 damageable = other.gameObject.GetComponentInParent<Enemy1>();
        if (damageable != null) damageable.enemyDelegate += ass;

        other.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(10);

    }

    private void OnTriggerExit2D(Collider2D other) {
        other.gameObject.GetComponentInParent<Enemy1>().enemyDelegate -= ass;
    }

    void ass() => Debug.Log("ass");


}
