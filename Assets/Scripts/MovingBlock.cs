using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public float moveSpeed = 4f;  // �ƶ��ٶ�
    public float moveRange = 3f;  // ����ƶ���Χ
    public float bounceForce = 20f; // ��ײʱ����ҵķ�����

    private Vector3 startPos;
    private int direction = 1; // 1 = ����, -1 = ����

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // �����µ� Y λ��
        float newY = transform.position.y + direction * moveSpeed * Time.deltaTime;

        // �����ƶ���Χ������
        if (Mathf.Abs(newY - startPos.y) > moveRange)
        {
            direction *= -1;
        }

        // �ƶ�
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // ��������� MovingBlock ʱ������һ��������
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // ���㷴������
                Vector3 bounceDirection = (collision.transform.position - transform.position).normalized;
                bounceDirection.y = 0.1f; // ������һ�����ϵ�����������ȫˮƽ����

                // ʩ�ӷ�����
                playerRb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
