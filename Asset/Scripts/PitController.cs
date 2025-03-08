using UnityEngine;
using System.Collections;

public class PitController : MonoBehaviour
{
    public bool isFilled = false;  // 是否被填充
    public Material defaultMaterial; // 坑的原始材质
    public GameObject boxPrefab; 
    private GameObject filledBox = null; // 记录填充的箱子
    public GameManager gameManager;
    public Material pinkMaterial;
    private Vector3 Zoffset = new Vector3(0, 0, 1);
    private bool isOP = false;
    void Start()
    {
        // 确保坑只在 TopDown 视角下可见
        //gameObject.SetActive(gameManager.isTopDownView);
    }

    void Update()
    {
     
    }

    void OnTriggerEnter(Collider other)
    {
        if (isFilled) return; // 坑已经被填充，不执行任何操作
        if (isOP) return;
        // 玩家掉进坑，触发游戏结束
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家掉入坑！游戏结束");
            GameOverManager.instance.ShowGameOver(false);
        }

        // 如果箱子放到坑上，则填充
        if (other.CompareTag("Pickable"))
        {
            FillPit(other.gameObject);
        }
    }

  public  void FillPit(GameObject box)
    {
        isFilled = true;
        filledBox = box;

        // 让箱子的位置对齐到坑
        box.transform.position = transform.position+Zoffset;
        box.GetComponent<BoxController>().enabled = false; // 禁用箱子脚本，防止被再次拾取
        
        Debug.Log("坑被填充");

        // 播放填坑动画
        StartCoroutine(SmoothFillPit(box));
        
        // 让坑变成普通地面
        Renderer pitRenderer = GetComponent<Renderer>();
        if (pitRenderer != null && pinkMaterial != null)
        {
            pitRenderer.material = pinkMaterial;
        }
        gameObject.layer = LayerMask.NameToLayer("pitWithBox"); // 让坑变成普通地面
        GetComponent<BoxCollider>().isTrigger = false; // 让碰撞恢复为实体
    }

    IEnumerator SmoothFillPit(GameObject box)
    {
        Vector3 startScale = box.transform.localScale;
        Vector3 endScale = new Vector3(1, 0.1f, 1); // 让箱子变扁，填充坑
        Vector3 startPosition = box.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -0.4f, 0); // 让箱子稍微沉入坑里
        float duration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            box.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            box.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            yield return null;
        }

        // 确保最终状态正确
        box.transform.localScale = endScale;
        box.transform.position = endPosition;

        // **动画结束后销毁箱子**
        Destroy(box);
    }
    public GameObject ExtractBox()
    {
        if (isFilled && boxPrefab != null)
        {
            isFilled = false;
            isOP = true;
            // 生成新的箱子
            GameObject newBox = Instantiate(boxPrefab, transform.position - Zoffset * 0.5f, Quaternion.identity);
            Debug.Log("从坑里取出了箱子");

            // 还原坑的状态
            Renderer pitRenderer = GetComponent<Renderer>();
            if (pitRenderer != null && defaultMaterial != null)
            {
                pitRenderer.material = defaultMaterial; // 恢复原始材质
            }

            gameObject.layer = LayerMask.NameToLayer("Pit"); // 让坑回到普通状态
            GetComponent<BoxCollider>().isTrigger = true; // 让坑恢复为陷阱



            StartCoroutine(ResetIsOP());
            return newBox;
        }
        return null;
    }
    private IEnumerator ResetIsOP()
    {
        yield return new WaitForSeconds(0.8f); // 等待 0.8 秒
        isOP = false;
        Debug.Log("isOP 已重置为 false");
    }
}