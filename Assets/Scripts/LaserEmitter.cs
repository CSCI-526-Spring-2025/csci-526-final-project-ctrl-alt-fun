using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    public float maxDistance = 20f;

    [Tooltip("激光方向（例如：右 -> (1,0,0)，上 -> (0,1,0)）")]
    public Vector3 laserDirection = Vector3.forward;

    public LayerMask laserBlockLayers; // 设置为 Ground 和 Box 层
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        FireLaser();
    }

    void FireLaser()
    {
        Vector3 offset = new Vector3(0, 0, -0.25f);
        Vector3 origin = transform.position + offset ;
        Vector3 direction = laserDirection.normalized;

        RaycastHit hit;
        Vector3 endPosition = origin + direction * maxDistance;

        if (Physics.Raycast(origin, direction, out hit, maxDistance, laserBlockLayers))
        {
            endPosition = hit.point;

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player hit by laser!");
                if (GameOverManager.instance != null)
                {
                    GameOverManager.instance.ShowGameOver(false);
                }
            }
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPosition);
    }
}
