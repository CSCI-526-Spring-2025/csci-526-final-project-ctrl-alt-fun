using System.Collections;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public GameObject spriteBody;          // 拖入 SpriteBody 对象
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float groundDelay = 0.2f;

    private Animator animator;
    private Rigidbody rb;
    private bool isGrounded = false;
    private float lastCollisionTime = 0f;

    void Start()
    {
        animator = spriteBody.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            moveInput = 1f;

        // 移动（只控制x方向速度，不改y/z）
        Vector3 velocity = rb.velocity;
        velocity.x = moveInput * moveSpeed;
        rb.velocity = velocity;

        // 左右翻转 Sprite
        if (moveInput != 0)
        {
            Vector3 scale = spriteBody.transform.localScale;
            scale.x = moveInput > 0 ? 1 : -1;
            spriteBody.transform.localScale = scale;
        }

        // 设置动画参数（仅用于切换动画）
        animator.SetBool("isWalking", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        // 跳跃（只在地面且按键时，施加竖直向上跳跃力）
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        bool grounded = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                grounded = true;
                break;
            }
        }
        if (grounded)
        {
            isGrounded = true;
            lastCollisionTime = Time.time;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        StartCoroutine(DelayedGroundCheck());
    }


    private IEnumerator DelayedGroundCheck()
    {
        yield return new WaitForSeconds(groundDelay);
        if (Time.time - lastCollisionTime >= groundDelay)
        {
            isGrounded = false;
        }
    }
}
