using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This basically is in charge of all the player related checks
/// for the ninjaroot statemachine. Additionally, it connects to
/// the input system player input gets handle here as well
/// </summary>
public class CharacterController : MonoBehaviour
{
    public TrashcanContainer trashcanContainer;
    public bool hideInTrash;
    public PlayerInput playerInput;

    public PlayerContext ctx = new PlayerContext();

    // debugging collider jazz
    private Vector2 debugCheckCenter;
    private Vector2 debugCheckSize;
    private bool debugTouchingWall;
    private bool debugHasCheck;
    private Vector2 boxCenter;
    private Vector2 boxSize;

    string lastPath;
    JStateMachine machine;
    public NinjaRoot root;

    void Awake()
    {
        // initializes statemachine
        root = new NinjaRoot(null, ctx);
        var builder = new JStateMachineBuilder(root);
        machine = builder.Build();

        ctx.platformTracker.FindPlatformBelow();

        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Locked; 

        // Grab the component automatically
        playerInput = GetComponent<PlayerInput>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(.1f);
        if (hideInTrash) SpawnInRandTrashcan();


        this.ctx.platformTracker.FindPlatformBelow();

        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Player").Enable();
    }

    static string StatePath(JState s)
    {
        return string.Join(" > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        HandleGrounded();
        HandleWallDetection();
        HandlePhaseThruPlatforms();

        ctx.currentPos = ctx.tr.position;

        // for statemachine on update i did fixed time because
        // we using physics so its better to just use fixedUpdate time
        // instead of update
        machine.Tick(Time.fixedDeltaTime);

        // handles the switching of states
        var path = StatePath(machine.Root.Leaf());

        // debugger
        if (path != lastPath)
        {
            Debug.Log($"[State] {path}");
            lastPath = path;
        }
    }

    public Platform CurrentPlatform
    {
        get
        {
            return this.ctx.platformTracker.CurrentPlatform;
        }
    }

    /// <summary>
    /// Spawns character in a random trashcan
    /// </summary>
    public void SpawnInRandTrashcan()
    {
        Debug.Log("Started randTrashspawn");
        Trashcan trashcan = trashcanContainer.GetRandomTrashcan();

        ctx.tr.position = trashcan.transform.position;

        ctx.nearestInteractable = trashcan.gameObject;
        
        ICharacterInteractable interactable = ctx.nearestInteractable.GetComponent<ICharacterInteractable>();
        interactable.Interact();
    }

    /// <summary>
    /// Call this from the character controller if you want 
    /// the player to take damage. Make sure the amount is 
    /// always positive and damagePos helps determine the 
    /// direction that the player will be knocked back
    /// </summary>
    public void TakeDamage(float amount, Vector2 damagePos)
    {
        ctx.isDamaged = true;
        ctx.damagePos = damagePos;
        ctx.ModifyHealth(-amount);
    }

    /// <summary>
    /// HandleSlashActivation/Deactivation is used
    /// as an Animation event in the "Ninja_SneakAttack"
    /// Animation
    /// </summary>
    public void HandleSlashActivation()
    {
        Debug.Log("Activated slash go");
        ctx.slashGO.SetActive(true);
    }

    public void HandleSlashDeactivation()
    {
        Debug.Log("Deactivated slash go");
        ctx.slashGO.SetActive(false);
    }
    
    /// <summary>
    /// if is on ground... its pretty self explainatory
    /// </summary>
    private void HandleGrounded()
    {
        if (ctx.collider2d == null) return;

        // makes box collider on bottom of character
        Bounds charBounds = ctx.collider2d.bounds;
        boxSize = new Vector2(charBounds.size.x * 0.9f, 0.1f);
        boxCenter = new Vector2(charBounds.center.x, charBounds.min.y - (0.1f / 2f));

        // if box hits a ground layer and is a valid platform then set grounded == true
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, ctx.groundLayer);
        ctx.isGrounded = hit != null && (hit.gameObject.CompareTag(ctx.groundTag) || hit.gameObject.CompareTag(ctx.phaseThruPlatform));
    }

    /// <summary>
    /// makes a box collider in the direction player is going
    /// and checks if touching wall.
    /// </summary>
    private void HandleWallDetection()
    {
        
        Bounds charBounds = ctx.collider2d.bounds;
        float direction = ctx.moveInput.x > 0 ? 1f : (ctx.moveInput.x < 0 ? -1f : 0f);

        if (direction == 0f)
        {
            ctx.isTouchingWall = false;
            debugHasCheck = false;
            return;
        }

        // makes a box collider that is in front of the character collider
        Vector2 checkCenter = new Vector2(charBounds.center.x + direction * (charBounds.extents.x + 0.1f), charBounds.center.y);
        Vector2 checkSize = new Vector2(0.15f, charBounds.size.y * 0.9f);
        Collider2D hit = Physics2D.OverlapBox(checkCenter, checkSize, 0f, ctx.wallLayer);

        // toggles true if touching wall
        ctx.isTouchingWall = hit != null && hit.gameObject.CompareTag(ctx.wallTag);

        if (ctx.isTouchingWall)
        {
            ctx.wallDirection = direction;
        }

        debugCheckCenter = checkCenter;
        debugCheckSize = checkSize;
        debugTouchingWall = ctx.isTouchingWall;
        debugHasCheck = true;
    }

    /// <summary>
    /// handles all the jump through platforms by checking all
    /// nearby platforms and dermining if character should ignore 
    /// collision depending on above or below etc
    /// </summary>
    private void HandlePhaseThruPlatforms()
    {
        // makes a box that gets all of the Colliders that overlaps it
        // the y bounds of the box scales with how fast the player is going down
        Bounds charBounds = ctx.collider2d.bounds;
        float scaledYSize = Mathf.Abs(ctx.rb.linearVelocity.y) * Time.fixedDeltaTime;
        Vector2 boundSize = new Vector2(charBounds.size.x, charBounds.size.y + scaledYSize * 2f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(charBounds.center, boundSize, 0);

        foreach (Collider2D hit in hits)
        {
            // if you can't phase through it then skip
            if (!hit.gameObject.CompareTag(ctx.phaseThruPlatform) && !hit.gameObject.CompareTag("Phased")) continue;

            float characterBottom = charBounds.min.y;
            float platformTop = hit.bounds.max.y;
            bool isDown = ctx.moveInput.y < 0;

            // if the character is above the platform check if player
            // is pressing the down key to see if you can pass through
            // if the character is below the platform automatically sets
            // passing through as true
            bool passesThru = characterBottom >= platformTop - 0.05f ? isDown : true;

            if (passesThru)
            {
                hit.gameObject.tag = "Phased";
            }
            else
            {
                hit.gameObject.tag = ctx.phaseThruPlatform;
            } 
            Physics2D.IgnoreCollision(ctx.collider2d, hit, passesThru);
        }
    }

    public void HandleHiding()
    {
        ctx.isHidden = true;
    }
    
    /// <summary>
    /// Debugging
    /// </summary>
    private void OnDrawGizmos()
    {
        if (debugHasCheck)
        {
            Gizmos.color = debugTouchingWall ? Color.green : Color.red;
            Gizmos.DrawWireCube(debugCheckCenter, debugCheckSize);
        }

        if (ctx.collider2d != null)
        {
            Gizmos.color = ctx.isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }

    #region input system methods
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && ctx.avaliableJumps > ctx.jumpCount) ctx.pressedJump = true;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        ctx.moveInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && !ctx.isHidden && Time.time >= ctx.nextTimeReady) ctx.pressedDash = true;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (ctx.isHidden && context.started && Time.time >= ctx.attackNextTimeReady) ctx.isAttacking = true;
        else if (!ctx.isHidden && ctx.nearestInteractable != null && context.started)
        {
            ctx.dashTrail.enabled = true;
            ICharacterInteractable interactable = ctx.nearestInteractable.GetComponent<ICharacterInteractable>();

            interactable.Interact();
        }
    }
    #endregion
}

/// <summary>
/// Context for the ninja statemachine
/// This thing holds all the info the the ninja might need
/// you can edit this in inspector
/// </summary>
[Serializable]
public class PlayerContext
{
    public Rigidbody2D rb;
    public Transform tr;
    public GameObject modelGo;
    public SpriteRenderer sr;
    public Collider2D collider2d;
    [SerializeField] public PlatformTracker platformTracker;
    [Header("Player Health Settings")]
    public float health = 100f;
    public float maxHealth = 100f;
    public Slider healthSlider;
    [Header("Player Movement Settings")]
    public float speed = 8f;
    public float startAcceleration = 60f;
    public float runningAcceleration = 60f;
    public float deceleration = 40f;
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float airJumpForce = 5f;
    public byte avaliableJumps = 2;
    [Header("Dash Settings")]
    public float dashForce = 60f;
    public byte dashCooldown = 1;
    public float dashDuration = 0.15f;
    public TrailRenderer dashTrail;
    [Header("Hiding Setting")]
    public float hideTime;
    public float tickDamage;
    public float tickRate;
    public GameObject hidingWarning;
    [Header("Wall Settings")]
    public float wallSlideSpeed = 1.5f;
    public float wallJumpForce = 1.5f;
    public float wallJumpTime = 1f;
    public float rightSideOffset = .5f;
    public float leftSideOffset = 1f;
    [Header("Sneak Attack Settings")]
    public float initialSneakAttackCooldown = 7f;
    public float sneakAttackCooldown = 0;
    public float cooldownRefundPerKill = 1f;
    public float minsneakAttackCooldown = 2f;
    public float sneakAttackDuration = 0.30f;
    [Header("Take Damage Settings")]
    public float damagedDuration = 0.3f;
    public float knockbackStrenth = 0.4f;
    public Vector2 damagePos;
    public GameObject slashGO;
    [Header("Collision & Layers Settings")]
    public string groundTag = "Platform";
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public string phaseThruPlatform = "JumpThruPlatform";
    public string wallTag = "Wall";
    [Header("Live States")]
    [HideInInspector]public bool pressedJump = false;
    [HideInInspector]public bool pressedDash = false;
    [HideInInspector]public Vector2 moveInput;
    [HideInInspector]public Vector2 currentPos;
    [HideInInspector]public float nextTimeReady = 0f;
    [HideInInspector]public float attackNextTimeReady = 0f;
    [HideInInspector]public bool isGrounded = false;
    [HideInInspector]public bool isTouchingWall = false;
    [HideInInspector]public float wallDirection = 0f;
    [HideInInspector]public byte jumpCount;
    [HideInInspector]public bool isRight;
    [HideInInspector]public float previousWallDirection = 0f;
    [HideInInspector]public bool isHidden = false;
    [HideInInspector]public bool isAttacking = false;
    [HideInInspector]public bool isDamaged = false;
    [HideInInspector]public bool isDead = false;
    [HideInInspector]public GameObject nearestInteractable;
    public int enemyKillCombo;
    [Header("Animations")]
    public Animator animator;
    public string idle = "Ninja_Idle";
    public string runningL = "Ninja_Running_Left";
    public string runningR = "Ninja_Running_Right";
    public string jumpingL = "Ninja_Jump_Left";
    public string jumpingR = "Ninja_Jump_Right";
    public string fallingL = "Ninja_Fall_Left";
    public string fallingR = "Ninja_Fall_Right";
    public string dashL = "Ninja_Dash_Left";
    public string dashR = "Ninja_Dash_Right";
    public string wsL = "Ninja_WS_L";
    public string wsR = "Ninja_WS_R";
    public string sneakAttack = "Ninja_SneakAttack";
    public string ninjaDamaged = "Ninja_Damaged";
    public string ninjaDeath = "Ninja_Death";
    public string currentAnimState;
    [Header("Audio")]
    public AudioClip swordSlash;
    public AudioClip trashRussling;
    public AudioClip explosion;

    public void ChangeAnimationState(string newState, bool canPlayAgain)
    {
        if (currentAnimState == newState && canPlayAgain == false) return;

        animator.Play(newState,0,0f);
        currentAnimState = newState;
    }

    public void FlipCharacterRight(bool flip)
    {
        isRight = flip;
        sr.flipX = flip;
    }

    public void ModifyHealth(float amount)
    {
        Debug.Log($"modifing Health by {amount}");
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        if (health == 0) isDead = true;
    }
}