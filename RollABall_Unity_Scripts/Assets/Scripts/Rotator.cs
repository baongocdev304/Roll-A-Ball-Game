using UnityEngine;

/// <summary>
/// Điều khiển vật phẩm tự xoay tròn tại chỗ trong môi trường ảo 3D.
/// Dùng cho cả Tinh thể năng lượng (CrystalPickup) và Khiên bảo vệ (ShieldPowerUp).
/// </summary>
public class Rotator : MonoBehaviour
{
    public enum ItemType
    {
        CrystalPickup,
        ShieldPowerUp
    }

    [Header("Loại vật phẩm")]
    public ItemType itemType = ItemType.CrystalPickup;

    [Header("Tốc độ xoay 3 trục (X, Y, Z)")]
    public Vector3 rotationSpeed = new Vector3(15f, 30f, 45f);

    void Update()
    {
        // Xoay khối 3D mượt mà theo thời gian thực giữa các khung hình
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

