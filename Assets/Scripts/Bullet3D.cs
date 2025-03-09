using UnityEngine;

public class Bullet3D : MonoBehaviour
{
    public float bulletSpeed = 10f;      // 子弹初始速度
    public float forceMagnitude = 10f;   // 施加的冲击力大小

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        // 禁用重力，并确保没有拖拽
        rb.drag = 0f;
        rb.angularDrag = 0f;
        // 设置初始速度
        rb.velocity = transform.forward * bulletSpeed;
        // 6秒后自动销毁子弹
       
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
