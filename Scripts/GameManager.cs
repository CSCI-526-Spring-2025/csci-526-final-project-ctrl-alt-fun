using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GameObject[] boxes;
    public GameObject topDownCharacter;
    public GameObject platformerCharacter;
    public Camera mainCamera;
    public Light directionalLight;

    private TopDownController topDownController;
    private PlatformerController platformerController;

    public bool isTopDownView {get; private set;} = false;

    // �������������λ�úͽǶ�
    // public Vector3 topDownPosition = new Vector3(0, 10, -10);
    // public Vector3 topDownRotation = new Vector3(45, 0, 0);
    public Vector3 topDownPosition = new Vector3(-4, -3, -16);
    public Vector3 topDownRotation = new Vector3(-10, 10, 0);

    // public Vector3 platformerPosition = new Vector3(0, 2, -10);
    // public Vector3 platformerRotation = new Vector3(0, 0, 0);
    public Vector3 platformerPosition = new Vector3(0, 0, 0);
    public Vector3 platformerRotation = new Vector3(0, 0, 0);

    public float cameraTransitionSpeed = 5f;

    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;

    void Start()
    {
        platformerController = platformerCharacter.GetComponent<PlatformerController>();
        topDownController = topDownCharacter.GetComponent<TopDownController>();

        // ��ʼ״̬��TopDown �ӽ�
        SwitchToPlatformerView();
    }

    void Update()
    {
        // ��� Shift ���л��ӽ�
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isTopDownView = !isTopDownView;
            if (isTopDownView)
            {
                SwitchToTopDownView();
            }
            else
            {
                SwitchToPlatformerView();
            }
        }

        // ������ɶ���
        SmoothCameraTransition();
    }
    // �л��� TopDown �ӽ�
    void SwitchToTopDownView()
    {
        // ���� TopDown ģʽ
        topDownController.enabled = true;
        SetCharacterCollision(topDownCharacter, true);

        // ���� Platformer ģʽ
        platformerController.enabled = false;
        SetCharacterCollision(platformerCharacter, false);

        // ͸��������
        SetCharacterTransparency(topDownCharacter, 1f);
        SetCharacterTransparency(platformerCharacter, 0.2f);

        // ���Ŀ��λ�úͽǶ�
        // targetCameraPosition = topDownCharacter.transform.position + topDownPosition;
        targetCameraPosition = topDownPosition;
        targetCameraRotation = Quaternion.Euler(topDownRotation);

        if (directionalLight != null)
        {
            directionalLight.transform.position = new Vector3(0, 18, -15);
            directionalLight.transform.rotation = Quaternion.Euler(53, 26, -133);
        }
    }

    // �л��� Platformer �ӽ�
    void SwitchToPlatformerView()
    {

        // Debug.Log("SwitchToPlatformerView");
        // ���� Platformer ģʽ
        platformerController.enabled = true;
        SetCharacterCollision(platformerCharacter, true);

        // ���� TopDown ģʽ
        topDownController.enabled = false;
        SetCharacterCollision(topDownCharacter, false);

        // ͸��������
        SetCharacterTransparency(topDownCharacter, 0.2f);
        SetCharacterTransparency(platformerCharacter, 1f);

        // ���Ŀ��λ�úͽǶ�
        targetCameraPosition = platformerCharacter.transform.position / 2 + platformerPosition;
        // targetCameraRotation = Quaternion.Euler(platformerRotation);
        // targetCameraPosition = platformerPosition;
        targetCameraRotation = Quaternion.Euler(platformerRotation);

        if (directionalLight != null)
        {
            directionalLight.transform.position = new Vector3(0, 0, -15);
            directionalLight.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // ���ý�ɫ͸����
    void SetCharacterTransparency(GameObject character, float alpha)
    {
        Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }

    // ���û���ý�ɫ����ײ������Ч��
    void SetCharacterCollision(GameObject character, bool isEnabled)
    {
        Collider[] colliders = character.GetComponentsInChildren<Collider>();
        Rigidbody rb = character.GetComponent<Rigidbody>();

        // ����� TopDown ģʽ��������Ҫ��͸��ģʽ��
        if (isEnabled == false)
        {
            // ���� Rigidbody ����������
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.detectCollisions = false;
            }

            // ����������ײ
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
        else
        {
            // Debug.Log("SetCharacterCollision(GameObject Plat, bool true)");
            // ���� Rigidbody ����������
            if (rb != null)
            {
                // Debug.Log("rb");
                rb.isKinematic = false;
                if (isTopDownView)
                    
                {
                    // Debug.Log("isTopDownView");
                    rb.useGravity = false;
                }
                else
                {
                    // Debug.Log("!isTopDownView");
                    rb.useGravity=true;
                }
                rb.detectCollisions = true;
            }

            // ����������ײ
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }

    void SmoothCameraTransition()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraTransitionSpeed);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetCameraRotation, Time.deltaTime * cameraTransitionSpeed);
    }
}