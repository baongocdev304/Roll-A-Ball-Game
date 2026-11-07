using UnityEngine;

/// <summary>
/// Điều khiển chướng ngại vật tự động di chuyển tuần tra qua lại giữa 2 điểm A và B.
/// </summary>
public class ObstaclePatrol : MonoBehaviour
{
    [Header("Điểm đầu và điểm cuối của quỹ đạo tuần tra")]
    public Vector3 pointA;
    public Vector3 pointB;

    [Header("Tốc độ di chuyển nhanh - chậm")]
    public float speed = 3.0f;

    private Vector3 targetPoint;

    void Start()
    {
        // Nếu chưa gán sẵn, mặc định tuần tra sang hai bên từ vị trí hiện tại
        if (pointA == Vector3.zero && pointB == Vector3.zero)
        {
            pointA = transform.position + new Vector3(-4f, 0f, 0f);
            pointB = transform.position + new Vector3(4f, 0f, 0f);
        }
        targetPoint = pointB;
    }

    void Update()
    {
        // Di chuyển đối tượng tới điểm đích
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // Khi chạm tới điểm đích thì đảo chiều quay lại điểm kia
        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }
}

