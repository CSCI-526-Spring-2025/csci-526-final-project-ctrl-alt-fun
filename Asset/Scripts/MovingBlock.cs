using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public float moveSpeed = 4f;  // 移动速度
    public float moveRange = 3f;  // 最大移动范围
    public float bounceForce = 20f; // 碰撞时给玩家的反弹力

    private Vector3 startPos;
    private int direction = 1; // 1 = 向上, -1 = 向下

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 计算新的 Y 位置
        float newY = transform.position.y + direction * moveSpeed * Time.deltaTime;

        // 限制移动范围并反向
        if (Mathf.Abs(newY - startPos.y) > moveRange)
        {
            direction *= -1;
        }

        // 移动
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // 当玩家碰到 MovingBlock 时，给它一个反弹力
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // 计算反弹方向
                Vector3 bounceDirection = (collision.transform.position - transform.position).normalized;
                bounceDirection.y = 0.1f; // 让它有一点向上的力，避免完全水平弹开

                // 施加反弹力
                playerRb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
