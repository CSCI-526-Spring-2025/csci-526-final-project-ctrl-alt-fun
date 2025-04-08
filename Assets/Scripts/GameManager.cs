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

    public bool isTopDownView { get; private set; } = false;

    private Vector3 topDownPosition = new Vector3(0, -9f, -7.5f);
    private Vector3 topDownRotation = new Vector3(-30, 0, 0);

    private Vector3 platformerPosition = new Vector3(0, -5, -10);
    private Vector3 platformerRotation = new Vector3(0, 0, 0);

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

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.6f, 0.6f, 0.6f); // 柔和灰色环境光
        RenderSettings.reflectionIntensity = 1f;
        DynamicGI.UpdateEnvironment();

        // Initialize controllers if characters exist
        if (topDownCharacter != null)
        {
            topDownController = topDownCharacter.GetComponent<TopDownController>();
        }

        if (platformerCharacter != null)
        {
            platformerController = platformerCharacter.GetComponent<PlatformerController>();
        }

        // 自动选择初始视角
        AutoSelectInitialView();

        // Start an analytics session
        sessionId = Guid.NewGuid().ToString();
        levelId = SceneManager.GetActiveScene().name;
        shiftCount = 0;
        string viewBeforeEvent = isTopDownView ? "TopDown" : "Platformer";
        Vector3 position = isTopDownView && topDownCharacter != null ?
            topDownCharacter.transform.position :
            platformerCharacter != null ? platformerCharacter.transform.position : Vector3.zero;
        if (AnalyticsManager.instance != null)
        {
            AnalyticsManager.instance.AddAnalyticsEvent(
                sessionId: sessionId,
                eventType: "Start",
                levelId: levelId,
                timestamp: System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                eventSequence: -1,
                viewBeforeEvent: viewBeforeEvent,
                reason: "N/A",
                position: position
            );
        }
        // Recording ends
    }

    private void AutoSelectInitialView()
    {
        bool hasTopDown = topDownCharacter != null;
        bool hasPlatformer = platformerCharacter != null;

        if (hasTopDown && hasPlatformer)
        {
            // 双角色模式，默认使用Platformer视角
            SwitchToPlatformerView();
            Debug.Log("双角色模式，默认使用Platformer视角");
        }
        else if (hasTopDown)
        {
            // 只有TopDown角色
            isTopDownView = true;
            SwitchToTopDownView();
            Debug.Log("单人模式，使用TopDown视角");
        }
        else if (hasPlatformer)
        {
            // 只有Platformer角色
            isTopDownView = false;
            SwitchToPlatformerView();
            Debug.Log("单人模式，使用Platformer视角");
        }
        else
        {
            Debug.LogError("场景中没有可用的玩家角色！");
        }
    }

    void Update()
    {
        if (GameOverManager.instance != null && GameOverManager.instance.isGamePaused) return;

        // Check if we have both characters before allowing view switching
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) &&
            topDownCharacter != null && platformerCharacter != null)
        {
            // Record a shift event for analytics
            shiftCount += 1;
            Vector3 position = isTopDownView ? topDownCharacter.transform.position : platformerCharacter.transform.position;
            string viewBeforeEvent = isTopDownView ? "TopDown" : "Platformer";
            if (AnalyticsManager.instance != null)
            {
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
            // Recording ends

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

        // Smooth camera transition
        SmoothCameraTransition();
    }

    void SwitchToTopDownView()
    {
        if (topDownCharacter == null)
        {
            Debug.LogWarning("Cannot switch to TopDown view - no TopDown character assigned!");
            return;
        }

        //UpdatePortalStatus();

        if (topDownController != null)
        {
            topDownController.enabled = true;
        }
        SetCharacterCollision(topDownCharacter, true);

        // Disable Platformer mode if it exists
        if (platformerCharacter != null)
        {
            if (platformerController != null)
            {
                platformerController.enabled = false;
            }
            SetCharacterCollision(platformerCharacter, false);
            SetCharacterTransparency(platformerCharacter, 0.2f);
        }

        SetCharacterTransparency(topDownCharacter, 1f);

        targetCameraPosition = topDownPosition;
        targetCameraRotation = Quaternion.Euler(topDownRotation);

        if (directionalLight != null)
        {
            directionalLight.transform.position = new Vector3(0, -10, -15);
            directionalLight.transform.rotation = Quaternion.Euler(53, 26, -133);
        }

        // Set material transparency
        if (TransparentGridMaterial != null)
        {
            SetMaterialTransparency(TransparentGridMaterial, 1.0f);
        }
    }

    void checkBoxState()
    {
        if (topDownController == null || topDownController.pickedBox == null) { return; }
        if (topDownController.TryPlaceBox()) { return; }
        BoxController boxController = topDownController.pickedBox.GetComponent<BoxController>();
        if (boxController != null)
        {
            boxController.ReleaseBox();
            topDownController.pickedBox = null;
        }
        return;
    }

    void SwitchToPlatformerView()
    {
        if (platformerCharacter == null)
        {
            Debug.LogWarning("Cannot switch to Platformer view - no Platformer character assigned!");
            return;
        }

        if (topDownCharacter != null)
        {
            checkBoxState();
        }

        if (platformerController != null)
        {
            platformerController.enabled = true;
        }
        SetCharacterCollision(platformerCharacter, true);

        // Disable TopDown mode if it exists
        if (topDownCharacter != null)
        {
            if (topDownController != null)
            {
                topDownController.enabled = false;
            }
            SetCharacterCollision(topDownCharacter, false);
            SetCharacterTransparency(topDownCharacter, 0.2f);
        }

        SetCharacterTransparency(platformerCharacter, 1f);

        // Set camera target position and rotation
        targetCameraPosition = platformerPosition;
        targetCameraRotation = Quaternion.Euler(platformerRotation);

        if (directionalLight != null)
        {
            directionalLight.transform.position = new Vector3(0, 0, -25);
            directionalLight.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Set material transparency
        if (TransparentGridMaterial != null)
        {
            SetMaterialTransparency(TransparentGridMaterial, 1.0f);
        }
    }

    void SetMaterialTransparency(Material material, float alpha)
    {
        if (material == null) return;

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

    void SetCharacterTransparency(GameObject character, float alpha)
    {
        if (character == null) return;

        Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }

    void SetCharacterCollision(GameObject character, bool isEnabled)
    {
        if (character == null) return;

        Collider[] colliders = character.GetComponentsInChildren<Collider>();
        Rigidbody rb = character.GetComponent<Rigidbody>();

        if (isEnabled == false)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.detectCollisions = false;
            }

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
        else
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                if (isTopDownView)
                {
                    rb.useGravity = false;
                }
                else
                {
                    rb.useGravity = true;
                }
                rb.detectCollisions = true;
            }

            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }

    void SmoothCameraTransition()
    {
        if (mainCamera == null) return;

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraTransitionSpeed);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetCameraRotation, Time.deltaTime * cameraTransitionSpeed);
    }


    public TMPro.TMP_Text playerCountText;
    public int totalPlayers = 1;

    public void UpdateGoalProgress(int currentReached)
    {
        if (playerCountText != null)
        {
            playerCountText.text = $"□■: {currentReached} / {totalPlayers}";
        }
    }
}