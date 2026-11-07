# BỘ PROMPT CHI TIẾT ĐỂ TẠO GAME 3D ROLL-A-BALL TRONG UNITY

Bạn có thể copy **Prompt 1 (Tất cả trong một - Tự động dựng cả Scene bằng 1 cú click)** hoặc dùng **Prompt 2 (Hướng dẫn làm từng bước để hiểu rõ khi vấn đáp với giảng viên)**.

---

## Cách 1: Master Prompt "All-in-One" (Khuyên dùng – Có kèm Script tự động tạo toàn bộ màn chơi bằng 1 nút bấm)

Copy toàn bộ đoạn dưới đây gửi cho AI:

```text
Đóng vai một lập trình viên Unity 3D chuyên nghiệp. Tôi đang làm đồ án môn "Tương tác giữa các đối tượng trong môi trường ảo" với đề tài: "Lập trình game 3D Lăn bóng thu thập vật phẩm (Roll-a-Ball) sử dụng Unity".

Hãy viết đầy đủ code C# hoàn chỉnh, không viết tắt, có chú thích tiếng Việt rõ ràng ở từng hàm tương tác vật lý để tôi dễ bảo vệ đồ án.

### 1. YÊU CẦU GAMEPLAY & QUY LUẬT TƯƠNG TÁC
- **Nhân vật chính (Player):**
  + Hình khối: 3D Sphere, có `Rigidbody` và `SphereCollider`.
  + Di chuyển: Dùng phím WASD hoặc 4 phím mũi tên (`Input.GetAxis("Horizontal")` và `Input.GetAxis("Vertical")`), tác động lực vật lý `rb.AddForce()` để bóng lăn có quán tính trên mặt phẳng.
  + Rơi khỏi sàn: Nếu vị trí `transform.position.y < -3f`, trừ 1 mạng và đưa bóng về vị trí xuất phát (Spawn Point), đồng thời reset vận tốc `rb.velocity = Vector3.zero`.
- **Mặt sàn & Tường chắn (Ground & Walls):**
  + Mặt sàn (`Plane` hoặc `Cube` dẹt kích thước 20x1x20) và 4 bức tường bao quanh có `BoxCollider` ngăn bóng lăn ra ngoài.
- **Vật phẩm thu thập (Pickup Items - Tinh thể năng lượng):**
  + Hình khối: `Cube` kích thước (0.6, 0.6, 0.6), xoay nghiêng góc (45, 45, 45).
  + Hành vi: Tự xoay tròn liên tục mỗi khung hình bằng `transform.Rotate()`.
  + Tương tác: Có `BoxCollider` bật `Is Trigger = true`. Khi Player chạm vào (`OnTriggerEnter`), ẩn vật phẩm (`SetActive(false)`), cộng 10 điểm và kiểm tra nếu đã ăn hết toàn bộ vật phẩm thì hiển thị màn hình Chiến thắng (Win).
- **Vật phẩm hỗ trợ (Shield Power-up - Khiên bảo vệ):**
  + Hình khối: `Sphere` nhỏ màu xanh dương có `Is Trigger = true`.
  + Tương tác: Khi Player ăn phải, kích hoạt trạng thái Khiên bảo vệ trong 5 giây (đổi màu Player hoặc hiện hiệu ứng), giúp Player va chạm với Chướng ngại vật mà không bị mất mạng, đồng thời húc văng chướng ngại vật đó.
- **Chướng ngại vật tuần tra (Patrol Obstacles):**
  + Hình khối: `Cube` hoặc `Cylinder` màu đỏ, có `BoxCollider`.
  + Hành vi: Tự động di chuyển qua lại giữa 2 điểm A và B bằng `Mathf.PingPong` hoặc `Vector3.MoveTowards` với tốc độ cấu hình được.
  + Tương tác: Khi va chạm vật lý (`OnCollisionEnter`) với Player:
    * Nếu Player không có Khiên: Trừ 1 mạng (Player bắt đầu với 3 mạng), đưa Player về vị trí xuất phát. Nếu mạng = 0 thì hiện màn hình Thất bại (Game Over).
    * Nếu Player đang có Khiên: Vô hiệu hóa chướng ngại vật đó.
- **Camera (Main Camera):**
  + Đi theo Player từ góc nhìn thứ 3 trên cao nhìn xuống với khoảng cách `offset` cố định trong hàm `LateUpdate()`.
- **Giao diện (UI) & Quản lý Game (GameManager):**
  + Hiển thị Điểm số (`Score`), Số mạng còn lại (`Lives: 3`), Trạng thái Khiên bảo vệ, Thông báo Thắng (`YOU WIN!`) / Thua (`GAME OVER!`) và nút Chơi lại (`Restart` bằng `SceneManager.LoadScene`).

### 2. YÊU CẦU ĐẶC BIỆT (CỰC KỲ QUAN TRỌNG)
Ngoài các script gameplay (`PlayerController.cs`, `Rotator.cs`, `ObstaclePatrol.cs`, `CameraController.cs`, `GameManager.cs`), hãy viết thêm **1 Editor Script tên là `RollABallSceneBuilder.cs`** đặt trong thư mục `Assets/Editor/`.
- Script này tạo ra một nút trên thanh menu của Unity: `Tools -> Build Roll-A-Ball Scene`.
- Khi tôi bấm vào nút này, code sẽ **TỰ ĐỘNG** tạo sẵn toàn bộ mọi thứ trong Scene hiện tại mà tôi không cần kéo thả thủ công:
  1. Tự động tạo Materials với màu sắc đẹp mắt (Sàn màu xám đậm, Tường màu xám sáng, Player màu xanh ngọc, Tinh thể Pickup màu vàng cam, Khiên Power-up màu xanh dương, Chướng ngại vật màu đỏ).
  2. Tự động tạo Ground, 4 bức tường, Player (gắn sẵn Rigidbody và `PlayerController`), 10 khối Pickup xếp thành vòng tròn, 2 viên Khiên bảo vệ, và 3 Chướng ngại vật di chuyển tuần tra.
  3. Tự động gắn `CameraController` vào `Main Camera` và kết nối tham chiếu tới Player.
  4. Tự động tạo `Canvas` UI (sử dụng `UnityEngine.UI.Text` và `Button` chuẩn để không phụ thuộc TextMeshPro) hiển thị Điểm, Mạng, trạng thái Khiên, Panel Thắng/Thua và nút Chơi lại, đồng thời tự động gán toàn bộ biến vào `GameManager`.

Hãy xuất ra đầy đủ từng file code C# và hướng dẫn ngắn gọn 3 bước để tôi copy code vào Unity và bấm chạy.
```

---

## Cách 2: Prompt hỏi chi tiết giải thích nguyên lý (Dùng khi chuẩn bị báo cáo / bảo vệ đồ án)

Nếu giảng viên hỏi bạn cách hoạt động của từng phần trong Unity, hãy copy prompt này:

```text
Hãy giải thích ngắn gọn, dễ hiểu để tôi trả lời vấn đáp giảng viên môn "Tương tác giữa các đối tượng trong môi trường ảo" đối với game Roll-a-Ball trên Unity:
1. Sự khác nhau giữa va chạm vật lý (`OnCollisionEnter` - dùng cho tường và chướng ngại vật) và va chạm xuyên qua (`OnTriggerEnter` - dùng khi ăn vật phẩm tinh thể).
2. Vai trò của thành phần `Rigidbody` và hàm `FixedUpdate()` khi điều khiển quả bóng lăn bằng `AddForce()`.
3. Cách Camera bám theo quả bóng trong `LateUpdate()` mà không bị xoay lộn vòng khi quả bóng lăn.
```

