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
    [SerializeField] private float speed, jumpForce, PlayerDeadAnimationTime, PlayerRechargeAnimationTime;
    private Vector2 force;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private GameOverController gameOverController;
    [SerializeField] private int MaxLives;
    [SerializeField] private int RemainingLives;


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
        RemainingLives = MaxLives;
        healthManager.PlaceHearts(MaxLives);
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalAxisValue = Input.GetAxisRaw("Horizontal");
        verticalAxisValue = Input.GetAxisRaw("Jump");

        MoveCharacter(horizontalAxisValue, verticalAxisValue);
        PlayerMovementAnimation(horizontalAxisValue, verticalAxisValue);
        Crouch();
    }


    private void MoveCharacter(float horizontal, float vertical)
    {
        //move character horizontally
        position = transform.position;
        position.x += horizontal * speed * Time.deltaTime;
        transform.position = position;

        //move character vertically
        if (vertical > 0)
        {
            if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Platform")))
            {
                PlayerRigidBody.AddForce(force, ForceMode2D.Force);
            }
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
            }
        }
        else if(Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            animator.SetBool("Crouch", false);
            boxCollider.size = BoxcolliderInitialSize;
            boxCollider.offset = BoxcolliderInitialOffSet;
        }
    }


    private void PlayerMovementAnimation(float horizontalValue, float verticalValue)
    {
        //run
        animator.SetFloat("Speed", Mathf.Abs(horizontalValue));

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

        //jump
        if (verticalValue > 0)
        {
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
        }
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

    /*private void ReloadLevel()
    {
        StartCoroutine("ReloadAfterAnimationFinished");
    }*/


    /*private IEnumerator ReloadAfterAnimationFinished()
    {
        yield return new WaitForSeconds(PlayerDeadAnimationTime);
        animator.SetBool("Dead", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }*/

    private IEnumerator RechargeTime()
    {
        canLooseOneLife = false;
        animator.SetBool("Recharge", true);
        yield return new WaitForSeconds(PlayerRechargeAnimationTime);
        animator.SetBool("Recharge", false);
        canLooseOneLife = true;
    }
}
