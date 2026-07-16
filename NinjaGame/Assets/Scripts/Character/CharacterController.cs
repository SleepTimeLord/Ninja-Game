using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// This basically is in charge of all the player related checks
/// for the ninjaroot statemachine. Additionally, it connects to
/// the input system player input gets handle here as well
/// </summary>
public class CharacterController : MonoBehaviour
{
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
    JState root;

    void Awake()
    {
        ctx.rb = GetComponent<Rigidbody2D>();
        ctx.tr = transform;
        ctx.collider2d = GetComponent<Collider2D>();

        // initializes statemachine
        root = new NinjaRoot(null, ctx);
        var builder = new JStateMachineBuilder(root);
        machine = builder.Build();
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
        IsGrounded();
        DetectWall();
        HandlePhaseThruPlatforms();

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
    /// <summary>
    /// if is on ground... its pretty self explainatory
    /// </summary>
    private void IsGrounded()
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
    private void DetectWall()
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
        Collider2D hit = Physics2D.OverlapBox(checkCenter, checkSize, 0);

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
        if (context.started) ctx.pressedJump = true;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        ctx.moveInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started) ctx.pressedDash = true;
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
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Transform tr;
    public GameObject modelGo;
    public SpriteRenderer sr;
    [HideInInspector] public Collider2D collider2d;
    public Animator animator;

    [Header("Player Movement Settings")]
    public float speed = 8f;
    public float startAcceleration = 60f;
    public float runningAcceleration = 60f;
    public float deceleration = 40f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float airJumpForce = 5f;
    public byte avaliableJumps = 2;
    public bool pressedJump = false;

    [Header("Dash Settings")]
    public float dashForce = 60f;
    public byte dashCooldown = 1;
    public float dashDuration = 0.15f;
    public bool pressedDash = false;

    [Header("Wall Settings")]
    public float wallSlideSpeed = 1.5f;
    public float wallJumpForce = 1.5f;
    public float wallJumpTime = 1f;
    public float rightSideOffset = .5f;
    public float leftSideOffset = 1f;

    [Header("Collision & Layers Settings")]
    public string groundTag = "Platform";
    public LayerMask groundLayer;
    public string phaseThruPlatform = "JumpThruPlatform";
    public string wallTag = "Wall";

    [Header("Live States")]
    public Vector2 moveInput;
    public float nextTimeReady = 0f;
    public bool isGrounded = false;
    public bool isTouchingWall = false;
    public float wallDirection = 0f;
    public byte jumpCount;
    public bool isRight;
    public float previousWallDirection = 0f;
    [Header("Animations")]
    public string currentAnimState;
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

    public void ChangeAnimationState(string newState, bool canPlayAgain)
    {
        if (currentAnimState == newState && canPlayAgain == false) return;

        animator.Play(newState,0,0f);
        currentAnimState = newState;
    }

    public void FlipCharacter(bool flip)
    {
        isRight = flip;
        sr.flipX = flip;
    }
}