using UnityEngine;

[ExecuteInEditMode] // 让网格在 Scene 视图可见
public class GridGizmos : MonoBehaviour
{
    public int gridSize = 100; // 100x100 的网格
    public float planeSize = 10.0f; // Plane 实际大小（Scale = 10,1,10）
    public Color gridColor = Color.gray; // 网格颜色

    void OnDrawGizmos()
    {
        Gizmos.color = gridColor;
        Vector3 planePosition = transform.position;

        // 计算每个格子的大小
        float cellSize = planeSize / gridSize;

        // **绘制 XY 平面的网格**
        for (int x = 0; x <= gridSize; x++)
        {
            Vector3 start = planePosition + new Vector3(x * cellSize - planeSize / 2, -planeSize / 2, 0);
            Vector3 end = start + new Vector3(0, planeSize, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridSize; y++)
        {
            Vector3 start = planePosition + new Vector3(-planeSize / 2, y * cellSize - planeSize / 2, 0);
            Vector3 end = start + new Vector3(planeSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
