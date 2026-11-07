using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Cần gạt cảm ứng ảo (Virtual Joystick) hiển thị trên màn hình
/// giúp điều khiển quả bóng dễ dàng trên Web Điện thoại (Mobile WebGL), Android, iOS và Chuột máy tính.
/// </summary>
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public static VirtualJoystick Instance { get; private set; }

    [Header("Tham chiếu Giao diện Joystick")]
    public RectTransform backgroundRect;
    public RectTransform handleRect;

    [Header("Biên độ kéo tối đa (pixel)")]
    public float handleRange = 65f;

    // Giá trị đầu ra từ -1 đến 1 cho trục Ngang (Horizontal) và Dọc (Vertical)
    public Vector2 InputVector { get; private set; } = Vector2.zero;

    void Awake()
    {
        Instance = this;
        if (backgroundRect == null)
            backgroundRect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (backgroundRect == null || handleRect == null) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            backgroundRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            Vector2 radius = backgroundRect.sizeDelta * 0.5f;
            InputVector = new Vector2(
                localPoint.x / radius.x,
                localPoint.y / radius.y
            );

            // Giới hạn độ dài vector trong khoảng [0, 1]
            if (InputVector.magnitude > 1.0f)
            {
                InputVector = InputVector.normalized;
            }

            // Di chuyển núm gạt (handle) theo ngón tay
            handleRect.anchoredPosition = InputVector * handleRange;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Khi thả ngón tay ra, trả núm gạt về tâm
        InputVector = Vector2.zero;
        if (handleRect != null)
        {
            handleRect.anchoredPosition = Vector2.zero;
        }
    }
}

