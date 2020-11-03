using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    Rigidbody2D body;
    public SpriteRenderer spRender;
    public CapsuleCollider2D myCollider;
    public TrailRenderer trail;
    public Transform arrowPivot;
    public Image shotForceBar;
    private ConstantForce2D constanteForce;

    [Header("Particles")]
    public ParticleSystem swatParticle;

    [Header("MoveAttributes")]
    public float speedMax;
    public float speedAirMax;
    public float acceleration;
    public float desacceleration;
    public float jumpForce;
    public float dashForce;
    public float dashDuration;
    public float cutDashForce;
    public float wallDistanceRay;
    public float groundDistanceRay;
    public float rotationVelocity;
    public float gravity;
    public float slope;
    public bool grounded;
    public bool groundedPlayer;
    public bool wall;
    public bool rightWall;
    public bool leftWall;
    public bool doubleJump;        

    [Header("PackageAttributes")]
    public float accelerationForce;
    public float throwMinForce;
    public float throwMaxForce;
    public float currentForce;
    public bool withPackage;
    public bool exhausted;

    public PlayerId curPlayerId;
    public LayerMask wallMask;

    private Vector3 currentVelocity;
    private Vector2 directionDash = Vector2.zero;
    private PackageController curPackage;
    private Coroutine curCoroutine;
    private Vector2 directionThrow;
    private Vector2 curGravityDirection;
    private float curSlope;
    bool sloped;

    [HideInInspector]
    public float horizontalAxis;
    [HideInInspector]
    public float verticalAxis;
    [HideInInspector]
    public float throwAxis;
    private float currentMaxSpeed;    
    private float currentStateTime;
    private float curSlopeVelocity;

    public PlayerState currentState;

    public bool jumpButtonDown;
    public bool jumpButtonUp;
    public bool throwButtonDown;
    public bool throwButton;
    public bool throwButtonUp;
    public bool cancelButton;
    #endregion

    #region Unity Functions
    void Start()
    {
        body                    = GetComponent<Rigidbody2D>();
        myCollider              = GetComponent<CapsuleCollider2D>();
        constanteForce          = GetComponent<ConstantForce2D>();
        arrowPivot.gameObject.SetActive(false);
        constanteForce.force    = -transform.up * gravity;
    }
    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Move:
                if(withPackage && throwButton)
                {
                    ChangeState(PlayerState.Throw);
                }
                GroundMove();                
                break;
            case PlayerState.Dash:
                DashState();
                break;
            case PlayerState.Charging:
                break;
            case PlayerState.Throw:
                ThrowState();                
                break;
        }
    }
    #endregion

    #region ChangeState
    public void ChangeState(PlayerState newState)
    {
        currentStateTime = Time.timeSinceLevelLoad;

        switch (currentState)
        {
            case PlayerState.None:
                break;
            case PlayerState.Dash:
                trail.emitting = false;
                //body.isKinematic = false;
                constanteForce.force = -transform.up * gravity;
                if (!grounded)
                {                    
                    currentVelocity = directionDash * cutDashForce;
                    body.velocity   = currentVelocity;
                }                                
                break;
            case PlayerState.Move:
                break;
            case PlayerState.Throw:
                currentForce = 0;
                arrowPivot.gameObject.SetActive(false);
                if (curCoroutine != null)
                    StopCoroutine(curCoroutine);
                curCoroutine = StartCoroutine(FreePackage());
                break;
        }
        switch (newState)
        {
            case PlayerState.None:
                break;
            case PlayerState.Dash:
                //body.isKinematic    = true;
                constanteForce.force = Vector2.zero;
                trail.emitting      = true;
                doubleJump          = false;
                directionDash.x     = horizontalAxis;
                directionDash.y     = verticalAxis;
                directionDash.Normalize();
                currentVelocity     = directionDash * dashForce;                
                body.velocity       = currentVelocity;
                break;
            case PlayerState.Move:
                break;
            case PlayerState.Charging:
                break;
            case PlayerState.Throw:
                currentForce = 0;
                arrowPivot.gameObject.SetActive(true);
                break;
        }
        currentState = newState;
    }
    #endregion

    #region States
    public void GravityMethod()
    {
    //    if (curSlope > slope)
    //        currentVelocity = -transform.right * gravity;
    //    //constanteForce.force = -transform.right * gravity * 2;
    //    else if (curSlope < -slope)
    //        currentVelocity = transform.right * gravity;
        //constanteForce.force = transform.right * gravity * 2;

        constanteForce.force = -transform.up * gravity;

        currentVelocity.y = body.velocity.y;
    }
    
    public void GroundMove()
    {
        RaycastWall();
        CheckGroundAngle();
        GravityMethod();

        float diferenceHorizonAxis = 0.1f;
        float curMax = speedMax;

        if (grounded || groundedPlayer)
        {
            //curSlope
            if (horizontalAxis > diferenceHorizonAxis && curSlope < slope)
            {
                currentVelocity.x += acceleration * Time.deltaTime;
            }
            else if (horizontalAxis < -diferenceHorizonAxis && curSlope > -slope)
            {
                currentVelocity.x -= acceleration * Time.deltaTime;
            }
            else if(sloped == false)
            {
                if (currentVelocity.x > 0.5f && curSlope < slope)
                    currentVelocity.x -= acceleration * Time.deltaTime;
                else if (currentVelocity.x < -0.5f && curSlope > -slope)
                    currentVelocity.x += acceleration * Time.deltaTime;
                else
                    currentVelocity.x = 0;
            }
            else if(sloped == true)
            {
                if(curSlope >= slope)
                {
                    currentVelocity -= transform.right * (acceleration * Time.deltaTime);
                }
                else if(curSlope <= -slope)
                {
                    currentVelocity += transform.right * (acceleration * Time.deltaTime);
                }
            }

            if(groundedPlayer == false && sloped == false)
                doubleJump = true;

            float minSlope = 20;
            float time = 4;
            float speedLimit = 0.2f;
            float maxValue = speedMax * 0.8f;
            if (horizontalAxis > 0)
            {
                if (curSlope > minSlope && curSlope < slope)
                {                    
                    float newMax = curSlope / slope;
                    newMax *= maxValue;
                    //curMax -= newMax;
                    //newMax = Mathf.Clamp(newMax, -maxValue, maxValue);
                    //newMax = Mathf.Round(newMax);
                    curSlopeVelocity = Mathf.Lerp(curSlopeVelocity, newMax, time * Time.deltaTime);
                    //curSlopeVelocity = Mathf.Round(curSlopeVelocity);                               
                    curMax -= curSlopeVelocity;
                    //print(curMax);
                }
                else
                {
                    curSlopeVelocity = Mathf.Lerp(curSlopeVelocity, 0, time * Time.deltaTime);                              
                    curMax -= curSlopeVelocity;
                }
            }
            else if(horizontalAxis < 0)
            {
                if(curSlope < -minSlope && curSlope > -slope)
                {
                    float newMax = curSlope / slope;
                    newMax *= maxValue;

                    //curMax -= newMax;
                    //newMax = Mathf.Clamp(newMax, -maxValue, maxValue);
                    //newMax = Mathf.Round(newMax);
                    curSlopeVelocity = Mathf.Lerp(curSlopeVelocity, newMax, time * Time.deltaTime);
                    //curSlopeVelocity = Mathf.Round(curSlopeVelocity);
                    curMax += curSlopeVelocity;
                    //print(curMax);
                }
                else
                {
                    curSlopeVelocity = Mathf.Lerp(curSlopeVelocity, 0, time * Time.deltaTime);                                   
                    curMax += curSlopeVelocity;
                }
            }
            //if(curPlayerId == PlayerId.playerOne)
            //    print(curSlopeVelocity);

            if (withPackage || exhausted)
            {
                curMax *= 0.6f;
            }            

            currentVelocity.x = Mathf.Clamp(currentVelocity.x, -curMax, curMax);

            if (jumpButtonDown)
            {
                float curJump = jumpForce;
                if (withPackage)
                    curJump *= .7f;

                if(curSlope < slope && curSlope > -slope)
                {
                    currentVelocity.y = curJump;
                }
                else
                {                    
                    currentVelocity = transform.up * curJump;
                }                
                //currentVelocity = transform.up * curJump;
            }
        }
        else
        {
            if(curSlopeVelocity < 0)
                curMax += curSlopeVelocity;
            else
                curMax -= curSlopeVelocity;

            if (withPackage || exhausted)
            {
                curMax *= 0.6f;
            }

            //if(curPlayerId == PlayerId.playerOne)
            //{
            //    print("CurMax: " + -curMax);
            //    print("vel: " + currentVelocity.x);
            //}
            //print(curMax);
            //print(curMax);

            if (horizontalAxis > diferenceHorizonAxis && currentVelocity.x < curMax)
                currentVelocity.x += Time.deltaTime * speedAirMax;
            else if (horizontalAxis < -diferenceHorizonAxis && currentVelocity.x > -curMax)
                currentVelocity.x -= Time.deltaTime * speedAirMax;

            if (doubleJump && body.velocity.y > 10 && jumpButtonUp)
                currentVelocity.y = 10;

            if (doubleJump && jumpButtonDown)
            {
                if (withPackage == false && exhausted == false)
                {
                    ChangeState(PlayerState.Dash);
                }
            }
        }

        if (currentVelocity.x > 0 && rightWall == true || currentVelocity.x < 0 && leftWall == true )
        {
            currentVelocity.x = 0;
        }            

        CheckSide(horizontalAxis);

        float maxSpeedY = jumpForce;
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, -maxSpeedY - 20, maxSpeedY);

        body.velocity = currentVelocity;

        if (curPlayerId == PlayerId.playerOne)
            print(body.velocity);
    }
    public void ThrowState()
    {
        if (cancelButton)
        {
            ChangeState(PlayerState.Move);
            return;
        }

        RaycastWall();
        CheckSide(horizontalAxis);

        if (grounded || groundedPlayer)
        {
            if (currentVelocity.x > 0.5f)
                currentVelocity.x -= Time.deltaTime * desacceleration;
            else if (currentVelocity.x < -0.5f)
                currentVelocity.x += Time.deltaTime * desacceleration;
            else
                currentVelocity.x = 0;
        }

        currentVelocity.y = body.velocity.y;
        body.velocity = currentVelocity;        

        currentForce += accelerationForce * Time.deltaTime;
        currentForce = Mathf.Clamp(currentForce, throwMinForce, throwMaxForce);

        //Vector2 direction = new Vector2(horizontalAxis, verticalAxis);

        float diference = 0;
        float time = 8f;

        if(horizontalAxis > diference || horizontalAxis < -diference)
        {
            directionThrow.x = Mathf.Lerp(directionThrow.x, horizontalAxis, time * Time.deltaTime);
            directionThrow.y = Mathf.Lerp(directionThrow.y, verticalAxis, time * Time.deltaTime);
        }
        if (verticalAxis > diference || verticalAxis < -diference)
        {
            directionThrow.x = Mathf.Lerp(directionThrow.x, horizontalAxis, time * Time.deltaTime);
            directionThrow.y = Mathf.Lerp(directionThrow.y, verticalAxis, time * Time.deltaTime);
        }

        directionThrow.Normalize();
        ArrowRotation(directionThrow);

        float scaleBar = currentForce / throwMaxForce;
        shotForceBar.fillAmount = scaleBar;
        if (throwButton == false)
        {
            withPackage = false;
            currentVelocity = Vector2.zero;
            directionThrow.Normalize();
            curPackage.Shot(directionThrow, currentForce);
            ChangeState(PlayerState.Move);
        }
    }
    public void DashState()
    {
        if (grounded || Time.timeSinceLevelLoad > currentStateTime + dashDuration || rightWall || leftWall || exhausted)
        {
            ChangeState(PlayerState.Move);
        }
    }
    #endregion

    #region Methods    
    public IEnumerator FreePackage          ()
    {
        //body.gravityScale = 10f;
        constanteForce.force = -transform.up * gravity;
        yield return new WaitForSeconds(1f);
        swatParticle.Stop();
        exhausted = false;        
        curPackage.Free();
    }
    public void GetPackage                  (Transform package)
    {
        curPackage = package.GetComponent<PackageController>();
        exhausted = true;
        constanteForce.force = -transform.up * (gravity + 10);
        //body.gravityScale = 15f;
        swatParticle.Play();
        curPackage.Caught();
        curPackage.transform.parent = transform;
        curPackage.transform.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.OutSine).OnComplete(() => withPackage = true);
        //curPackage.
    }
    public void CheckSide                   (float Side)
    {
        Vector2 newScale = Vector2.one;
        if(Side > 0)
        {
            if (transform.localScale.x != 1)
            {
                newScale.x = 1;
                transform.localScale = newScale;
                arrowPivot.localScale = newScale;
            }                
        }
        else if(Side < 0)
        {
            if (transform.localScale.x != -1)
            {
                newScale.x = -1;
                transform.localScale = newScale;
                arrowPivot.localScale = newScale;
            }
        }
    }
    public void ArrowRotation               (Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //float newAngle = Mathf.Lerp(arrowPivot.eulerAngles.z, angle, 25 * Time.deltaTime);
        //arrowPivot.eulerAngles = new Vector3(0, 0, newAngle);
        arrowPivot.eulerAngles = new Vector3(0, 0, angle);
    }
    public void RaycastWall                 ()
    {
        //public RaycastHit2D

        Vector3 RightTopPos = transform.position +(transform.up * 0.8f);
        Vector3 RightBotPos = transform.position + (-transform.up * 1f);

        float rayDistance = 1;

        //RightTopPos.x = myCollider.bounds.max.x - rayDistance;
        //RightTopPos.y = myCollider.bounds.max.y - 0.6f;
        //RightBotPos.x = myCollider.bounds.max.x - rayDistance;
        //RightBotPos.y = myCollider.bounds.min.y + 0.6f;



        RaycastHit2D RightTopHit = Physics2D.Raycast(RightTopPos, transform.right, wallDistanceRay, wallMask);
        RaycastHit2D RightBotHit = Physics2D.Raycast(RightBotPos, transform.right, wallDistanceRay, wallMask);
        Debug.DrawLine(RightTopPos, RightTopPos + (transform.right * wallDistanceRay), Color.red);
        Debug.DrawLine(RightBotPos, RightBotPos + (transform.right * wallDistanceRay), Color.red);
        
        //Vector3 LeftTopPos = Vector2.zero;
        //Vector3 LeftBotPos = Vector2.zero;

        //LeftTopPos.x = myCollider.bounds.min.x + rayDistance;
        //LeftTopPos.y = myCollider.bounds.max.y - 0.6f;
        //LeftBotPos.x = myCollider.bounds.min.x + rayDistance;
        //LeftBotPos.y = myCollider.bounds.min.y + 0.6f;

        RaycastHit2D LeftTopHit = Physics2D.Raycast(RightTopPos, -transform.right, wallDistanceRay, wallMask);
        RaycastHit2D LeftBotHit = Physics2D.Raycast(RightBotPos, -transform.right, wallDistanceRay, wallMask);
        Debug.DrawLine(RightTopPos, RightTopPos + (-transform.right * wallDistanceRay), Color.red);
        Debug.DrawLine(RightBotPos, RightBotPos + (-transform.right * wallDistanceRay), Color.red);

        float wallDiference = .95f;

        if (RightTopHit)
        {
            if(RightTopHit.normal.x <= -wallDiference)
            {
                rightWall = true;
                leftWall = false;
                return;
            }
        }
        else if (RightBotHit)
        {
            if (RightBotHit.normal.x <= -wallDiference)
            {
                rightWall = true;
                leftWall = false;
                return;
            }
        }

        if (LeftTopHit)
        {
            if (LeftTopHit.normal.x >= wallDiference)
            {                
                leftWall = true;
                rightWall = false;
                return;
            }
        }
        else if (LeftBotHit)
        {
            if (LeftBotHit.normal.x >= wallDiference)
            {                
                leftWall = true;
                rightWall = false;
                return;
            }
        }
        wall = false;
        leftWall = false;
        rightWall = false;
    }
    public void CheckGroundAngle            ()
    {        
        float angle = 0;

        Vector3 rightPos = transform.position + (transform.right * (myCollider.size.x * 0.5f));
        Vector3 leftPos = transform.position + (-transform.right * (myCollider.size.x * 0.5f));
        rightPos.x -= 0.2f;
        leftPos.x += 0.2f;

        RaycastHit2D hitMid     = Physics2D.Raycast(transform.position, -transform.up, groundDistanceRay, 1 << 8);
        RaycastHit2D hitRight   = Physics2D.Raycast(rightPos, -transform.up, groundDistanceRay, 1 << 8);
        RaycastHit2D hitLeft    = Physics2D.Raycast(leftPos, -transform.up, groundDistanceRay, 1 << 8);

        Debug.DrawLine(transform.position, transform.position + (-transform.up * groundDistanceRay), Color.red);
        Debug.DrawLine(rightPos, rightPos + (-transform.up * groundDistanceRay), Color.red);
        Debug.DrawLine(leftPos, leftPos + (-transform.up * groundDistanceRay), Color.red);
        
        if (hitMid)
        {
            angle = Mathf.Atan2(hitMid.normal.y, hitMid.normal.x) * Mathf.Rad2Deg;
            angle -= 90;
        }
        else if (hitRight)
        {
            angle = Mathf.Atan2(hitRight.normal.y, hitRight.normal.x) * Mathf.Rad2Deg;
            angle -= 90;
        }
        else if (hitLeft)
        {
            angle = Mathf.Atan2(hitLeft.normal.y, hitLeft.normal.x) * Mathf.Rad2Deg;
            angle -= 90;
        }

        //print(angle);

        curSlope = angle;
        if (angle > slope || angle < -slope)
        {
            sloped = true;
        }
        else
        {
            sloped = false;
        }

        float newAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle, rotationVelocity * Time.deltaTime);
        newAngle = Mathf.Round(newAngle);
                
        transform.eulerAngles = new Vector3(0, 0, newAngle);
        
    }

    public void CurVelocityAngle()
    {

    }
    #endregion

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Package"))
        {
            if (withPackage == false && exhausted == false)
                GetPackage(collision.transform);
        }        
    }
}
