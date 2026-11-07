#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Script tự động dựng toàn bộ màn chơi Roll-A-Ball 3D (kèm Cần gạt cảm ứng Điện thoại)
/// Menu: Tools -> Build Roll-A-Ball Game
/// </summary>
public class RollABallSceneBuilder : EditorWindow
{
    [MenuItem("Tools/Build Roll-A-Ball Game", false, 1)]
    public static void BuildCompleteScene()
    {
        CleanupOldObjects();

        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        Material groundMat = CreateOrLoadMaterial("GroundMat", new Color(0.18f, 0.20f, 0.25f));
        Material wallMat   = CreateOrLoadMaterial("WallMat",   new Color(0.45f, 0.50f, 0.58f));
        Material playerMat = CreateOrLoadMaterial("PlayerMat", new Color(0.0f,  0.85f, 0.75f));
        Material pickupMat = CreateOrLoadMaterial("PickupMat", new Color(1.0f,  0.75f, 0.10f));
        Material shieldMat = CreateOrLoadMaterial("ShieldMat", new Color(0.15f, 0.55f, 1.0f));
        Material enemyMat  = CreateOrLoadMaterial("EnemyMat",  new Color(0.95f, 0.20f, 0.25f));

        // 1. Môi trường (Sàn & 4 Tường)
        GameObject envRoot = new GameObject("RollABall_Environment");

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.SetParent(envRoot.transform);
        ground.transform.position = new Vector3(0f, -0.5f, 0f);
        ground.transform.localScale = new Vector3(22f, 1f, 22f);
        ground.GetComponent<Renderer>().sharedMaterial = groundMat;

        CreateWall("Wall_North", new Vector3(0f, 0.75f, 10.75f),  new Vector3(22f, 1.5f, 0.5f), wallMat, envRoot.transform);
        CreateWall("Wall_South", new Vector3(0f, 0.75f, -10.75f), new Vector3(22f, 1.5f, 0.5f), wallMat, envRoot.transform);
        CreateWall("Wall_East",  new Vector3(10.75f, 0.75f, 0f),  new Vector3(0.5f, 1.5f, 22f), wallMat, envRoot.transform);
        CreateWall("Wall_West",  new Vector3(-10.75f, 0.75f, 0f), new Vector3(0.5f, 1.5f, 22f), wallMat, envRoot.transform);

        // 2. Nhân vật chính (Player)
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 0.5f, 0f);
        player.GetComponent<Renderer>().sharedMaterial = playerMat;

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 1.0f;
#if UNITY_6000_0_OR_NEWER
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
#else
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
#endif
        PlayerController playerCtrl = player.AddComponent<PlayerController>();
        playerCtrl.moveSpeed = 12f;
        playerCtrl.spawnPoint = new Vector3(0f, 0.5f, 0f);

        // 3. 10 Khối Tinh thể Năng lượng + 2 Khiên Bảo Vệ
        GameObject pickupsRoot = new GameObject("Pickups_Container");
        int totalCrystals = 10;
        float radius = 7.5f;

        for (int i = 0; i < totalCrystals; i++)
        {
            float angle = i * Mathf.PI * 2f / totalCrystals;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0.6f, Mathf.Sin(angle) * radius);

            GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickup.name = $"Crystal_Pickup_{i + 1}";
            pickup.transform.SetParent(pickupsRoot.transform);
            pickup.transform.position = pos;
            pickup.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            pickup.transform.rotation = Quaternion.Euler(45f, 45f, 45f);
            pickup.GetComponent<Renderer>().sharedMaterial = pickupMat;

            BoxCollider col = pickup.GetComponent<BoxCollider>();
            col.isTrigger = true;

            Rotator rot = pickup.AddComponent<Rotator>();
            rot.itemType = Rotator.ItemType.CrystalPickup;
            rot.rotationSpeed = new Vector3(15f, 30f, 45f);
        }

        Vector3[] shieldPositions = new Vector3[]
        {
            new Vector3(-4.5f, 0.6f, 0f),
            new Vector3(4.5f, 0.6f, 0f)
        };

        for (int i = 0; i < shieldPositions.Length; i++)
        {
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shield.name = $"Shield_PowerUp_{i + 1}";
            shield.transform.SetParent(pickupsRoot.transform);
            shield.transform.position = shieldPositions[i];
            shield.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            shield.GetComponent<Renderer>().sharedMaterial = shieldMat;

            SphereCollider sCol = shield.GetComponent<SphereCollider>();
            sCol.isTrigger = true;

            Rotator sRot = shield.AddComponent<Rotator>();
            sRot.itemType = Rotator.ItemType.ShieldPowerUp;
            sRot.rotationSpeed = new Vector3(0f, 90f, 0f);
        }

        // 4. 4 Chướng ngại vật tuần tra
        GameObject obstaclesRoot = new GameObject("Obstacles_Container");
        CreateObstacle("Obstacle_North", new Vector3(-5f, 0.5f, 4.5f), new Vector3(5f, 0.5f, 4.5f), new Vector3(1.8f, 1f, 0.6f), 3.5f, enemyMat, obstaclesRoot.transform);
        CreateObstacle("Obstacle_South", new Vector3(5f, 0.5f, -4.5f), new Vector3(-5f, 0.5f, -4.5f), new Vector3(1.8f, 1f, 0.6f), 4.5f, enemyMat, obstaclesRoot.transform);
        CreateObstacle("Obstacle_West",  new Vector3(-4.5f, 0.5f, -4f), new Vector3(-4.5f, 0.5f, 4f), new Vector3(0.6f, 1f, 1.8f), 2.8f, enemyMat, obstaclesRoot.transform);
        CreateObstacle("Obstacle_East",  new Vector3(4.5f, 0.5f, 4f),  new Vector3(4.5f, 0.5f, -4f), new Vector3(0.6f, 1f, 1.8f), 5.0f, enemyMat, obstaclesRoot.transform);

        // 5. Camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            mainCam = camObj.AddComponent<Camera>();
        }
        mainCam.transform.position = new Vector3(0f, 10.5f, -10f);
        mainCam.transform.LookAt(player.transform.position);

        CameraController camCtrl = mainCam.GetComponent<CameraController>();
        if (camCtrl == null) camCtrl = mainCam.gameObject.AddComponent<CameraController>();
        camCtrl.target = player.transform;
        camCtrl.offset = new Vector3(0f, 10f, -10f);

        // 6. Giao diện UI (kèm Cần gạt cảm ứng Điện thoại - Virtual Joystick)
        SetupUIAndGameManager(totalCrystals);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Đã dựng xong toàn bộ màn chơi Roll-A-Ball 3D (kèm Cần gạt cảm ứng Điện thoại)! Hãy bấm nút PLAY (▶).");
        EditorUtility.DisplayDialog(
            "Hoàn tất dựng Scene Roll-A-Ball (Hỗ trợ Web Điện thoại)!",
            "Đã tự động tạo xong:\n- Mặt sàn, 4 Tường, Quả cầu Player\n- 10 Tinh thể năng lượng + 2 Khiên bảo vệ\n- 4 Chướng ngại vật tuần tra\n- Cần gạt cảm ứng (Virtual Joystick) góc dưới màn hình cho Web Điện thoại!\n\nBấm nút Play (▶) phía trên để chạy thử ngay!",
            "Bắt đầu ngay");
    }

    private static void CleanupOldObjects()
    {
        string[] namesToRemove = {
            "RollABall_Environment",
            "Player",
            "Pickups_Container",
            "Obstacles_Container",
            "GameManager",
            "GameCanvas",
            "EventSystem"
        };

        foreach (string objName in namesToRemove)
        {
            GameObject existing = GameObject.Find(objName);
            while (existing != null)
            {
                DestroyImmediate(existing);
                existing = GameObject.Find(objName);
            }
        }
    }

    private static Material CreateOrLoadMaterial(string matName, Color color)
    {
        string path = $"Assets/Materials/{matName}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.color = color;
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", color);
        }
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void CreateWall(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent);
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void CreateObstacle(string name, Vector3 ptA, Vector3 ptB, Vector3 scale, float speed, Material mat, Transform parent)
    {
        GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs.name = name;
        obs.transform.SetParent(parent);
        obs.transform.position = ptA;
        obs.transform.localScale = scale;
        obs.GetComponent<Renderer>().sharedMaterial = mat;

        ObstaclePatrol patrol = obs.AddComponent<ObstaclePatrol>();
        patrol.pointA = ptA;
        patrol.pointB = ptB;
        patrol.speed = speed;
    }

    private static void SetupUIAndGameManager(int totalCrystals)
    {
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        GameObject esObj = new GameObject("EventSystem");
        esObj.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        esObj.AddComponent<InputSystemUIInputModule>();
#else
        esObj.AddComponent<StandaloneInputModule>();
#endif

        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        Text scoreTxt  = CreateUIText("ScoreText",  canvasObj.transform, defaultFont, "Điểm: 0 (0/10)", 24, TextAnchor.UpperLeft, new Vector2(20, -20), new Vector2(450, 40));
        Text livesTxt  = CreateUIText("LivesText",  canvasObj.transform, defaultFont, "Mạng: 3",        24, TextAnchor.UpperLeft, new Vector2(20, -60), new Vector2(450, 40));
        Text shieldTxt = CreateUIText("ShieldText", canvasObj.transform, defaultFont, "Khiên bảo vệ: Chưa kích hoạt", 22, TextAnchor.UpperLeft, new Vector2(20, -100), new Vector2(500, 40));

        // Tạo Cần gạt cảm ứng (Virtual Joystick) ở góc dưới bên trái cho Điện thoại
        GameObject joyBgObj = new GameObject("Mobile_VirtualJoystick");
        joyBgObj.transform.SetParent(canvasObj.transform, false);
        Image joyBgImg = joyBgObj.AddComponent<Image>();
        joyBgImg.color = new Color(1f, 1f, 1f, 0.22f);
        RectTransform joyBgRect = joyBgObj.GetComponent<RectTransform>();
        joyBgRect.anchorMin = new Vector2(0f, 0f);
        joyBgRect.anchorMax = new Vector2(0f, 0f);
        joyBgRect.pivot = new Vector2(0.5f, 0.5f);
        joyBgRect.anchoredPosition = new Vector2(140f, 140f);
        joyBgRect.sizeDelta = new Vector2(170f, 170f);

        GameObject joyHandleObj = new GameObject("JoystickHandle");
        joyHandleObj.transform.SetParent(joyBgObj.transform, false);
        Image joyHandleImg = joyHandleObj.AddComponent<Image>();
        joyHandleImg.color = new Color(0.0f, 0.85f, 0.75f, 0.85f);
        RectTransform joyHandleRect = joyHandleObj.GetComponent<RectTransform>();
        joyHandleRect.anchorMin = new Vector2(0.5f, 0.5f);
        joyHandleRect.anchorMax = new Vector2(0.5f, 0.5f);
        joyHandleRect.pivot = new Vector2(0.5f, 0.5f);
        joyHandleRect.anchoredPosition = Vector2.zero;
        joyHandleRect.sizeDelta = new Vector2(70f, 70f);

        VirtualJoystick vJoy = joyBgObj.AddComponent<VirtualJoystick>();
        vJoy.backgroundRect = joyBgRect;
        vJoy.handleRect = joyHandleRect;
        vJoy.handleRange = 55f;

        // Tạo Panel Kết quả (Thắng / Thua)
        GameObject panelObj = new GameObject("ResultPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.82f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(520, 280);

        Text resultTitle = CreateUIText("ResultTitleText", panelObj.transform, defaultFont, "CHIẾN THẮNG!", 34, TextAnchor.MiddleCenter, new Vector2(0, 45), new Vector2(480, 120));
        RectTransform titleRect = resultTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 45);

        GameObject btnObj = new GameObject("RestartButton");
        btnObj.transform.SetParent(panelObj.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.15f, 0.65f, 0.95f, 1f);
        Button restartBtn = btnObj.AddComponent<Button>();
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = new Vector2(0, -65);
        btnRect.sizeDelta = new Vector2(200, 52);

        Text btnText = CreateUIText("ButtonLabel", btnObj.transform, defaultFont, "CHƠI LẠI", 22, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(200, 52));
        RectTransform btnTxtRect = btnText.GetComponent<RectTransform>();
        btnTxtRect.anchorMin = Vector2.zero;
        btnTxtRect.anchorMax = Vector2.one;
        btnTxtRect.offsetMin = Vector2.zero;
        btnTxtRect.offsetMax = Vector2.zero;

        panelObj.SetActive(false);

        GameObject gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();
        gm.totalPickups = totalCrystals;
        gm.scoreText = scoreTxt;
        gm.livesText = livesTxt;
        gm.shieldText = shieldTxt;
        gm.resultPanel = panelObj;
        gm.resultTitleText = resultTitle;
        gm.restartButton = restartBtn;
    }

    private static Text CreateUIText(string name, Transform parent, Font font, string content, int fontSize, TextAnchor anchor, Vector2 anchoredPos, Vector2 size)
    {
        GameObject txtObj = new GameObject(name);
        txtObj.transform.SetParent(parent, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.font = font;
        txt.text = content;
        txt.fontSize = fontSize;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = anchor;
        txt.color = Color.white;

        RectTransform rect = txtObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        return txt;
    }
}
#endif
