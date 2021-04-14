using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    protected PlayerAttackState state;
    [SerializeField] private SO_WeaponData weaponData;

    protected Animator baseAnimator;
    protected Animator weaponAnimator;

    protected int attackCounter;
    private float timeSinceFirstAttack;

    protected virtual void Start() {
        baseAnimator = transform.Find("Base").GetComponent<Animator>();
        weaponAnimator = transform.Find("Weapon").GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    public virtual void EnterWeapon() {
        gameObject.SetActive(true);

        // Resets attack counter after final attack sequence reached, Reset counter once time since first attack passes the reset time limit
        if (attackCounter >= weaponData.movementSpeed.Length || Time.time - timeSinceFirstAttack >= weaponData.resetTime) {
            attackCounter = 0;
        }

        // Keep track of time when first attack is used
        if (attackCounter == 0) timeSinceFirstAttack = Time.time;

        baseAnimator.SetBool("attack", true);
        weaponAnimator.SetBool("attack", true);

        baseAnimator.SetInteger("attackCounter", attackCounter);
        weaponAnimator.SetInteger("attackCounter", attackCounter);
    }
    public virtual void ExitWeapon() {
        baseAnimator.SetBool("attack", false);
        weaponAnimator.SetBool("attack", false);

        attackCounter++;

        gameObject.SetActive(false);
    }

    #region Animation Triggers

    public virtual void AnimationFinishTrigger() => state.AnimationFinishTrigger();
    public virtual void AnimationStartMovementTrigger() => state.SetPlayerVelocity(weaponData.movementSpeed[attackCounter]);
    public virtual void AnimationStopMovementTrigger() => state.SetPlayerVelocity(0);

    public virtual void AnimationTurnOffFlipTrigger() => state.SetFlipCheck(false);
    public virtual void AnimationTurnOnFlipTrigger() => state.SetFlipCheck(true);

    #endregion

    public void InitializeWeapon(PlayerAttackState state) => this.state = state;
}
