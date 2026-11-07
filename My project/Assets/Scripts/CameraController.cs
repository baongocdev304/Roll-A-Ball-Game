using UnityEngine;

/// <summary>
/// Điều khiển Camera góc nhìn thứ 3 luôn bám theo Quả cầu (Player)
/// với một khoảng cách (offset) cố định mà không bị xoay lộn vòng theo bóng.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Mục tiêu cần bám theo (Quả cầu Player)")]
    public Transform target;

    [Header("Khoảng cách cố định từ Camera tới Player")]
    public Vector3 offset = new Vector3(0f, 10f, -10f);

    void LateUpdate()
    {
        // Sử dụng LateUpdate để cập nhật vị trí Camera sau khi Player đã di chuyển xong trong khung hình
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target.position);
        }
    }
}

