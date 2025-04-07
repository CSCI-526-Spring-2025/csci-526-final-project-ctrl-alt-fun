using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter1 : MonoBehaviour
{
    public float maxDistance = 20f;


    public Vector3 laserDirection = Vector3.forward;

    public LayerMask laserBlockLayers; // …Ë÷√Œ™ Ground ∫Õ Box ≤„
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
        Vector3 origin = transform.position + offset;
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
