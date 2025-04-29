using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BidirectionalPortal : MonoBehaviour
{

    [Tooltip("The paired portal (another portal that this one connects to)")]
    public BidirectionalPortal linkedPortal; // �����Ĵ�����

    [Tooltip("Cooldown time to prevent immediate re-teleporting")]
    public float teleportCooldown = 0.5f; // ������ȴʱ��

    [Tooltip("Tags that can be teleported (e.g., Player, Enemy)")]
    public List<string> targetTags ; // �ɴ��͵Ķ����б�

    private bool isTeleporting = false; // �Ƿ�����ȴ״̬
    private Vector3 portalDirection; // ������Ĵ����ŷ���

    private void Start()
    {
        // ��ȡ��һ�㸸���壨�� `portal1` / `portal2`��
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // ���㷽��realPortal - ����������
            portalDirection = (transform.position - parentTransform.position).normalized;
        }
        else
        {
            Debug.LogError("TeleportPortal must have a parent object!");
            portalDirection = transform.forward; // ���׷�������ֹ����
        }
    }



    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.WakeUp(); // ǿ�ƻ��Ѹ��壬��ֹ��������ϵͳ����
            Debug.Log("������봫���ţ��ѻ��ѣ���rb.velocity = " + rb.velocity);
        }
        // 1. ���˵������� tag ������
        if (!targetTags.Contains(other.tag)) return;
        // 2. ȷ������Ч�Ĵ�����
        if (linkedPortal == null || isTeleporting) return;
        if (GameManager.Instance != null && GameManager.Instance.isTopDownView) return; 
        // 3. ȷ����ҽӴ����� "��ȷ�ķ���"��ǳ��ɫ�����棩
        if (!IsCorrectSurface(other)) return;

        //Rigidbody rb = other.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            Vector3 incomingVelocity = rb.velocity;

            StartCoroutine(TeleportWithVelocity(other, incomingVelocity));

        }
        else
        {
            StartCoroutine(TeleportWithVelocity(other, Vector3.zero)); // û�и�������
        }
    }

    private bool IsCorrectSurface(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return false; // �������û�и��壬��������

        Vector3 incomingVelocity = rb.velocity.normalized; // ��һ���ٶȷ���
        float angle = Vector3.Angle(incomingVelocity, portalDirection); // ����Ƕ�

        // ֻ�нǶ��� 90�� ~ 180�� ֮�䣬��������
        return (angle >= 90f && angle <= 270f);
    }

    private IEnumerator TeleportWithVelocity(Collider objectToTeleport, Vector3 incomingVelocity)
    {
        isTeleporting = true; // ������ȴ
        linkedPortal.isTeleporting = true; // ��һ��Ҳ������ȴ

        // ֱ�Ӵ��͵� `linkedPortal` ��λ��
        Vector3  offset = linkedPortal.portalDirection * 0.5f;
        objectToTeleport.transform.position = linkedPortal.transform.position + offset ;

        // �� `linkedPortal` ������ķ������ `transform.forward`
        Vector3 newVelocity = linkedPortal.portalDirection * incomingVelocity.magnitude ;
        Rigidbody rb = objectToTeleport.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = newVelocity; // �����·�����ٶ�
        }

        yield return new WaitForSeconds(teleportCooldown); // ��ȴʱ��

        isTeleporting = false;
        linkedPortal.isTeleporting = false;
    }
}