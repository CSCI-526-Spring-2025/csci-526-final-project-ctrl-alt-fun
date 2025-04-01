using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public Material TransparentGridMaterial; // 透明网格材质
    public GameObject[] boxes;
    public GameObject topDownCharacter;
    public GameObject platformerCharacter;
    public Camera mainCamera;
    public Light directionalLight;
    public static GameManager Instance;
    private TopDownController topDownController;
    private PlatformerController platformerController;

    public bool isTopDownView {get; private set;} = false;


    public Vector3 topDownPosition = new Vector3(-4, -3, -60);
    public Vector3 topDownRotation = new Vector3(-10, 10, 0);

    public Vector3 platformerPosition = new Vector3(0, 0, -30);
    public Vector3 platformerRotation = new Vector3(0, 0, 0);

    public float cameraTransitionSpeed = 5f;

    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;
    public List<GameObject> portals;

    public string sessionId;
    public string levelId;
    public int shiftCount;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 如果没有实例，设置当前实例
        }
        else
        {
            Destroy(gameObject); // 如果已有实例，销毁新创建的 GameManager，防止重复
        }
    }

    void Start()
    {
        // Start an analytics session
        sessionId = Guid.NewGuid().ToString();
        levelId = SceneManager.GetActiveScene().name;
        shiftCount = 0;
        // Debug.Log("New analytics session: " + sessionId);
        Vector3 position = isTopDownView ? topDownCharacter.transform.position : platformerCharacter.transform.position;
        if (AnalyticsManager.instance != null) {
            AnalyticsManager.instance.AddAnalyticsEvent(
                sessionId: sessionId, 
                eventType: "Start", 
                levelId: levelId, 
                timestamp: System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
                eventSequence: -1,
                viewBeforeEvent: "N/A",
                reason: "N/A",
                position: position
            );
        }


        platformerController = platformerCharacter.GetComponent<PlatformerController>();
        topDownController = topDownCharacter.GetComponent<TopDownController>();

        SwitchToPlatformerView();
    }

    void Update()
    {
        if (GameOverManager.instance != null && GameOverManager.instance.isGamePaused) return;
        // ��� Shift ���л��ӽ�
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            // Record a shift event for analytics
            shiftCount += 1;
            Vector3 position = isTopDownView ? topDownCharacter.transform.position : platformerCharacter.transform.position;
            string viewBeforeEvent = isTopDownView ? "TopDown" : "Platformer";
            if (AnalyticsManager.instance != null) {
                AnalyticsManager.instance.AddAnalyticsEvent(
                    sessionId: sessionId, 
                    eventType: "Shift", 
                    levelId: levelId, 
                    timestamp: System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
                    eventSequence: shiftCount,
                    viewBeforeEvent: viewBeforeEvent,
                    reason: "N/A",
                    position: position
                );
            }

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

    void SwitchToTopDownView()
    {
        //UpdatePortalStatus();
 
        topDownController.enabled = true;
        SetCharacterCollision(topDownCharacter, true);

        // ���� Platformer ģʽ
        platformerController.enabled = false;
        SetCharacterCollision(platformerCharacter, false);

        SetCharacterTransparency(topDownCharacter, 1f);
        SetCharacterTransparency(platformerCharacter, 0.2f);


        targetCameraPosition = topDownPosition;
        targetCameraRotation = Quaternion.Euler(topDownRotation);

        if (directionalLight != null)
        {
            directionalLight.transform.position = new Vector3(0, 18, -25);
            directionalLight.transform.rotation = Quaternion.Euler(53, 26, -133);
        }
        // 设置材质透明度为 0.3
        SetMaterialTransparency(TransparentGridMaterial, 1.0f);
    }

    void checkBoxState()
    {
        if (topDownController.pickedBox == null) { return; }
        if (topDownController.TryPlaceBox()) { return; }
        BoxController boxController= topDownController.pickedBox.GetComponent<BoxController>();
        if (boxController != null)
        {
            boxController.ReleaseBox();
            topDownController.pickedBox=  null;
        }
        return;
    }
    // �л��� Platformer �ӽ�
    void SwitchToPlatformerView()
    {
        checkBoxState();
        platformerController.enabled = true;
        SetCharacterCollision(platformerCharacter, true);

        // ���� TopDown ģʽ
        topDownController.enabled = false;
        SetCharacterCollision(topDownCharacter, false);

        // ͸��������
        SetCharacterTransparency(topDownCharacter, 0.2f);
        SetCharacterTransparency(platformerCharacter, 1f);

        // ���Ŀ��λ�úͽǶ�
        // targetCameraPosition = platformerCharacter.transform.position + platformerPosition;
        targetCameraPosition = platformerPosition;
        // Debug.Log("targetCameraPosition set to: " + targetCameraPosition);

        targetCameraRotation = Quaternion.Euler(platformerRotation);

        if (directionalLight != null)
        {
            directionalLight.transform.position = new Vector3(0, 0, -25);
            directionalLight.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        // 设置材质透明度为 1.0
        SetMaterialTransparency(TransparentGridMaterial, 1.0f);
    }
    void SetMaterialTransparency(Material material, float alpha)
    {

        if (material.HasProperty("_BaseColor")) // URP
        {
            Color color = material.GetColor("_BaseColor");
            color.a = alpha;
            material.SetColor("_BaseColor", color);
        }
        else if (material.HasProperty("_Color")) // 标准 Shader
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;
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