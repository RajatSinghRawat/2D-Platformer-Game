using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    private bool isRunning;
    [SerializeField] private float speed, jumpForce;
    private Vector2 force;
    [SerializeField] private ScoreController scoreController;


    void Awake()
    {
        BoxcolliderInitialSize = boxCollider.size;
        BoxcolliderInitialOffSet = boxCollider.offset;
        isRunning = false;
        force = new Vector2(0f, jumpForce);
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
}
