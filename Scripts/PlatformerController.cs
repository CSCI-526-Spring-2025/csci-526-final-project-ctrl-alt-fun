using UnityEngine;

public class PlatformerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody rb;
    private bool isGrounded = false;

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
        velocity.z = 0.5f;  // 限制 Z 轴位移
        rb.velocity = velocity;

        // 只有落地状态下（isGrounded == true）才允许跳跃
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Z)))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;  // 跳跃后立即设为 false
        }
    }

    // 通过 OnCollisionStay 检测持续接触情况
    void OnCollisionStay(Collision collision)
    {
        bool grounded = false;
        // 遍历所有接触点，判断是否存在底部接触
        foreach (ContactPoint contact in collision.contacts)
        {
            // 如果接触点的法线和向上的向量点积大于 0.5（可根据需要调整阈值）
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                grounded = true;
                break;
            }
        }
        isGrounded = grounded;
    }

    // 当碰撞结束时，将 isGrounded 置为 false
    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
