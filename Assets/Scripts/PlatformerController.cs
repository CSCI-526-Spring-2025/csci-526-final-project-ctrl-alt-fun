using UnityEngine;
using System.Collections;

public class PlatformerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody rb;
    private bool isGrounded = false;

    // 记录上一次检测到接触地面的时间
    private float lastCollisionTime = 0f;
    // 延迟时间（秒），可以根据实际情况调整
    public float groundDelay = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 仅允许在 X 轴移动，保持 Z 轴为固定值
        float h = Input.GetAxis("Horizontal");
        Vector3 velocity = rb.velocity;
        velocity.x = h * moveSpeed;
        //velocity.z = 0.5f;  // 限制 Z 轴位移
        rb.velocity = velocity;

        // 只有落地状态下才允许跳跃
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;  // 跳跃后立即设为 false
        }
        // Debug.Log(velocity);
    }

    // 利用 OnCollisionStay 检测持续接触情况
    void OnCollisionStay(Collision collision)
    {
        bool grounded = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            // 使用局部上方向检测
            if (Vector3.Dot(contact.normal, transform.up) > 0.5f)
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

    // 当碰撞结束时，延迟一段时间再判断是否真的脱离地面
    void OnCollisionExit(Collision collision)
    {
        StartCoroutine(DelayedGroundCheck());
    }

    private IEnumerator DelayedGroundCheck()
    {
        yield return new WaitForSeconds(groundDelay);
        // 如果在延迟期间没有更新 lastCollisionTime，则认为已经脱离地面
        if (Time.time - lastCollisionTime >= groundDelay)
        {
            isGrounded = false;
        }
    }
}