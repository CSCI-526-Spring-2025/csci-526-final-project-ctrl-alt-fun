using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
   

    // ��ǰ�Ƿ�Ϊ TopDown ģʽ
    private Rigidbody rb;
    private Collider boxCollider;
    private bool isPickedUp = false;
    private Transform playerTransform;
    private Vector3 Zoffset = new Vector3(0, 0, 1);
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();

      
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();
        
        UpdateBoxState();
    }

    void Update()
    {
        if (GameOverManager.instance != null && GameOverManager.instance.isGamePaused) return;
        // ����ģʽ�л�״̬
        UpdateBoxState();

        // �����ʰȡ���������ɫλ���غ�
        if (isPickedUp && playerTransform != null)
        {
            // ֱ��ʹ���������꣬ʹ���Ӻͽ�ɫ�غ�
            transform.position = playerTransform.position - Zoffset;
        }
    }

    // �������ӵ�״̬
    void UpdateBoxState()
    {
        if (GameManager.Instance.isTopDownView)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            boxCollider.isTrigger = isPickedUp; // ��ʰȡ�������ײ
        }
        else
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            boxCollider.isTrigger = false; // Platform ģʽ�»ָ���ײ
            rb.useGravity = false;
            ReleaseBox();
        }
    }

    // ʰȡ����
    public void PickUpBox(Transform player)
    {
        if (GameManager.Instance.isTopDownView)
        {
            isPickedUp = true;
            playerTransform = player;
            transform.position = player.position; // ��������λ��
            boxCollider.isTrigger = true; // ������ײ
        }
    }

    // ��������
    public void PlaceBox(Vector3 placePosition)
    {
        if (GameManager.Instance.isTopDownView && isPickedUp)
        {
            isPickedUp = false;
            playerTransform = null;
            transform.position = placePosition;
            boxCollider.isTrigger = false; // �ָ���ײ
        }
    }

    // �ͷ����ӣ��л��� Platform ģʽʱ���ã�
    public void ReleaseBox()
    {
        if (isPickedUp)
        {
            isPickedUp = false;
            playerTransform = null;
            Vector3[] directions = new Vector3[]
            {
            Vector3.right,  // ��
            Vector3.left,   // ��
            Vector3.up,     // ��
            Vector3.down    // ��
            };

            foreach (Vector3 dir in directions)
            {
                Vector3 potentialPosition = transform.position + dir + Zoffset;
                Collider[] hitColliders = Physics.OverlapBox(potentialPosition, Vector3.one * 0.4f, Quaternion.identity);

                // �����λ��û���������壬���������
                if (hitColliders.Length == 0)
                {
                    transform.position = potentialPosition;
                    boxCollider.isTrigger = false;
                    Debug.Log("���ӷ�����: " + potentialPosition);
                    return; // ���óɹ����˳�����
                }
            }

            // ����ĸ����򶼱�ռ�ã����ӱ���ԭ��
            Debug.Log("�޷��������ӣ����з��򶼱�ռ�ݣ�");
            
            boxCollider.isTrigger = false;
        }
    }
}