using UnityEngine;

public class Bullet3D : MonoBehaviour
{
    public float bulletSpeed = 10f;      // �ӵ���ʼ�ٶ�
    public float forceMagnitude = 10f;   // ʩ�ӵĳ������С

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        // ������������ȷ��û����ק
        rb.drag = 0f;
        rb.angularDrag = 0f;
        // ���ó�ʼ�ٶ�
        rb.velocity = transform.forward * bulletSpeed;
        // 6����Զ������ӵ�
       
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDirection = transform.forward;
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }
        
    }
}
