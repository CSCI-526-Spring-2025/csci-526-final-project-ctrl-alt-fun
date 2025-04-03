using UnityEngine;
using System.Collections;

public class PlatformerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    // 你可以调低 jumpForce 的值来降低基础跳跃高度
    public float jumpForce = 15f;
    // 下落时额外加速的倍率（值越大下落越快）
    public float fallMultiplier = 1f;
    // 当玩家未持续按住跳跃键时，额外施加的上升阶段的加速度倍率
    public float lowJumpMultiplier = 1.5f;

    private Rigidbody rb;
    private bool isGrounded = false;

    // 记录上一次检测到接触地面的时间
    private float lastCollisionTime = 0f;
    // 离开地面后延迟检测的时间
    public float groundDelay = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameOverManager.instance != null && GameOverManager.instance.isGamePaused) return;
        // 仅允许在 X 轴上移动
        float h = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            h = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            h = 1f;
        }
        Vector3 velocity = rb.velocity;
        velocity.x = h * moveSpeed;
        rb.velocity = velocity;

        // 落地状态下允许跳跃
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;  // 跳跃后立即置为 false
        }

        // 当玩家下落时，增加额外的重力让下落更快
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // 如果玩家正在上升，但松开了跳跃键，则额外加速下降（让跳跃更低）
        else if (rb.velocity.y > 0 && !(Input.GetKey(KeyCode.UpArrow)))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // 利用 OnCollisionStay 检测与地面的持续接触
    void OnCollisionStay(Collision collision)
    {
        bool grounded = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            // 判断接触面的法向量与角色上方向的夹角
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
        if (Time.time - lastCollisionTime >= groundDelay)
        {
            isGrounded = false;
        }
    }
}
