using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDeley = 0.25f;
    public float jumpTimer; 

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;
    // public GameObject characterHolder;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = false;
    public float groundLength = 0.69f;
    public Vector3 colliderOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||  Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        if(Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDeley;
        }
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    void FixedUpdate()
    {
        MoveCharacter(direction.x);
        if(jumpTimer > Time.time && onGround)
        {
            Jump();
        }
        ModifyPhysics();
    }
    void MoveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        //animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));

        if((horizontal>0 && !facingRight) || (horizontal<0 && facingRight ))
        {
            Flip();
        }

        if(Mathf.Abs(rb.velocity.x) > (maxSpeed))
        {
            rb.velocity = new Vector2 (Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }
    void Jump()
    {
        rb.velocity = new Vector2 (rb.velocity.x,0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
        // StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
    }
    void ModifyPhysics()
    {
        bool changingDirections = (direction.x> 0 && rb.velocity.x <0) || (direction.x < 0 && rb.velocity.x >0);
        if(onGround)
        {    
            if(Mathf.Abs (direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if(rb.velocity.y<0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if(rb.velocity.y>0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }
    void Flip()
    {
        facingRight =!facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180,0);
    }
    // IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    // {
    //     Vector3 originalSize = Vector3.one;
    //     Vector3 newSize = new Vector3 (xSqueeze, ySqueeze, originalSize.z);
    //     float t = 0f;
    //     while (t <= 1.0)
    //     {
    //         t += jumpTimer.deltaTime / seconds;
    //         characterHolder.transform.localScale = Vector3.Lerp (originalSize, newSize, t);
    //         yield return null;
    //     }
    //     t = 0f;
    //     while (t <= 1.0)
    //     {
    //         t+=jumpTimer.deltaTime / seconds;
    //         characterHolder.transform.localScale = Vector3.Lerp (newSize, originalSize, t);
    //         yield return null;
    //     }

    // }
    private void OnDrawGizmos()
    {
        Gizmos.color  = Color.white;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}
