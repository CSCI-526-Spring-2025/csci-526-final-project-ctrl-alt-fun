using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public GameManager gameManager;
     // ��ǰ�Ƿ�Ϊ TopDown ģʽ
    private Rigidbody rb;
    private Collider boxCollider;
    private bool isPickedUp = false;
    private Transform playerTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();
        UpdateBoxState();
    }

    void Update()
    {
        // ����ģʽ�л�״̬
        UpdateBoxState();

        // �����ʰȡ���������ɫλ���غ�
        if (isPickedUp && playerTransform != null)
        {
            // ֱ��ʹ���������꣬ʹ���Ӻͽ�ɫ�غ�
            transform.position = playerTransform.position;
        }
    }

    // �������ӵ�״̬
    void UpdateBoxState()
    {
        if (gameManager.isTopDownView)
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
        if (gameManager.isTopDownView)
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
        if (gameManager.isTopDownView && isPickedUp)
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
            boxCollider.isTrigger = false; // �ָ���ײ
        }
    }
}