using UnityEngine;

public class TopDownController : MonoBehaviour
{

        public float gridSize = 1f; // 每次移动一格
        public float moveSpeed = 4f; // 移动速度
        public LayerMask obstacleLayer; // 障碍物层
        public LayerMask boxLayer; // 箱子层

        private Vector3 moveDirection = Vector3.zero;
        private Vector3 faceDirection = Vector3.right; // 默认面朝右
        private GameObject pickedBox = null; // 当前拾取的箱子
        private LineRenderer lineRenderer;
        private LayerMask combinedLayer;
    void Start()
        {
        combinedLayer = obstacleLayer | boxLayer;
            // 初始化 LineRenderer 用于虚线框
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.loop = true;
            lineRenderer.positionCount = 4;
            lineRenderer.enabled = false;
        }

        void Update()
        {
            HandleInput();
            DrawFaceDirection();
        }

        // 处理输入
        void HandleInput()
        {
            // 移动
            if (!Input.GetKey(KeyCode.Space))
            {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                AttemptMove(Vector3.up);
                faceDirection = Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                AttemptMove(Vector3.down);
                faceDirection = Vector3.down;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AttemptMove(Vector3.left);
                faceDirection = Vector3.left;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                AttemptMove(Vector3.right);
                faceDirection = Vector3.right;
            }
            }

            // 调整面朝方向
            if (Input.GetKey(KeyCode.Space))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    faceDirection = Vector3.up;
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    faceDirection = Vector3.down;
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    faceDirection = Vector3.left;
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    faceDirection = Vector3.right;
            }

            // 拾取/放置箱子
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (pickedBox == null)
                {
                    Debug.Log("TryPickUpBox");
                    TryPickUpBox();
                }
                else
                {
                    Debug.Log("TryPlaceBox");
                    TryPlaceBox();
                }
            }
        }

        // 尝试移动
        void AttemptMove(Vector3 direction)
        {
            Vector3 targetPosition = transform.position + direction * gridSize;

            // 检查是否有障碍物
            if (!Physics.CheckBox(targetPosition, Vector3.one * 0.4f, Quaternion.identity, combinedLayer))
            {
                transform.position = targetPosition;
            }
            else
            {
                Debug.Log("obs");
            }
        }

    // 尝试拾取箱子
        void TryPickUpBox()
    {
        // 从角色当前位置发出射线，朝 faceDirection 方向
        float gridSize = 1f;
        Ray ray = new Ray(transform.position, faceDirection);
        RaycastHit hit;

        // 射线检测的距离为 gridSize，可以根据需要调整
        if (Physics.Raycast(ray, out hit, gridSize))
        {
            // 检查射线碰撞到的物体是否带有 "Pickable" 标签
            if (hit.collider.CompareTag("Pickable"))
            {
                BoxController boxController = hit.collider.GetComponent<BoxController>();
                if (boxController != null)
                {
                    boxController.PickUpBox(transform);
                    pickedBox = hit.collider.gameObject; // 记录被拾取的物体
                }
            }
        }
    }

    // void TryPickUpBox()
    // {
    //     Vector3 boxPosition = transform.position + faceDirection * gridSize;

    //     Collider[] hits = Physics.OverlapBox(boxPosition, Vector3.one * 0.4f, Quaternion.identity, boxLayer);
    //     if (hits.Length > 0)
    //     {
    //         BoxController boxController = hits[0].GetComponent<BoxController>();
    //         if (boxController != null)
    //         {
    //             boxController.PickUpBox(transform);
    //             pickedBox = hits[0].gameObject; // 记录拾取的箱子
    //         }
    //     }
    // }

    // 尝试放置箱子
    void TryPlaceBox()
    {
        Vector3 placePosition = transform.position + faceDirection * gridSize;

        // 检查目标位置是否有障碍物
        if (!Physics.CheckBox(placePosition, Vector3.one * 0.4f, Quaternion.identity, combinedLayer))
        {
            if (pickedBox != null)
            {
                BoxController boxController = pickedBox.GetComponent<BoxController>();
                if (boxController != null)
                {
                    boxController.PlaceBox(placePosition);
                    pickedBox = null;
                }
            }
        }
    }

    // 绘制面朝方向的虚线框
    void DrawFaceDirection()
        {
            Vector3 boxPosition = transform.position + faceDirection * gridSize;
            lineRenderer.enabled = true;

            Vector3[] corners = new Vector3[4];
            corners[0] = boxPosition + new Vector3(-0.5f, 0.5f, 0.4f);
            corners[1] = boxPosition + new Vector3(0.5f, 0.5f, 0.4f);
            corners[2] = boxPosition + new Vector3(0.5f, -0.5f, 0.4f);
            corners[3] = boxPosition + new Vector3(-0.5f, -0.5f, 0.4f);

            lineRenderer.SetPositions(corners);
        }
    
}
