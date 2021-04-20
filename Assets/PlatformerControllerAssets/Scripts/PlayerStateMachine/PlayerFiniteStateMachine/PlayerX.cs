using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerX : Singleton<PlayerX> {

    #region Unity Editor Functions
#if UNITY_EDITOR
    void OnGUI() {
        if (!playerData.debugMode) return;

        // Calculate Frame Rate
        var current = (int)(1f / Time.unscaledDeltaTime);
        var avgFrameRate = (int)current;

        // Show Current State 
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 50;
        string state = StateMachine.CurrentState.ToString();

        // Display on Editor Window
        GUI.Label(new Rect(3000, 0, 1000, 100), state, guiStyle);
        GUI.Label(new Rect(3000, 100, 1000, 100), avgFrameRate.ToString(), guiStyle);
        GUI.Label(new Rect(3000, 200, 1000, 100), "Health:" + currentHealth.ToString(), guiStyle);

    }
#endif
    #endregion

    #region Player Info Variables

    public int currentHealth;

    #endregion

    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerShootState ShootState { get; private set; }
    public PlayerHit1State Hit1State { get; private set; }
    public PlayerAttackState PrimaryAttackState { get; private set; }
    public PlayerAttackState SecondaryAttackState { get; private set; }

    [SerializeField] private PlayerData playerData;
    #endregion

    #region Components
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerInventory Inventory { get; private set; }
    public Animator Anim { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public SpriteRenderer SR { get; private set; }
    public Transform DashTimeIndicator { get; private set; }
    public Transform DashDirectionIndicator { get; private set; }
    public Material DashTimeIndicatorMaterial { get; private set; }

    RigidbodyConstraints2D rigidbodyConstraints2D;

    #endregion

    #region Check Transforms

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    private Transform BulletShootPos;

    #endregion

    #region Other Variables
    private Vector2 previousVelocity;
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }
    public GameObject MenuObject;
    bool freezePlayer;
    bool freezeBullets;
    bool freezeInput;

    #endregion

    #region Unity Callback Functions
    private void Awake() {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "inAir");
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeClimbState");
        DashState = new PlayerDashState(this, StateMachine, playerData, "dash");
        ShootState = new PlayerShootState(this, StateMachine, playerData, "shoot");
        Hit1State = new PlayerHit1State(this, StateMachine, playerData, "hit1");
        PrimaryAttackState = new PlayerAttackState(this, StateMachine, playerData, "attack");
        SecondaryAttackState = new PlayerAttackState(this, StateMachine, playerData, "attack");
    }
    private void Start() {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        Inventory = GetComponent<PlayerInventory>();

        DashDirectionIndicator = transform.Find("DashDirectionIndicator");
        DashTimeIndicator = transform.Find("DashTimeIndicator");
        DashTimeIndicatorMaterial = DashTimeIndicator.GetComponent<Renderer>().material;
        BulletShootPos = transform.Find("BulletShootPos");

        currentHealth = playerData.maxHealth;
        FacingDirection = 1;

        PrimaryAttackState.SetWeapon(Inventory.weapons[(int)CombatInputs.primary]);
        // SecondaryAttackState.SetWeapon(Inventory.weapons[(int)CombatInputs.primary]);


        StateMachine.Initialize(IdleState);
    }

    private void Update() {

        CurrentVelocity = RB.velocity;

        PlayerDebugInput();

        StateMachine.CurrentState.LogicUpdate();
    }
    private void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Animation Triggers

    // Used so Animation Events on animations can be used
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    #endregion

    #region Set Functions

    public void SetVelocityToZero() {
        RB.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void SetWallJumpVelocity(float velocity, Vector2 angle, int direction) {
        angle.Normalize();
        previousVelocity.Set(angle.x * velocity * direction, angle.y * velocity);
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }
    public void SetDashVelocity(float velocity, Vector2 direction) {
        previousVelocity = direction * velocity;
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }

    public void SetVelocityX(float velocity) {
        previousVelocity.Set(velocity, CurrentVelocity.y);
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }
    public void SetVelocityY(float velocity) {
        previousVelocity.Set(CurrentVelocity.x, velocity);
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }

    public void SetVelocity(Vector2 velocity) {
        previousVelocity.Set(velocity.x, velocity.y);
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }

    private void Flip() {
        FacingDirection *= -1;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        // transform.Rotate(0.0f, 180.0f, 0.0f);
        // SR.flipX = FacingDirection != 1;
    }
    // This is here because you cant instantiate without monobehavior
    public void ShootBullet() {
        GameObject bullet = Instantiate((GameObject)Resources.Load("Bullet"), BulletShootPos.position, Quaternion.identity);
        // bullet.name = BulletPrefab.name; // Instantiate creates a copy and renames it to clone --  this sets it back for visual convenience
        bullet.GetComponent<BulletScript>().SetDamageValue(playerData.bulletDamage);
        bullet.GetComponent<BulletScript>().SetBulletSpeed(playerData.bulletSpeed);
        bullet.GetComponent<BulletScript>().SetBulletDirection((FacingDirection == 1) ? Vector2.right : Vector2.left);
        bullet.GetComponent<BulletScript>().Shoot();
    }

    #endregion

    #region Check Functions

    public bool CheckIfTouchingWall() => Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    public bool CheckIfTouchingWallBack() => Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    public bool CheckIfGrounded() => Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    public bool CheckIfTouchingLedge() => Physics2D.Raycast(ledgeCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);

    public void CheckIfShouldFlip(int xInput) {
        if (xInput != 0 && xInput != FacingDirection) {
            Flip();
        }
    }
    public Vector2 DetermineCornerPos() {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
        float xDistance = xHit.distance;
        previousVelocity.Set(xDistance * FacingDirection, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)previousVelocity, Vector2.down, ledgeCheck.position.y - wallCheck.position.y, playerData.whatIsGround);
        float yDistance = yHit.distance;

        previousVelocity.Set(wallCheck.position.x + (xDistance * FacingDirection), ledgeCheck.position.y - yDistance);
        Debug.DrawRay(ledgeCheck.position + (Vector3)previousVelocity, Vector2.down, Color.red, playerData.whatIsGround);
        return previousVelocity;
    }

    #endregion

    #region Other Functions

    public void PlayerDebugInput() {

        // TODO: Change all buttons to use New Input System

        if (Input.GetKeyDown(KeyCode.B)) {
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
            if (bullets.Length > 0) {
                freezeBullets = !freezeBullets;
                foreach (GameObject bullet in bullets) {
                    bullet.GetComponent<BulletScript>().FreezeBullet(freezeBullets);
                }
            }
            // TODO: Reference some sort of Dialogue Manager instance and display this to the player on screen
            Debug.Log("Freeze Bullets");
        }

        if (Input.GetKeyDown(KeyCode.I) && !EnemyController.Instance.isInvincible) {
            EnemyController.Instance.isInvincible = true;

            // TODO: Reference some sort of Dialogue Manager instance and display this to the player on screen
            Debug.Log("Invincible Mode Toggle");
        } else if (Input.GetKeyDown(KeyCode.I) && EnemyController.Instance.isInvincible) {
            EnemyController.Instance.isInvincible = false;

            // TODO: Reference some sort of Dialogue Manager instance and display this to the player on screen
            Debug.Log("Invincible Mode Toggle");
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            freezeInput = !freezeInput;
            if (freezeInput) InputHandler.PlayerInput.SwitchCurrentActionMap("Empty");
            if (!freezeInput) InputHandler.PlayerInput.SwitchCurrentActionMap("Gameplay");
        }
        if (Input.GetKeyDown(KeyCode.Equals)) {
            this.Defeat();
            Debug.Log("Defeat");
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            FreezePlayer(!freezePlayer);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MenuObject.SetActive(true);
            InputHandler.PlayerInput.SwitchCurrentActionMap("Menu");
        }
    }
    public void FreezePlayer(bool freeze) {
        if (freeze) {
            freezePlayer = true;
            Anim.speed = 0;
            rigidbodyConstraints2D = RB.constraints;
            RB.constraints = RigidbodyConstraints2D.FreezeAll;
        } else {
            freezePlayer = false;
            Anim.speed = 1;
            RB.constraints = rigidbodyConstraints2D;
            RB.velocity = new Vector2(0, -0.1f);
        }
    }

    #endregion

    #region Player Death
    public void Defeat() {
        GameManager.Instance.PlayerDefeated();
        Invoke("StartDefeatAnimation", 0.5f); // Using function written below
    }
    void StartDefeatAnimation() {
        GameObject explodeEffect = Instantiate(playerData.explodeEffectPrefab);
        explodeEffect.name = playerData.explodeEffectPrefab.name;
        explodeEffect.transform.position = SR.bounds.center;
        SoundManager.Instance.Play(playerData.explodeEffectClip);
        Destroy(gameObject);
    }

    #endregion
}
