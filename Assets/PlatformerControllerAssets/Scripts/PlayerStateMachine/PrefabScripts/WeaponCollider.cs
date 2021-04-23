using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour {

    [SerializeField] SO_WeaponData weaponData;

    private void OnTriggerEnter2D(Collider2D other) {

        Enemy1 enemy = other.gameObject.GetComponentInParent<Enemy1>();
        if (enemy != null) enemy.enemyDelegate += DisplayEnemyInteracted;

        other.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(weaponData.meleeAttackDamage);

    }

    private void OnTriggerExit2D(Collider2D other) {

        Enemy1 enemy = other.gameObject.GetComponentInParent<Enemy1>();
        if (enemy != null) enemy.enemyDelegate -= DisplayEnemyInteracted;
    }

    void DisplayEnemyInteracted(Enemy1 enemy) => Debug.Log($"{enemy.EnemyName}");


}
