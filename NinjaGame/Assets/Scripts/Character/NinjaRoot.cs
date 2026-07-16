using UnityEngine;
using System;
// statmachine for the ninja


/// <summary>
/// Move state is a child of Ground and Airborne
/// this handles the movement in the air and on the ground
/// using a phyiscs based movement system
/// </summary>
public class Move : JState
{
    readonly PlayerContext ctx;

    public Move(JStateMachine m, JState parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override JState GetTransition()
    {
        // matched the threshold to 0.01f so you don't get trapped between states
        if (Mathf.Abs(ctx.moveInput.x) <= 0.01f)
        {
            if (Parent is Grounded g) return g.Idle;
            if (Parent is Airborne a) return a.Idle;
        }
        return null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        // check if just started moving or is mid motion
        float currentAccel = (Mathf.Abs(ctx.rb.linearVelocity.x) < 0.1f) ? ctx.startAcceleration : ctx.runningAcceleration;

        // check if right or left and flip
        if (ctx.moveInput.x > 0.01f || ctx.rb.linearVelocity.x > 0.1f)
        {
            ctx.FlipCharacter(true);
        }
        else if (ctx.moveInput.x < -0.01f || ctx.rb.linearVelocity.x < -0.1f)
        {
            ctx.FlipCharacter(false);
        }

        // play animations for ground and air
        if (Parent is Grounded)
        {
            ctx.ChangeAnimationState(ctx.isRight ? ctx.runningR : ctx.runningL, false);
        }
        else if (Parent is Airborne)
        {
            if (ctx.rb.linearVelocity.y < 0f)
                ctx.ChangeAnimationState(ctx.isRight ? ctx.fallingR : ctx.fallingL, false);
            else
                ctx.ChangeAnimationState(ctx.isRight ? ctx.jumpingR : ctx.jumpingL, false);
        }

        // math for movement
        if (Mathf.Abs(ctx.moveInput.x) > 0.01f)
        {
            // checks if below the intended speed or if its not going in the right direction that the player is inputting
            // if any of these are true it moves the player
            if (Mathf.Abs(ctx.rb.linearVelocity.x) < ctx.speed || Mathf.Sign(ctx.moveInput.x) != Mathf.Sign(ctx.rb.linearVelocity.x))
            {
                ctx.rb.AddForce(Vector2.right * ctx.moveInput.x * currentAccel, ForceMode2D.Force);
            }
        }
        // if not moving and on the ground it slowly stops the player to 0
        // this basically simulates friction on the ground so yea
        else if (ctx.isGrounded)
        {
            float slowdown = Mathf.MoveTowards(ctx.rb.linearVelocity.x, 0, ctx.deceleration * Time.fixedDeltaTime);
            ctx.rb.linearVelocity = new Vector2(slowdown, ctx.rb.linearVelocity.y);
        }
    }
}

/// <summary>
/// Idle state is a child of Ground and Airborne
/// this handles what the character should do when not
/// inputting any buttons
/// </summary>
public class Idle : JState
{
    readonly PlayerContext ctx;

    public Idle(JStateMachine m, JState parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override JState GetTransition()
    {
        // if player inputs something then move
        if (Mathf.Abs(ctx.moveInput.x) > 0.01f)
        {
            if (Parent is Grounded g) return g.Move;
            if (Parent is Airborne a) return a.Move;
        }
        return null;
    }

    protected override void OnEnter()
    {
        // stops the player horizontally when entering idle
        ctx.rb.linearVelocity = new Vector2(0f, ctx.rb.linearVelocity.y);

        // when not moving on the ground make sure the character is always
        // facing left to make it accurate to the other art and make it so
        // he is in the idle state
        if (Parent is Grounded) ctx.FlipCharacter(false); ctx.ChangeAnimationState(ctx.idle, false);
    }

    protected override void OnUpdate(float deltaTime)
    {
        // if in air play falling on jumping animation if falling or jump ofc
        if (Parent is Airborne)
        {
            if (ctx.rb.linearVelocity.y < 0f)
            {
                ctx.ChangeAnimationState(ctx.isRight ? ctx.fallingR : ctx.fallingL, false);
            }
            else
            {
                ctx.ChangeAnimationState(ctx.isRight ? ctx.jumpingR : ctx.jumpingL, false);
            }
        }
    }
}

/// <summary>
/// Grounded states handles everything that the player can do
/// on the ground. It has two children which is Idle and Move.
/// </summary>
public class Grounded : JState
{
    readonly PlayerContext ctx;
    public readonly Idle Idle;
    public readonly Move Move;

    public Grounded(JStateMachine m, JState parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Idle = new Idle(m, this, ctx);
        Move = new Move(m, this, ctx);
    }

    protected override JState GetInitialState()
    {
        // check input to land smoothly into a run
        return Mathf.Abs(ctx.moveInput.x) > 0.01f ? Move : Idle;
    }

    protected override void OnEnter()
    {
        // resets jump when on ground
        ctx.jumpCount = 0;
    }

    protected override JState GetTransition()
    {
        // handles jump and if he did jump then go to the airborne state
        bool wantsJump = ctx.pressedJump;
        if (wantsJump) ctx.pressedJump = false;

        if (wantsJump && ctx.jumpCount < ctx.avaliableJumps)
        {
            ctx.rb.linearVelocity = new Vector2(ctx.rb.linearVelocity.x, 0f);
            ctx.rb.AddForce(Vector2.up * ctx.jumpForce, ForceMode2D.Impulse);
            ctx.jumpCount++;
            return ((NinjaRoot)Parent).Airborne;
        }

        return null;
    }
}

/// <summary>
/// Airborne states handles everything that the player can do
/// in the air. It has two children which is Idle and Move.
/// </summary>
public class Airborne : JState
{
    readonly PlayerContext ctx;
    public readonly Idle Idle;
    public readonly Move Move;

    public Airborne(JStateMachine m, JState parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Idle = new Idle(m, this, ctx);
        Move = new Move(m, this, ctx);
    }

    protected override JState GetInitialState()
    {
        // check input to land smoothly into a run
        return Mathf.Abs(ctx.moveInput.x) > 0.01f ? Move : Idle;
    }

    protected override JState GetTransition()
    {
        // handles the double jump and uses the airJumpForce instead of the normal jump force
        bool wantsJump = ctx.pressedJump;
        if (wantsJump) ctx.pressedJump = false;

        if (wantsJump && ctx.jumpCount < ctx.avaliableJumps)
        {
            ctx.ChangeAnimationState(ctx.isRight ? ctx.jumpingR : ctx.jumpingL, true);
            ctx.rb.linearVelocity = new Vector2(ctx.rb.linearVelocity.x, 0f);
            ctx.rb.AddForce(Vector2.up * ctx.airJumpForce, ForceMode2D.Impulse);
            ctx.jumpCount++;
        }
        return null;
    }
}

/// <summary>
/// WallJump state makes you jump from a wall this state
/// canonly be accessed through the wall cling state 
/// has timer so for wall jump so the player can actually 
/// notice the walljump and not just move and nullify it
/// </summary>
public class WallJump : JState
{
    readonly PlayerContext ctx;
    float jumpEndTime;

    public WallJump(JStateMachine m, JState parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override void OnEnter()
    {
        // calculate direction, change animation for jump, and apply force
        Vector2 jumpDir = new Vector2(-ctx.wallDirection, 1f).normalized;
        ctx.ChangeAnimationState(ctx.wallDirection < 0 ? ctx.jumpingR : ctx.jumpingL, false);
        ctx.rb.linearVelocity = Vector2.zero; // reset velocity before jumping
        ctx.rb.AddForce(jumpDir * ctx.wallJumpForce, ForceMode2D.Impulse);
        
        ctx.jumpCount = 1;
        
        jumpEndTime = Time.time + ctx.wallJumpTime;
    
        // ctx.ChangeAnimationState(ctx.isRight ? ctx.jumpingR : ctx.jumpingL, true);
    }

    protected override JState GetTransition()
    {
        // keep them trapped in this state until the timer ends
        if (Time.time < jumpEndTime) return null;

        // once the timer ends goes to grounded state or airborne
        return ctx.isGrounded ? ((NinjaRoot)Parent).Grounded : ((NinjaRoot)Parent).Airborne;
    }
}

/// <summary>
/// WallCling as the name states, makes you 
/// cling on to walls and slowly slide down
/// </summary>
public class WallCling : JState
{
    readonly PlayerContext ctx;

    public WallCling(JStateMachine m, JState parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override void OnEnter()
    {
        // freeze x position so doesnt move from the wall and 
        // changes the animation and flips the character depending on
        // which side you are on
        ctx.rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        ctx.ChangeAnimationState(ctx.wallDirection > 0 ? ctx.wsR : ctx.wsL, false);
        ctx.FlipCharacter(ctx.wallDirection > 0 ? false : true);

        // handles the offset when wall climbing as regular on offset looks weird
        // you can edit this in the PlayerContext
        ctx.modelGo.transform.localPosition = new Vector3(ctx.isRight ? ctx.rightSideOffset : -ctx.leftSideOffset ,0f,0f);
    }

    protected override void OnExit()
    {
        // unfreezes so the player can move and resets the model
        ctx.rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        ctx.modelGo.transform.localPosition = Vector3.zero;
        //ctx.previousWallDirection = ctx.wallDirection;
    }

    protected override JState GetTransition()
    {
        // goes to the walljump state if pressedJump
        if (ctx.pressedJump)
        {
            ctx.pressedJump = false;
            return ((NinjaRoot)Parent).WallJump; 
        }

        // goes to are state if not touching wall
        if (!ctx.isTouchingWall) 
        {
            return ((NinjaRoot)Parent).Airborne;
        }

        return null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        // handles slow fall
        if (ctx.rb.linearVelocity.y < -ctx.wallSlideSpeed)
        {
            ctx.rb.linearVelocity = new Vector2(ctx.rb.linearVelocity.x, -ctx.wallSlideSpeed);
        }
    }
}

/// <summary>
/// Dash state makes you dash a certain amount of time
/// you override this state and the dash must be finished
/// </summary>
public class Dash : JState
{
    readonly PlayerContext ctx;
    float dashEndTime;

    public Dash(JStateMachine m, JState parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override void OnEnter()
    {
        // if the player is pressing the direction they want to go then it goes to the
        // direction that the player is moving (1 == right, -1 left) if the player is
        // inputting nothing then it goes in the direction that the character is facing
        float dir = Mathf.Abs(ctx.moveInput.x) > 0.01f ? Mathf.Sign(ctx.moveInput.x) : (ctx.sr.flipX ? 1f : -1f);

        ctx.rb.constraints |= RigidbodyConstraints2D.FreezePositionY;

        // handles the dashing and changes to do a dash animation
        ctx.rb.linearVelocity = new Vector2(0f, ctx.rb.linearVelocity.y);
        ctx.rb.AddForce(new Vector2(dir * ctx.dashForce, 0), ForceMode2D.Impulse);

        ctx.ChangeAnimationState(dir > 0 ? ctx.dashR : ctx.dashL, true);

        // starts the timer of dashduration and cooldown
        ctx.nextTimeReady = Time.time + ctx.dashCooldown;
        dashEndTime = Time.time + ctx.dashDuration;
    }

    protected override void OnExit()
    {
        // unfreeze y pos
        ctx.rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    protected override JState GetTransition()
    {
        // doesnt exit dashstate until dash time is finished
        if (Time.time < dashEndTime) return null;
        return ctx.isGrounded ? ((NinjaRoot)Parent).Grounded : ((NinjaRoot)Parent).Airborne;
    }
}

public class Hidden : JState
{
    public event Action OnPlayerHide;
    public event Action OnPlayerLeave;
    readonly PlayerContext ctx;

    public Hidden(JStateMachine m, JState parent, PlayerContext ctx) : base (m, parent)
    {
        this.ctx = ctx;
    }

    protected override void OnEnter()
    {
        OnPlayerHide?.Invoke();
    }

    protected override void OnExit()
    {
        OnPlayerLeave?.Invoke();
    }

    protected override JState GetTransition()
    {
        return null;
    }
}


/// <summary>
/// NinjaRoot state is the root state and is the parent to
/// the Grounded, Airborne, WallCling, Dash, and WallJump States
/// this state basically is used for handling which state to go to.
/// </summary>
public class NinjaRoot : JState
{
    public readonly Grounded Grounded;
    public readonly Airborne Airborne;
    public readonly Dash Dash;
    public readonly WallCling WallCling;
    public readonly WallJump WallJump;
    readonly PlayerContext ctx;

    public NinjaRoot(JStateMachine m, PlayerContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        Grounded = new Grounded(m, this, ctx);
        Airborne = new Airborne(m, this, ctx);
        Dash = new Dash(m, this, ctx);
        WallCling = new WallCling(m, this, ctx);
        WallJump = new WallJump(m, this, ctx);
    }

    // always start on the ground
    protected override JState GetInitialState() => Grounded;

    protected override JState GetTransition()
    {
        if (ctx.pressedDash)
        {
            ctx.pressedDash = false;
            if (Time.time >= ctx.nextTimeReady) return Dash;
        }

        // maybe implement this later
        bool touchingPrevWall = ctx.isTouchingWall && (ctx.previousWallDirection == ctx.wallDirection);

        // this prevents dashing and walljump to be cancelled or interupted
        if (ActiveChild == Dash || (ActiveChild == WallJump)) return null; 

        // we do ActiveChild != Child so it doesn't keep going in and out
        if (ctx.isGrounded && ActiveChild != Grounded) return Grounded;
        
        if (ctx.isTouchingWall && !ctx.isGrounded && ActiveChild != WallCling) return WallCling;
        
        if (!ctx.isGrounded && !ctx.isTouchingWall && ActiveChild != Airborne) return Airborne;

        return null;
    }
}