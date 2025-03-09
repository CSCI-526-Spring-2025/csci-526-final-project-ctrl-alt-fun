using UnityEngine;

public class ShootingObstacle3D : MonoBehaviour
{
    [Header("bullet setting")]
    public GameObject bulletPrefab;     // �ӵ�Ԥ����
    public Transform shootPoint;        // �����ӵ���λ�úͷ���
    public float bulletSpeed = 10f;     // �ӵ��ƶ��ٶ�

    [Header("shooting setting")]
    public float shootInterval = 2f;    // ���������룩

    private float shootTimer;

    void Start()
    {
        shootTimer = shootInterval;
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    void Shoot()
    {
        // ��ָ��λ��ʵ�����ӵ�
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        // ���ӵ�����ٶȣ���������ӵ����� shootPoint ��ǰ�����䣩
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * bulletSpeed;
        }
    }
}
