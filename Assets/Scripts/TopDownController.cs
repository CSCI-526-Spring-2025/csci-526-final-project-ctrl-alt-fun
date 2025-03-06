using System;
using UnityEngine;

public class TopDownController : MonoBehaviour
{

        public float gridSize = 1f; // 每次移动一格
        public float moveSpeed = 4f; // 移动速度
        public LayerMask obstacleLayer; // 障碍物层
        public LayerMask boxLayer; // 箱子层

        private Vector3 moveDirection = Vector3.zero;
        private Vector3 faceDirection = Vector3.right; // 默认面朝右
        public GameObject pickedBox = null; // 当前拾取的箱子
        private LineRenderer lineRenderer;
        private LayerMask combinedLayer;
        private Vector3 Zoffset = new Vector3(0, 0, 1);
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
    //    void TryPickUpBox()
    //{
    //    // 从角色当前位置发出射线，朝 faceDirection 方向
    //    float gridSize = 1f;
    //    Ray ray = new Ray(transform.position, faceDirection);
    //    RaycastHit hit;
    //    Ray ray4pit = new Ray(transform.position + Zoffset, faceDirection);
    //    if (Physics.Raycast(ray4pit, out hit, gridSize))
    //    {
    //        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PitWithBox"))
    //        {
    //            PitController pitController = hit.collider.GetComponent<PitController>();
    //            if (pitController != null)
    //            {
    //                GameObject newBox = pitController.ExtractBox(); // 从坑里取出箱子
    //                if (newBox != null)
    //                {
    //                    pickedBox = newBox; // 记录新生成的箱子
    //                    BoxController boxController = newBox.GetComponent<BoxController>();
    //                    boxController.PickUpBox(transform);
    //                    return;
    //                }
    //            }
    //        }
    //    }

    //        // 射线检测的距离为 gridSize，可以根据需要调整
    //        if (Physics.Raycast(ray, out hit, gridSize))
    //    {

    //        // 检查射线碰撞到的物体是否带有 "Pickable" 标签
    //        if (hit.collider.CompareTag("Pickable"))
    //        {
    //            BoxController boxController = hit.collider.GetComponent<BoxController>();

    //            if (boxController != null)
    //            {
    //                boxController.PickUpBox(transform);
    //                pickedBox = hit.collider.gameObject; // 记录被拾取的物体
    //            }
    //            else if (boxController == null)
    //            {
    //                boxController = hit.collider.GetComponentInParent<BoxController>();
    //                if (boxController != null)
    //                {
    //                    boxController.PickUpBox(transform);
    //                    pickedBox = hit.collider.transform.parent.gameObject; // 记录被拾取的物体
    //                }
    //            }
    //        }
    //    }
    //}

    void TryPickUpBox()
    {
        float gridSize = 1f;
        Vector3 checkPosition = transform.position + faceDirection * gridSize; // 计算前方的检测位置

        Collider[] hitColliders = Physics.OverlapBox(checkPosition, Vector3.one * 0.4f, Quaternion.identity);

        foreach (Collider hit in hitColliders)
        {
            
            if (hit.gameObject.layer == LayerMask.NameToLayer("pitWithBox"))
            {
                PitController pitController = hit.GetComponent<PitController>();
                if (pitController != null)
                {
                    GameObject newBox = pitController.ExtractBox(); // 从坑里取出箱子
                    if (newBox != null)
                    {
                        pickedBox = newBox; // 记录新生成的箱子
                        BoxController boxController = newBox.GetComponent<BoxController>();
                       
                        if (boxController != null)
                        {
                            Debug.Log("找到 BoxController");
                        }
                        // 这里可能 boxController 被覆盖或者丢失
                        boxController.PickUpBox(transform);
                        return; 
                    }
                }
            }

            
            if (hit.CompareTag("Pickable"))
            {
                BoxController boxController = hit.GetComponent<BoxController>();

                if (boxController != null)
                {
                    boxController.PickUpBox(transform);
                    pickedBox = hit.gameObject; // 记录被拾取的物体
                }
                else
                {
                    boxController = hit.GetComponentInParent<BoxController>();
                    if (boxController != null)
                    {
                        boxController.PickUpBox(transform);
                        pickedBox = hit.transform.parent.gameObject; // 记录被拾取的物体
                    }
                }
                return; 
            }
        }
    }
    // 尝试放置箱子
    public  bool TryPlaceBox()
    {
        Vector3 placePosition = transform.position + faceDirection * gridSize;
        Collider[] hitColliders = Physics.OverlapBox(placePosition, Vector3.one * 0.4f, Quaternion.identity);
        foreach (Collider hit in hitColliders)
        {
            PitController pit = hit.GetComponent<PitController>();
            if (pit != null && !pit.isFilled) // 目标是坑且未填充
            {
                if (pickedBox != null)
                {
                    pit.FillPit(pickedBox); // 让坑填充
                    pickedBox = null;
                    return true;
                }
            }
        }
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
                    return true;
                }
            }
        }
        return false;
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
