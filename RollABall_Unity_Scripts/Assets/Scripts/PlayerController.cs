using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Điều khiển nhân vật chính (Quả cầu năng lượng - Player):
/// - Nhận tín hiệu bàn phím (WASD / Phím mũi tên) HOẶC Cần gạt cảm ứng ảo (Virtual Joystick) trên Web Điện thoại.
/// - Xử lý tương tác xuyên qua (OnTriggerEnter) khi thu thập Tinh thể và Khiên bảo vệ.
/// - Xử lý tương tác va chạm vật lý (OnCollisionEnter) khi đụng phải Chướng ngại vật.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Thông số Di chuyển Vật lý")]
    public float moveSpeed = 12f;
    public Vector3 spawnPoint = new Vector3(0f, 0.5f, 0f);

    [Header("Thông số Khiên Bảo Vệ (Power-Up)")]
    public float shieldDuration = 5.0f;
    public bool isShieldActive = false;
    private float shieldTimer = 0f;

    [Header("Màu sắc Trạng thái")]
    public Color normalColor = new Color(0.0f, 0.85f, 0.75f); // Xanh ngọc
    public Color shieldColor = new Color(0.15f, 0.55f, 1.0f); // Xanh dương sáng

    private Rigidbody rb;
    private Renderer playerRenderer;
    private float moveHorizontal;
    private float moveVertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponent<Renderer>();
        spawnPoint = transform.position;

        ApplyPlayerColor(normalColor);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        // 1. Đọc tín hiệu điều khiển từ Bàn phím HOẶC Cần gạt Cảm ứng trên Điện thoại (Virtual Joystick)
        ReadPlayerInput();

        // 2. Xử lý bộ đếm thời gian Khiên bảo vệ (nếu đang bật)
        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateUI(true, Mathf.Max(0f, shieldTimer));
            }

            if (shieldTimer <= 0f)
            {
                DeactivateShield();
            }
        }

        // 3. Kiểm tra nếu quả cầu rơi khỏi mặt sàn xuống vực
        if (transform.position.y < -3.0f)
        {
            RespawnPlayer();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * moveSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        Rotator item = other.GetComponent<Rotator>();
        if (item != null)
        {
            if (item.itemType == Rotator.ItemType.CrystalPickup)
            {
                other.gameObject.SetActive(false);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CollectPickup(10);
                }
            }
            else if (item.itemType == Rotator.ItemType.ShieldPowerUp)
            {
                other.gameObject.SetActive(false);
                ActivateShield();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ObstaclePatrol obstacle = collision.gameObject.GetComponent<ObstaclePatrol>();
        if (obstacle != null)
        {
            if (isShieldActive)
            {
                collision.gameObject.SetActive(false);
            }
            else
            {
                RespawnPlayer();
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoseLife();
                }
            }
        }
    }

    private void ActivateShield()
    {
        isShieldActive = true;
        shieldTimer = shieldDuration;
        ApplyPlayerColor(shieldColor);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI(true, shieldTimer);
        }
    }

    private void DeactivateShield()
    {
        isShieldActive = false;
        shieldTimer = 0f;
        ApplyPlayerColor(normalColor);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI(false, 0f);
        }
    }

    private void RespawnPlayer()
    {
        transform.position = spawnPoint;
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif
        rb.angularVelocity = Vector3.zero;
    }

    private void ApplyPlayerColor(Color color)
    {
        if (playerRenderer != null && playerRenderer.material != null)
        {
            playerRenderer.material.color = color;
            if (playerRenderer.material.HasProperty("_BaseColor"))
            {
                playerRenderer.material.SetColor("_BaseColor", color);
            }
        }
    }

    /// <summary>
    /// Đọc tín hiệu từ Cần gạt cảm ứng (Virtual Joystick trên Web Điện thoại) và Bàn phím máy tính.
    /// </summary>
    private void ReadPlayerInput()
    {
        moveHorizontal = 0f;
        moveVertical = 0f;

        // Ưu tiên 1: Nếu người chơi đang kéo Cần gạt cảm ứng trên màn hình điện thoại
        if (VirtualJoystick.Instance != null && VirtualJoystick.Instance.InputVector.sqrMagnitude > 0.001f)
        {
            moveHorizontal = VirtualJoystick.Instance.InputVector.x;
            moveVertical = VirtualJoystick.Instance.InputVector.y;
            return;
        }

        // Ưu tiên 2: Đọc từ bàn phím máy tính (WASD / Phím mũi tên)
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveHorizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveHorizontal += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveVertical -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveVertical += 1f;
            return;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
#endif
    }
}
