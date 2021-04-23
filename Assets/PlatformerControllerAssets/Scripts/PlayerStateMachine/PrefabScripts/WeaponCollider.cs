using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour {

    [SerializeField] SO_WeaponData weaponData;

    private void OnTriggerEnter2D(Collider2D other) {

        other.gameObject.GetComponentInParent<Enemy1>()?.AddDelegate(DisplayEnemyInteracted);

        other.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(weaponData.meleeAttackDamage);
    }

    void DisplayEnemyInteracted(Enemy1 enemy) => Debug.Log($"{enemy.EnemyName} dead");

}
