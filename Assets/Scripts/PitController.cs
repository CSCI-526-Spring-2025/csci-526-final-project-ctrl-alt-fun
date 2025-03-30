using UnityEngine;
using System.Collections;

public class PitController : MonoBehaviour
{
    public bool isFilled = false;  // �Ƿ����
    public Material defaultMaterial; // �ӵ�ԭʼ����
    public GameObject boxPrefab; 
    private GameObject filledBox = null; // ��¼��������
    public GameManager gameManager;
    public Material pinkMaterial;
    private Vector3 Zoffset = new Vector3(0, 0, 1);
    void Start()
    {
        // ȷ����ֻ�� TopDown �ӽ��¿ɼ�
        //gameObject.SetActive(gameManager.isTopDownView);
    }

    void Update()
    {
     
    }

    void OnTriggerEnter(Collider other)
    {
        if (isFilled) return; // ���Ѿ�����䣬��ִ���κβ���
        if (isOP) return;
        // ��ҵ����ӣ�������Ϸ����
        if (other.CompareTag("Player"))
        {
            // End an analytics session
            Vector3 position = other.transform.position;
            string reason = "Pit";
            if (AnalyticsManager.instance != null) {
                AnalyticsManager.instance.AddLossEvent(reason, position);
            }
            Debug.Log("��ҵ���ӣ���Ϸ����");
            GameOverManager.instance.ShowGameOver(false);
        }

        // ������ӷŵ����ϣ������
        if (other.CompareTag("Pickable"))
        {
            FillPit(other.gameObject);
        }
    }

  public void FillPit(GameObject box)
    {
        isFilled = true;
        filledBox = box;

        // �����ӵ�λ�ö��뵽��
        box.transform.position = transform.position+Zoffset;
        box.GetComponent<BoxController>().enabled = false; // �������ӽű�����ֹ���ٴ�ʰȡ
        
        Debug.Log("�ӱ����");

        // ������Ӷ���
        StartCoroutine(SmoothFillPit(box));
        
        // �ÿӱ����ͨ����
        Renderer pitRenderer = GetComponent<Renderer>();
        if (pitRenderer != null && pinkMaterial != null)
        {
            pitRenderer.material = pinkMaterial;
        }
        gameObject.layer = LayerMask.NameToLayer("pitWithBox"); // �ÿӱ����ͨ����
        GetComponent<BoxCollider>().isTrigger = false; // ����ײ�ָ�Ϊʵ��
    }

    IEnumerator SmoothFillPit(GameObject box)
    {
        Vector3 startScale = box.transform.localScale;
        Vector3 endScale = new Vector3(1, 0.1f, 1); // �����ӱ�⣬����
        Vector3 startPosition = box.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -0.4f, 0); // ��������΢�������
        float duration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            box.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            box.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            yield return null;
        }

        // ȷ������״̬��ȷ
        box.transform.localScale = endScale;
        box.transform.position = endPosition;

        // **������������������**
        Destroy(box);
    }
    public GameObject ExtractBox()
    {
        if (isFilled && boxPrefab != null)
        {
            isFilled = false;
            isOP = true;
            // �����µ�����
            GameObject newBox = Instantiate(boxPrefab, transform.position - Zoffset * 0.5f, Quaternion.identity);
            Debug.Log("�ӿ���ȡ��������");

            // ��ԭ�ӵ�״̬
            Renderer pitRenderer = GetComponent<Renderer>();
            if (pitRenderer != null && defaultMaterial != null)
            {
                pitRenderer.material = defaultMaterial; // �ָ�ԭʼ����
            }

            gameObject.layer = LayerMask.NameToLayer("Pit"); // �ÿӻص���ͨ״̬
            GetComponent<BoxCollider>().isTrigger = true; // �ÿӻָ�Ϊ����



            
            return newBox;
        }
        return null;
    }

    private IEnumerator ResetIsOP()
    {
        yield return new WaitForSeconds(0.8f); // �ȴ� 0.8 ��
        isOP = false;
        Debug.Log("isOP ������Ϊ false");
    }
}