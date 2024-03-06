using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Rigidbody2D PlayerRigidBody;
    private Vector2 BoxcolliderInitialSize;
    private Vector2 BoxcolliderInitialOffSet;
    [SerializeField] private Vector2 BoxColliderReducedSize;
    [SerializeField] private Vector2 BoxColliderReducedOffSet;
    private float horizontalAxisValue, verticalAxisValue;
    private Vector3 scale, position;
    private bool isRunning, canLooseOneLife;
    [SerializeField] private float speed, jumpForce, PlayerRechargeAnimationTime;
    private Vector2 force;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private GameOverController gameOverController;
    [SerializeField] private int MaxLives;
    [SerializeField] private int RemainingLives;


    private bool isGrounded, isJumping, playerStartedJumping, isCrouched;
    public Vector2 boxSize;
    public LayerMask whatIsGround;

    private AudioSource PlayerAudioSource;


    private Dictionary<Sounds, bool> isSoundClipPlaying;


    //getter
    public int getMaxLives()
    {
        return MaxLives; 
    }

    void Awake()
    {
        BoxcolliderInitialSize = boxCollider.size;
        BoxcolliderInitialOffSet = boxCollider.offset;
        isRunning = false;
        canLooseOneLife = true;
        force = new Vector2(0f, jumpForce);
    }

    // Start is called before the first frame update
    void Start()
    {

        isSoundClipPlaying = new Dictionary<Sounds, bool>();

        RemainingLives = MaxLives;
        healthManager.PlaceHearts(MaxLives);
        isJumping = false;
        playerStartedJumping = false;

        PlayerAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalAxisValue = Input.GetAxisRaw("Horizontal");
        MoveCharacter(horizontalAxisValue);
        PlayerMovementAnimation(horizontalAxisValue, verticalAxisValue);
        Crouch();
        JumpCharacter();
        CheckForAboveTheGround();
        CheckForReturnToGround();
        PlayerMovementSound();

        //Debug.Log("isJumping = " + isJumping);
    }



    private void JumpCharacter()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if (!isCrouched)
            {
                isGrounded = Physics2D.OverlapBox(transform.position, boxSize, 0f, whatIsGround);
                if (isGrounded)
                {
                    PlayerRigidBody.AddForce(force, ForceMode2D.Force);
                    animator.SetTrigger("Jump");

                    HandlePlayerAudio(Sounds.PlayerJump);

                    playerStartedJumping = true;
                }
            }           
        }
    }

    private void CheckForAboveTheGround()
    {
        if(playerStartedJumping)
        {
            bool isPlayerGrounded = Physics2D.OverlapBox(transform.position, boxSize, 0f, whatIsGround);
            if(!isPlayerGrounded)
            {
                isJumping = true;
                playerStartedJumping = false;
            }
        }
    }

    private void CheckForReturnToGround()
    {
        if(isJumping)
        {
            bool isAgainGrounded = Physics2D.OverlapBox(transform.position, boxSize, 0f, whatIsGround);

            
            if (isAgainGrounded)
            {
                HandlePlayerAudio(Sounds.PlayerLand);

                isJumping = false;
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, boxSize);
    }


    private void PlayerMovementSound()
    {
        if(Input.GetButton("Horizontal"))
        {
            if (!isJumping)
            {
                HandlePlayerAudio(Sounds.PlayerMove);
            }
        }        
    }


    private void MoveCharacter(float horizontal)
    {
        //move character horizontally
        if (!isCrouched)
        {
          if (transform.position.y < position.y)
          {
              isJumping = true;
          }


          position = transform.position;
          position.x += horizontal * speed * Time.deltaTime;
          transform.position = position;                      
        }
    }

    private void Crouch()
    {
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (!isRunning)
            {
                animator.SetBool("Crouch", true);
                boxCollider.size = BoxColliderReducedSize;
                boxCollider.offset = BoxColliderReducedOffSet; 
                isCrouched = true;
            }
        }
        else if(Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            animator.SetBool("Crouch", false);
            boxCollider.size = BoxcolliderInitialSize;
            boxCollider.offset = BoxcolliderInitialOffSet;
            isCrouched = false;
        }
    }

    private void PlayerMovementAnimation(float horizontalValue, float verticalValue)
    {
        if (isJumping)
        {
            animator.SetFloat("Speed", 0f);
        }
        else if (!isJumping)
        {
            //run
            animator.SetFloat("Speed", Mathf.Abs(horizontalValue));
            //SoundManager.Instance.Play(Sounds.PlayerMove);
        }

        scale = transform.localScale;
        if (horizontalValue < 0)
        {
          scale.x = -1f * Mathf.Abs(scale.x);
          isRunning = true;
        }
        else if (horizontalValue > 0)
        {
          scale.x = Mathf.Abs(scale.x);
          isRunning = true;
        }
        else
        {
          isRunning = false;
        }
        transform.localScale = scale;       
    }

    public void PickUpKey()
    {
        scoreController.IncreaseScore(10);
    }

    public void LooseOneLife()
    {
        if(canLooseOneLife)
        {
            RemainingLives--;
            if(RemainingLives < 1)
            {
                KillPlayer();
            }
            else
            {
                StartCoroutine("RechargeTime");
            }
            healthManager.UpdateHearts(RemainingLives);
        }
    }

    public void KillPlayer()
    {
        animator.SetBool("Dead", true);
        gameOverController.PlayerDied();    
        this.enabled = false;
        boxCollider.enabled = false;
        PlayerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public void PlayerInactive()
    {
        animator.SetFloat("Speed", 0f);
        this.enabled = false;
        boxCollider.enabled = false;
        PlayerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    private IEnumerator RechargeTime()
    {
        canLooseOneLife = false;
        animator.SetBool("Recharge", true);
        yield return new WaitForSeconds(PlayerRechargeAnimationTime);
        animator.SetBool("Recharge", false);
        canLooseOneLife = true;
    }

    private IEnumerator CheckClipIsSillPlaying(float duration, Sounds sound)
    {
        yield return new WaitForSeconds(duration);
        isSoundClipPlaying[sound] = false;
    }


    private void HandlePlayerAudio(Sounds sound)
    {
        if (!isSoundClipPlaying.ContainsKey(sound))
        {
            isSoundClipPlaying.Add(sound, false);
        }

            if (isSoundClipPlaying[sound] == false)
            {
                Debug.Log("Has Entry");
                float clipDuration = SoundManager.Instance.PlaySoundOfAudioSource(PlayerAudioSource, sound);
                isSoundClipPlaying[sound] = true;
                StartCoroutine(CheckClipIsSillPlaying(clipDuration, sound));
            }  
    }

}
