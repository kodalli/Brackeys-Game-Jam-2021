﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour{

    public AngleRotations angleRotations;
    private PlayerInput playerInput;
    private Camera cam;

    [SerializeField] private bool DisableInput;

    #region Movement Variables 
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }

    #endregion

    #region Jump Variables
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    [SerializeField] private float inputHoldTime; // Fixes Double Jump from Spamming Spacebar
    private float jumpinputStartTime;

    #endregion

    #region Dash Variables
    
    // Dash Button Variables
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }
    private float dashInputStartTime;

    // ** Dash with Arrow Keys
    public Vector2 DashDirectionKeyboardInput { get; private set; }
    public int DashInputX { get; private set; }
    public int DashInputY { get; private set; }

    #endregion

    #region Shooting Variables
    public bool ShootInput { get; private set; }
    public bool ShootInputStop { get; private set; }
    private float shootInputStartTime;

    #endregion

    #region Unity Callback Functions
    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        cam = Camera.main;
        angleRotations.up = 0f; angleRotations.right = 90f; angleRotations.down = 180f; angleRotations.left = 270f;

        if (DisableInput) DisableInputMode();
    }
    private void Update() {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
        CheckShootInputHoldTime();
    }

    #endregion

    #region Unity PlayerInput Event Functions
    public void OnMoveInput(InputAction.CallbackContext context) {
        RawMovementInput = context.ReadValue<Vector2>();

        NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }
    public void OnJumpInput(InputAction.CallbackContext context) {
        if (context.started) {
            JumpInput = true;
            JumpInputStop = false;
            jumpinputStartTime = Time.time;
        }
        if (context.canceled) {
            JumpInputStop = true;
        }
    }
    public void OnDashInput(InputAction.CallbackContext context) {
        if (context.started) {
            DashInput = true;
            DashInputStop = false;
            dashInputStartTime = Time.time;
        } else if (context.canceled) {
            DashInputStop = true;
        }
    }
    public void OnDashDirectionKeyboardInput(InputAction.CallbackContext context) {
        DashDirectionKeyboardInput = context.ReadValue<Vector2>();
        DashInputX = (int)(DashDirectionKeyboardInput * Vector2.right).normalized.x;
        DashInputY = (int)(DashDirectionKeyboardInput * Vector2.up).normalized.y;

    }
    public void OnShootInput(InputAction.CallbackContext context) {
        if (context.started) {
            ShootInput = true;
            ShootInputStop = false;
            shootInputStartTime = Time.time;
        } else if (context.canceled) {
            ShootInputStop = true;
        }
    }

    #endregion

    #region Other Functions
    public void UseJumpInput() => JumpInput = false;
    public void UseDashInput() => DashInput = false;
    public void UseShootInput() => ShootInput = false;
    private void CheckJumpInputHoldTime() {
        if(Time.time >= jumpinputStartTime + inputHoldTime) {
            JumpInput = false;
        }
    }
    private void CheckDashInputHoldTime() {
        if(Time.time >= dashInputStartTime + inputHoldTime) {
            DashInput = false;  
        }
    }
    private void CheckShootInputHoldTime() {
        if (Time.time >= shootInputStartTime + inputHoldTime) {
            ShootInput = false;
        }
    }

    private void DisableInputMode() {
        playerInput.currentActionMap.Disable();
    }

    #endregion

    #region Structs
    public struct AngleRotations {
        public float up, down, left, right;
    }

    #endregion

}
