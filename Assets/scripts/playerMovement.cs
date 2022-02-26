using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    //Layer Masks
    [Header("Collider settings")]
    [SerializeField]
    Vector3 gBoxSize;

    [SerializeField]
    Vector3 gBoxPos;

    [SerializeField]
    Vector3 wallBoxSize;

    [SerializeField]
    Vector3 wallBoxPos;

    [Header("Walljump settings")]
    [SerializeField]
    LayerMask lmWallStick;

    [SerializeField]
    float wallJumpColliderLen;

    [SerializeField]
    float normalGrav = 1.3f;

    [SerializeField]
    float slidingGrav = 0.5f;

    [SerializeField]
    float wallJumpVel;

    [SerializeField]
    float neutralWallJumpDistance;

    [SerializeField]
    float wallJumpDistance;

    [SerializeField]
    float superWallJumpAccelDampening;

    [SerializeField]
    float moveLockTimerAfterWallJump = 0.5f;

    [SerializeField]
    float moveLockTimerAfterNeutralJump = 0.3f;

    [Header("acceleration stuff")]
    // Settings
    [SerializeField]
    float groundedAcceleration = 0;

    [SerializeField]
    float airAcceleration = 0;

    [SerializeField]
    float speedcap = 0;

    [SerializeField]
    float airspeedcap = 0;

    [SerializeField]
    float deccelerate = 0;

    [SerializeField]
    float airdeccelerate = 0;
    [SerializeField]
    float maxFallSpeed = 0;
    [SerializeField]
    float maxSlideSpeed = 0;

    [SerializeField]
    float jumpVel;

    [SerializeField]
    float directionChangeDampening;

    [SerializeField]
    float dashSpeed;

    [SerializeField]
    float dashEndDampening = 0.3f;

    [SerializeField]
    float dashEndTime = 0.25f;

    [SerializeField]
    [Range(0, 1)]
    float cutJumpHeight = 0.5f;

    [SerializeField]
    LayerMask lmWalls;

    [SerializeField]
    ContactFilter2D filterWalls;

    [Header("Animation Settings")]
    [SerializeField]
    Color noDashesColor;

    [SerializeField]
    Color normalDashesColor;

    [SerializeField]
    Color maxDashesColor;

    [SerializeField]
    GameObject sprite;

    [SerializeField]
    GameObject sprite_1;

    GameObject activeSprite;

    [Header("Coyote Time / HyperGrace settings")]
    [SerializeField]
    float jumpHangGraceTicksMax = 10;

    [SerializeField]
    float groundRegisterGraceTicksMax = 10;

    [SerializeField]
    float jumpCooldownMax = 10;

    float jumpCooldown = 0;

    float jumpHangGraceTicks = 0;

    float groundRegisterGraceTicks = 0;

    [Header("Misc settings")]
    [SerializeField]
    bool showTrail = false;

    [SerializeField]
    GameObject trailPrefab;

    [SerializeField]
    float trailFrequency = 0.3f;

    public int maxDashCount = 1;

    public int dashes = 0;

    int dir = 0;

    bool dashAvailable = true;

    bool haltMomentum = false;

    public bool dashing = false;

    bool wallSliding = false;

    bool wallJumpedLeft = false;

    bool wallJumpedRight;

    bool LArrowPressed = false;

    bool RArrowPressed = false;

    bool UArrowPressed = false;

    bool DArrowPressed = false;

    bool ZPressed = false;

    bool ZUnpressed = false;

    bool XPressed = false;

    bool XUnpressed = false;

    bool preserveMomentum = false;

    [HideInInspector]
    public bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetSprite(sprite);
        InvokeRepeating("DrawTrail", 1f, trailFrequency);
    }

    private void OnDrawGizmos()
    {
        // Gizmos
        //     .DrawWireCube((Vector3) wallBoxPos + transform.position,
        //     wallBoxSize);
    }

    private void FixedUpdate()
    {
        jumpHangGraceTicks -= 1;
        groundRegisterGraceTicks -= 1;
        jumpCooldown -= 1;


        float horizontal = 0;
        if (LArrowPressed)
        {
            horizontal--;
            if (dir != -1)
            {
                dir = -1;
                sprite.GetComponent<SpriteRenderer>().flipX = true;
                sprite_1.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        if (RArrowPressed)
        {
            horizontal++;
            if (dir != 1)
            {
                dir = 1;
                sprite.GetComponent<SpriteRenderer>().flipX = false;
                sprite_1.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        Vector2 GBoxColliderPos =
            (Vector2)transform.position + (Vector2)gBoxPos;
        Vector2 GBoxColliderScale = (Vector2)gBoxSize;
        Collider2D[] results = new Collider2D[0];

        isGrounded = false;
        results =
            Physics2D
                .OverlapBoxAll(GBoxColliderPos, GBoxColliderScale, 0, lmWalls);
        foreach (Collider2D result in results)
        {
            if (!result.isTrigger)
            {
                isGrounded = true;
            }
        }

        Vector2 WallStickColliderPos =
            (Vector2)transform.position + new Vector2(0, 0.05f);
        Vector2 WallStickColliderScale =
            new Vector2(wallJumpColliderLen, -0.2f);
        bool isWallStuck = false;
        results =
            Physics2D
                .OverlapBoxAll(WallStickColliderPos,
                WallStickColliderScale,
                0,
                lmWallStick);
        foreach (Collider2D result in results)
        {
            if (!result.isTrigger)
            {
                isWallStuck = true;

            }
        }

        float xvel = rb.velocity.x;
        float yvel = rb.velocity.y;

        float newXvel = xvel;
        float newYvel = yvel;

        // if (rb.velocity.y > maxFallSpeed)
        // {
        //     newYvel = maxFallSpeed;
        // }
        if (rb.velocity.y < -maxFallSpeed)
        {
            newYvel = -maxFallSpeed;
        }
        if (rb.velocity.y < -maxSlideSpeed && wallSliding)
        {
            newYvel = -maxSlideSpeed;
        }

        if (isGrounded)
        {
            if (dashes < maxDashCount)
            {
                dashes = maxDashCount;
            }
            if (jumpCooldown < 0)
            {
                groundRegisterGraceTicks = groundRegisterGraceTicksMax;
            }
        }

        if (ZPressed)
        {
            jumpHangGraceTicks = jumpHangGraceTicksMax;
        }

        // jump code
        if (jumpHangGraceTicks > 0 && groundRegisterGraceTicks > 0 && !wallSliding)
        {
            //This is bad, fix later
            newYvel = jumpVel;
            jumpHangGraceTicks = -1;
            groundRegisterGraceTicks = -1;
            jumpCooldown = jumpCooldownMax;
            haltMomentum = false;
            if (dashing)
            {
                preserveMomentum = true;
            }
            isGrounded = false;
        }

        if (dashAvailable)
        {
            // X movement
            if (horizontal != 0)
            {
                if (
                    (horizontal < 0 && wallJumpedRight) ||
                    (horizontal > 0 && wallJumpedLeft)
                )
                {
                    horizontal *= superWallJumpAccelDampening;
                }

                if (
                    !(horizontal > 0 && wallJumpedLeft) &&
                    !(horizontal < 0 && wallJumpedRight)
                )
                {
                    // Player is accelerating
                    if (isGrounded)
                    {
                        if (Mathf.Abs(xvel) < speedcap)
                        {
                            // Use grounded acceleration
                            newXvel += horizontal * groundedAcceleration;
                        }
                        if (Mathf.Sign(xvel) != horizontal)
                        {
                            // Direction Change dampening
                            newXvel -= xvel / directionChangeDampening;
                            newXvel += horizontal * groundedAcceleration * 2;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(xvel) < airspeedcap)
                        {
                            // Use grounded acceleration
                            newXvel += horizontal * airAcceleration;
                        }
                        if (Mathf.Sign(xvel) != horizontal)
                        {
                            // Direction Change dampening
                            newXvel -= xvel / directionChangeDampening;
                            newXvel += horizontal * airAcceleration * 2;
                        }
                    }
                }
            }
            else
            {
                if (!wallJumpedLeft && !wallJumpedRight)
                {
                    // Player is not pressing arrow key
                    if (isGrounded)
                    {
                        // Apply ground friction
                        newXvel -= xvel / deccelerate;
                    }
                    else
                    {
                        newXvel -= xvel / airdeccelerate;
                    }
                }
            }
        }

        if (XPressed)
        {
            if (dashes > 0 && dashAvailable)
            {
                dashes--;
                NewCameraController.i.Shake(0.1f, 1, 3);

                isGrounded = false;
                Vector2 dashDir = new Vector2(0, 0);

                if (LArrowPressed)
                {
                    if (!RArrowPressed)
                    {
                        dashDir.x = -1;
                    }
                }
                else if (RArrowPressed)
                {
                    dashDir.x = 1;
                }
                else if (!DArrowPressed && !UArrowPressed)
                {
                    dashDir.x = dir;
                }
                if (DArrowPressed)
                {
                    if (!UArrowPressed)
                    {
                        dashDir.y = -1;
                    }
                }
                else if (UArrowPressed)
                {
                    dashDir.y = 1;
                }

                Vector2 dashVect = dashDir * dashSpeed;

                newXvel = dashVect.x;
                newYvel = dashVect.y;
                dashAvailable = false;

                dashing = true;
                Invoke("restoreDash", dashEndTime);
            }
        }
        if (ZUnpressed)
        {
            if (rb.velocity.y > 0)
            {
                newYvel = newYvel * cutJumpHeight;
            }
        }

        if (isWallStuck)
        {
            RaycastHit2D hit1 =
                Physics2D
                    .Raycast(transform.position,
                    new Vector2(1, 0),
                    wallJumpColliderLen,
                    lmWallStick);
            RaycastHit2D hit2 =
                Physics2D
                    .Raycast(transform.position,
                    new Vector2(-1, 0),
                     wallJumpColliderLen,
                    lmWallStick);
            if (hit1.collider != null)
            {
                if (jumpHangGraceTicks > 0)
                {
                    newYvel = wallJumpVel;
                    jumpHangGraceTicks = -1;
                    groundRegisterGraceTicks = -1;
                    jumpCooldown = jumpCooldownMax;
                    haltMomentum = false;
                    isGrounded = false;
                    wallSliding = false;

                    isWallStuck = false;

                    wallJumpedLeft = true;
                    if (horizontal != 0)
                    {
                        newXvel = -wallJumpDistance;
                        Invoke("RestoreControlsLeft",
                        moveLockTimerAfterWallJump);
                    }
                    else
                    {
                        newXvel = -neutralWallJumpDistance;
                        Invoke("RestoreControlsLeft",
                        moveLockTimerAfterNeutralJump);
                    }
                }
                else if (RArrowPressed)
                {
                    if (
                        wallSliding == false &&
                        !wallJumpedLeft &&
                        !wallJumpedRight
                    )
                    {
                        wallSliding = true;
                    }
                }
                else
                {
                    wallSliding = false;
                }
            }
            else if (hit2.collider != null)
            {
                if (jumpHangGraceTicks > 0)
                {
                    newYvel = wallJumpVel;
                    jumpHangGraceTicks = -1;
                    groundRegisterGraceTicks = -1;
                    jumpCooldown = jumpCooldownMax;
                    haltMomentum = false;
                    isGrounded = false;
                    wallSliding = false;
                    isWallStuck = false;

                    wallJumpedRight = true;
                    if (horizontal != 0)
                    {
                        newXvel = wallJumpDistance;
                        Invoke("RestoreControlsRight",
                        moveLockTimerAfterWallJump);
                    }
                    else
                    {
                        newXvel = neutralWallJumpDistance;
                        Invoke("RestoreControlsRight",
                        moveLockTimerAfterNeutralJump);
                    }
                }
                else if (LArrowPressed)
                {
                    if (
                        wallSliding == false &&
                        !wallJumpedLeft &&
                        !wallJumpedRight
                    )
                    {
                        wallSliding = true;
                    }
                }
                else
                {
                    wallSliding = false;
                }
            }
            else
            {
                //Debug.Log("Naturally, something went catastrophically wrong with the wall sliding code");
            }
        }
        else
        {
            wallSliding = false;
        }
        if (dashing && wallSliding)
        {
            Debug.Log("attempting to cancel dash");
            CancelInvoke("restoreDash");
            restoreDash();
            rb.gravityScale = normalGrav;
            rb.velocity = Vector2.zero;
        }
        if (haltMomentum)
        {
            if (preserveMomentum)
            {
                preserveMomentum = false;
            }
            else
            {
                Debug.Log("halting momentum");
                newXvel = newXvel - newXvel / dashEndDampening;
                newYvel = newYvel - newYvel / dashEndDampening;
            }
            haltMomentum = false;
        }
        HandleGravity();

        //set the new velocity
        rb.velocity = new Vector2(newXvel, newYvel);
        if (ZPressed)
        {
            ZPressed = false;
        }
        if (ZUnpressed)
        {
            ZUnpressed = false;
        }
        if (XPressed)
        {
            XPressed = false;
        }
        if (XUnpressed)
        {
            XUnpressed = false;
        }
    }

    void Update()
    {
        LArrowPressed = Input.GetKey(KeyCode.LeftArrow);
        RArrowPressed = Input.GetKey(KeyCode.RightArrow);
        UArrowPressed = Input.GetKey(KeyCode.UpArrow);
        DArrowPressed = Input.GetKey(KeyCode.DownArrow);

        if (Input.GetKeyDown(KeyCode.Z)) ZPressed = true;
        if (Input.GetKeyUp(KeyCode.Z)) ZUnpressed = true;
        if (Input.GetKeyDown(KeyCode.X)) XPressed = true;
        if (Input.GetKeyUp(KeyCode.X)) XUnpressed = true;

        if (dashes == 0)
        {
            SetSprite(sprite);
        }
        else if (dashes == 1)
        {
            SetSprite(sprite_1);
        }
        else if (dashes > 1)
        {
            //GetComponent<SpriteRenderer>().color = maxDashesColor;
        }
    }


    void DrawTrail()
    {
        if (showTrail)
        {
            GameObject trail =
                Instantiate(trailPrefab,
                transform.position,
                Quaternion.identity);
            if (dashes == 0)
            {
                trail.GetComponent<SpriteRenderer>().color = noDashesColor;
            }
            else if (dashes == 1)
            {
                trail.GetComponent<SpriteRenderer>().color = normalDashesColor;
            }
            else if (dashes > 1)
            {
                trail.GetComponent<SpriteRenderer>().color = maxDashesColor;
            }
            Destroy(trail, 5f);
        }
    }

    void RestoreControlsLeft()
    {
        wallJumpedLeft = false;
    }

    void RestoreControlsRight()
    {
        wallJumpedRight = false;
    }

    void HandleGravity()
    {
        if (dashing)
        {
            rb.gravityScale = 0;
        }
        else if (wallSliding)
        {
            // rb.gravityScale = slidingGrav;
        }
        else
        {
            rb.gravityScale = normalGrav;
        }
    }

    void restoreDash()
    {
        dashAvailable = true;
        haltMomentum = true;
        dashing = false;
    }

    void SetSprite(GameObject _sprite)
    {
        if (_sprite == sprite)
        {
            sprite_1.SetActive(false);
        }
        else
        {
            sprite.SetActive(false);
        }

        activeSprite = _sprite;
        activeSprite.SetActive(true);
    }
}
