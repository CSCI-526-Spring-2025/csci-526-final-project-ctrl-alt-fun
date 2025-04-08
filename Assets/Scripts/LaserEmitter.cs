using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    public float maxDistance = 20f;
    public Vector3 laserDirection = Vector3.forward;
    public LayerMask laserBlockLayers;

    private LineRenderer lineRenderer;
    private bool hasHitPlayer = false; // ✅ 新增：防止多次触发

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
        Vector3 origin = transform.position + offset;
        Vector3 direction = laserDirection.normalized;

        RaycastHit hit;
        Vector3 endPosition = origin + direction * maxDistance;

        if (Physics.Raycast(origin, direction, out hit, maxDistance, laserBlockLayers))
        {
            endPosition = hit.point;

            if (hit.collider.CompareTag("Player"))
            {
                if (!hasHitPlayer)
                {
                    hasHitPlayer = true; // ✅ 标记已触发
                    Debug.Log("Player hit by laser!");

                    // Analystics
                    Vector3 position = hit.collider.transform.position;
                    string reason = "Laser";

                    if (AnalyticsManager.instance != null)
                    {
                        // Debug.Log("Laser analysis upload");
                        AnalyticsManager.instance.AddLossEvent(reason, position);
                    }

                    if (GameOverManager.instance != null)
                    {
                        GameOverManager.instance.ShowGameOver(false);
                    }
                }
            }
            else
            {
                // 如果打到了别的对象，确保允许之后再次触发玩家命中
                hasHitPlayer = false;
            }
        }
        else
        {
            hasHitPlayer = false; // 没打到任何东西也要重置
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPosition);
    }
}
