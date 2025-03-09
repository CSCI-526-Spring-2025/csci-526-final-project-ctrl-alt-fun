using UnityEngine;

public class ShootingObstacle3D : MonoBehaviour
{
    [Header("bullet setting")]
    public GameObject bulletPrefab;     // 子弹预制体
    public Transform shootPoint;        // 发射子弹的位置和方向
    public float bulletSpeed = 10f;     // 子弹移动速度

    [Header("shooting setting")]
    public float shootInterval = 2f;    // 发射间隔（秒）

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
        // 在指定位置实例化子弹
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        // 给子弹添加速度（这里假设子弹沿着 shootPoint 的前方向发射）
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * bulletSpeed;
        }
    }
}
