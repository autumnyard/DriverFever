using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EntityBase : MonoBehaviour
{

    #region Variables
    protected enum States
    {
        Init = 0,
        Appearing,
        Normal,
        Hurting,
        Dying,
        Dead,
        MaxValues
    }
    protected States currentState { private set; get; }

    // Components
    // [Header("Components")]
    private Animator animator;
    new private Rigidbody2D rigidbody;
    new private Collider2D collider;
    new private SpriteRenderer renderer;

    // Data
    // [Header("Data")]
    private int id;
    protected int health;
    protected const int healthMax = 20;
    private bool isInvulnerable;
    private float invulnerabilityTime = -1f;
    private const uint invulnerabilityFrames = 5u;

    // Physics
    [Header( "Physics" ), SerializeField]
    private float acceleration = 4f;
    [SerializeField] private float dash = 2f;
    [SerializeField] private float dashAgainTime = 0.5f;
    private bool canDash = true;
    Coroutine dashTimerCoroutine;
    [SerializeField] private float grip = 2f;
    [SerializeField] private float dragLinear = 1f;
    [SerializeField] private float dragAngular = 0.05f;
    [SerializeField] private float rotationForce = 2f;


    // Tweens
    //[Header("Tweens")]
    //public TweenScale tweenAppear;
    //public TweenScale tweenDisappear;
    private Vector3 tweenAppearScaleBegin = Vector3.zero;
    private Vector3 tweenAppearScaleFinish = Vector3.one;
    private Vector3 tweenDisappearScaleBegin = Vector3.one;
    private Vector3 tweenDisappearScaleFinish = Vector3.zero;

    // Events
    public delegate void Delegate();
    public Delegate OnAppear;
    public Delegate OnDie;
    public Delegate OnExitChunk;
    public Delegate OnCollideWithEntity;

    public delegate void DelegateInt( int newHealth );
    public DelegateInt OnHurt;
    #endregion


    #region Monobehaviour
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (animator == null)
        {
            //Debug.LogWarning("Animator wasn't setted in " + this.gameObject.name);
        }

        if (rigidbody == null)
        {
            rigidbody = transform.GetComponent<Rigidbody2D>();
        }
        if (rigidbody == null)
        {
            rigidbody = transform.GetComponentInChildren<Rigidbody2D>();
        }
        if (rigidbody == null)
        {
            //Debug.LogWarning("Rigidbody wasn't setted in " + this.gameObject.name);
        }

        if (renderer == null)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
        if (renderer == null)
        {
            renderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (renderer == null)
        {
            //Debug.LogWarning("Renderer wasn't setted in " + this.gameObject.name);
        }

        if (collider == null)
        {
            collider = GetComponent<Collider2D>();
        }
        if (collider == null)
        {
            collider = GetComponentInChildren<Collider2D>();
        }
        if (collider == null)
        {
            //Debug.LogWarning("Collider wasn't setted in " + this.gameObject.name);
        }

    }

    private void Start()
    {
        ChangeState( States.Init );
    }
    #endregion


    #region Life cycle
    protected void ChangeState( States to )
    {
        currentState = to;
        //Debug.Log("Change the entity " + name + " state to: " + currentState);
        RunState();
    }

    private void RunState()
    {
        switch (currentState)
        {
            case States.Init:
                SetInvulnerability( true );
                health = healthMax;
                Director.Instance.managerUI.SetHealth( health );
                ChangeState( States.Appearing ); // Automatically change to appearing
                break;

            case States.Appearing:
                PlayAnimationAppearing(); // Play appearing animation
                break;

            case States.Normal:
                SetInvulnerability( false );
                break;

            case States.Hurting:
                SetInvulnerability( true );
                PlayAnimationHurting(); // Play hurting animation
                break;

            case States.Dying:
                SetInvulnerability( true );
                PlayAnimationDying(); // Play dying animation
                break;

            case States.Dead:
                Die();
                break;
        }
    }
    #endregion


    #region Entity management
    public void Set( int idP )
    {
        id = idP;

        // Physics
        rigidbody.drag = dragLinear;
        rigidbody.angularDrag = dragAngular;
    }

    private void SetInvulnerability( bool to )
    {
        isInvulnerable = to;
        if (isInvulnerable)
        {
            collider.enabled = false;
        }
        else
        {
            collider.enabled = true;
        }
    }

    private IEnumerator SetInvulnerabilityTimed( float time )
    {
        collider.enabled = false;
        yield return new WaitForSeconds( invulnerabilityTime );
        collider.enabled = true;
    }

    private void Hurt( int damage = 10 )
    {
        // TODO: This check may be unnecesary
        if (!isInvulnerable)
        {
            health -= damage;

            //Debug.Log(name+" was hurt, remaining health: "+ health);

            if (OnHurt != null)
            {
                OnHurt( health );
            }

            // You can instantly dash again after being hurt
            if (dashTimerCoroutine != null)
            {
                StopCoroutine( dashTimerCoroutine );
            }
            canDash = true;

            if (health <= 0)
            {
                ChangeState( States.Dying );
            }
            else
            {
                ChangeState( States.Hurting );
            }
        }
    }

    private void Die()
    {
        // TODO: Check integrity, this should be better integrated with entitymanager
        if (OnDie != null)
        {
            OnDie();
        }

        // TODO: Does this destroy everything?
        //Director.Instance.managerEntity.RemovePlayer( id );

        //Debug.Log(name + " has died.");
    }

    private void Reappear()
    {
        ChangeState( States.Appearing );
        //transform.localPosition = Vector2.zero;
        //transform.localPosition = initialPosition;
        rigidbody.velocity = Vector2.zero;
        canDash = true;
    }
    #endregion


    #region Animations and events
    private void PlayAnimationAppearing()
    {
        // Play animation, when it is finished change state to Normal
        // Play animation, when it is finished change state to Normal
        float scaleInit = 0f;
        float scaleFinish = 1f;
        float time = 0.4f;
        Ease ease = Ease.OutBack;

        transform.localScale.Set( scaleInit, scaleInit, transform.localScale.z );
        transform.DOScale( new Vector3( scaleFinish, scaleFinish, transform.localScale.z ), time )
                          .SetEase( ease )
                          .OnComplete( PlayAnimationAppearingHelper );
    }

    private void PlayAnimationAppearingHelper()
    {
        ChangeState( States.Normal );
    }

    private void PlayAnimationHurting()
    {
        // Play animation, when it is finished change state to Normal
        float scaleInit = 1f;
        float scaleFinish = 1.2f;
        float time = 0.4f;
        Ease ease = Ease.InOutBack;

        transform.localScale.Set( scaleInit, scaleInit, transform.localScale.z );
        transform.DOScale( new Vector3( scaleFinish, scaleFinish, transform.localScale.z ), time )
                          .SetEase( ease )
                          .SetLoops( 2, LoopType.Yoyo );
        //.OnComplete( Reappear );
    }

    private void PlayAnimationDying()
    {
        // Play animation, when it is finished change state to Dead
        float scaleInit = 1f;
        float scaleFinish = 0f;
        float time = 0.5f;
        Ease ease = Ease.OutBack;

        transform.localScale.Set( scaleInit, scaleInit, transform.localScale.z );
        transform.DOScale( new Vector3( scaleFinish, scaleFinish, transform.localScale.z ), time )
                          .SetEase( ease )
                          .OnComplete( PlayAnimationDyingHelper );
    }

    private void PlayAnimationDyingHelper()
    {
        ChangeState( States.Dead );
    }
    #endregion


    #region Physics
    private void MoveUp()
    {
        rigidbody.AddForce( Vector2.up * acceleration, ForceMode2D.Force );
    }

    private void MoveDown()
    {
        rigidbody.AddForce( Vector2.down * acceleration, ForceMode2D.Force );
    }

    private void MoveLeft( float multiplier = 1f, bool relative = false, ForceMode2D mode = ForceMode2D.Force )
    {
        Vector2 direction = Vector2.zero;
        if (relative)
        {
            direction = -transform.right;
        }
        else
        {
            direction = Vector2.left;
        }
        rigidbody.AddForce( direction * acceleration, mode );
    }

    private void MoveRight( float multiplier = 1f, bool relative = false, ForceMode2D mode = ForceMode2D.Force )
    {
        Vector2 direction = Vector2.zero;
        if (relative)
        {
            direction = transform.right;
        }
        else
        {
            direction = Vector2.right;
        }
        rigidbody.AddForce( direction * acceleration, mode );
    }

    private void MoveForward()
    {
        rigidbody.AddForce( transform.up * acceleration, ForceMode2D.Force );
    }

    private void RotateLeft( ForceMode2D mode )
    {
        rigidbody.AddTorque( rotationForce, mode );
    }

    private void RotateRight( ForceMode2D mode )
    {
        rigidbody.AddTorque( -rotationForce, mode );
    }
    #endregion


    #region Collision
    private void OnTriggerExit2D( Collider2D col )
    {
        // For example, when exiting the game zone
        if (col.gameObject.CompareTag( "Chunk" ))
        {
            //Die();
            if (OnExitChunk != null)
            {
                OnExitChunk();
            }
        }
    }

    private void OnCollisionEnter2D( Collision2D collision )
    {
        // For example, when touching another entity
        //if (collision.gameObject.CompareTag( "Player" ))
        //{
        //    if (OnCollideWithEntity != null)
        //    {
        //        OnCollideWithEntity();
        //    }
        //}
    }
    #endregion


    #region Input
    public void DashLeft()
    {
        if (canDash)
        {
            MoveLeft( dash, true, ForceMode2D.Impulse );
            dashTimerCoroutine = StartCoroutine( DashTimer() );
        }
    }

    public void DashRight()
    {
        if (canDash)
        {
            MoveRight( dash, true, ForceMode2D.Impulse );
            dashTimerCoroutine = StartCoroutine( DashTimer() );
        }
    }

    IEnumerator DashTimer()
    {
        //yield return new WaitForSeconds( dashTime );
        //if (currentState == States.Dashing)
        //{
        //    ChangeState( States.Normal );
        //    trail.time = 0.1f;
        //}

        canDash = false;
        yield return new WaitForSeconds( dashAgainTime );
        canDash = true;

        // Tween to show that you can dash again
        //transform.GetChild( 0 ).DOScale( new Vector3( transform.localScale.x * dashAgainScale, transform.localScale.y * dashAgainScale, transform.localScale.z ), timeAnimationDashAgain )
        //                  .SetEase( Ease.InOutCubic )
        //                  .SetLoops( 2, LoopType.Yoyo );
    }

    public void Accelerate()
    {
        MoveForward( );
    }

    public void RotateLeft()
    {
        RotateLeft( ForceMode2D.Force );
    }

    public void RotateRight()
    {
        RotateRight( ForceMode2D.Force );
    }
    #endregion
}
