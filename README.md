# 🎮 Đồ Án: Lập Trình Game 3D Lăn Bóng Thu Thập Vật Phẩm (Roll-A-Ball) Sử Dụng Unity

**Môn học:** Đồ án tương tác giữa các đối tượng trong môi trường ảo  
**Công cụ lập trình:** Unity 6 (3D Engine), C#, WebGL (Three.js hỗ trợ cảm ứng Mobile)

---

## 🌐 Chơi Thử Trực Tiếp Trên Trình Duyệt (Máy Tính & Điện Thoại)

* 🔗 **Link Web / Điện thoại (GitHub Pages):** [https://baongocdev304.github.io/Roll-A-Ball-Game/](https://baongocdev304.github.io/Roll-A-Ball-Game/)
* 📱 **Điều khiển trên Điện thoại:** Kéo **Cần gạt cảm ứng (Virtual Joystick)** ở phía dưới màn hình.
* 💻 **Điều khiển trên Máy tính:** Sử dụng các phím **`W, A, S, D`** hoặc **4 phím Mũi tên (`▲, ▼, ◀, ▶`)**.

---

## 📖 1. Giới Thiệu Game

Trong một thế giới hình học không gian ba chiều (3D) đầy màu sắc, nguồn năng lượng duy trì sự cân bằng của vùng đất bị phân tán thành các khối tinh thể năng lượng nằm rải rác khắp các mê cung nổi trên không trung. Những cạm bẫy và chướng ngại vật cơ học liên tục di chuyển để ngăn cản bất kỳ ai muốn thu hồi chúng.

Người chơi hóa thân thành **Quả cầu Năng lượng (Energy Ball)** với nhiệm vụ lăn qua các địa hình, khéo léo né tránh chướng ngại vật và thu thập toàn bộ 10 khối tinh thể để giành chiến thắng.

---

## ⚙️ 2. Các Quy Luật Chơi Chính

### 🟢 Di chuyển
* Quả cầu xuất phát từ vị trí trung tâm bản đồ (`Spawn Point`).
* Người chơi sử dụng phím điều hướng (`W, A, S, D` / Phím mũi tên) hoặc **Cần gạt cảm ứng (Virtual Joystick)** trên điện thoại để tác động lực đẩy vật lý (`Rigidbody.AddForce`) giúp quả cầu lăn theo các hướng trên mặt phẳng.
* Quả cầu chịu tác động của quán tính và ma sát thực tế; trong khi đó, các khối chướng ngại vật màu đỏ di chuyển tuần tra liên tục từ điểm đầu đến điểm kết thúc với tốc độ nhanh – chậm khác nhau.

### 🟡 Thu thập vật phẩm & Tấn công / Phòng thủ
* **Tinh thể năng lượng (Màu vàng):** Tự xoay tròn tại chỗ. Khi quả cầu chạm vào sẽ cộng **+10 điểm** và ẩn tinh thể đi.
* **Khiên bảo vệ – Shield Power-up (Màu xanh dương):** Khi thu thập sẽ kích hoạt lớp Khiên bảo vệ trong **5 giây**, giúp quả cầu đổi sang màu xanh sáng và có thể **húc văng / phá hủy các chướng ngại vật màu đỏ** mà không bị mất mạng.

### 🏆 Điều kiện Thắng – Thua
* **Chiến thắng (`CHIẾN THẮNG!`):** Thu thập đủ **10/10** khối tinh thể năng lượng.
* **Thất bại (`THẤT BẠI - GAME OVER`):** Va chạm với chướng ngại vật đỏ (khi không có Khiên) hoặc rơi khỏi mặt sàn quá **3 mạng**.

---

## 🧩 3. Thiết Kế Đối Tượng & Tương Tác Vật Lý Trong Unity

| Đối tượng (Object) | Hình khối 3D (Mesh) | Thành phần Unity (Components) | Quy luật tương tác trong môi trường ảo |
| :--- | :--- | :--- | :--- |
| **Player (Quả cầu)** | `Sphere` | `Rigidbody`, `Sphere Collider`, `PlayerController.cs` | Nhận lực đẩy vật lý `AddForce` trong `FixedUpdate()`, chịu trọng lực và tương tác va chạm với mọi đối tượng. |
| **Ground & Walls (Sàn & Tường)** | `Cube` | `Box Collider` | Nâng đỡ quả cầu và giới hạn vùng di chuyển, tạo phản lực bật lại khi va chạm. |
| **Pickup (Tinh thể & Khiên)** | `Cube` / `Sphere` | `Collider (Is Trigger = true)`, `Rotator.cs` | Xoay tròn 3 trục. Phát hiện tương tác đi xuyên qua bằng `OnTriggerEnter(Collider other)` để cộng điểm hoặc bật Khiên. |
| **Obstacle (Chướng ngại vật)** | `Cube` | `Box Collider`, `ObstaclePatrol.cs` | Tuần tra qua lại giữa 2 điểm `A` và `B` bằng `Vector3.MoveTowards`. Phát hiện va chạm cứng bằng `OnCollisionEnter(Collision)`. |
| **Main Camera** | `Camera` | `CameraController.cs` | Bám theo vị trí của `Player` với khoảng cách `offset` cố định trong `LateUpdate()`. |
| **Mobile Virtual Joystick** | `UI Image` | `VirtualJoystick.cs` (`IDragHandler`) | Nhận thao tác chạm kéo trên màn hình cảm ứng điện thoại và chuyển đổi thành vector di chuyển `[-1, 1]`. |

---

## 🚀 4. Hướng Dẫn Chạy Project Trong Unity (Tự Động Dựng Scene 1-Click)

1. Mở **Unity Hub** -> Chọn **Add** -> Trỏ vào thư mục `My project` trong repository này.
2. Sau khi Unity Editor mở lên, nhìn lên thanh Menu trên cùng và chọn:
   👉 **`Tools` -> `Build Roll-A-Ball Game`**
3. Script [`RollABallSceneBuilder.cs`](My%20project/Assets/Editor/RollABallSceneBuilder.cs) sẽ tự động dựng toàn bộ Mặt sàn, Tường, Quả cầu, 10 Tinh thể, 2 Khiên bảo vệ, 4 Chướng ngại vật, Giao diện UI và Cần gạt cảm ứng Điện thoại.
4. Bấm nút **Play (`▶`)** để chơi ngay!

---

## 📂 5. Cấu Trúc Thư Mục

* `My project/Assets/Scripts/`: Chứa toàn bộ mã nguồn C# điều khiển tương tác (`PlayerController.cs`, `GameManager.cs`, `Rotator.cs`, `ObstaclePatrol.cs`, `CameraController.cs`, `VirtualJoystick.cs`).
* `My project/Assets/Editor/RollABallSceneBuilder.cs`: Công cụ tự động hóa dựng màn chơi 3D trong Unity.
* `index.html` & `web-mobile/index.html`: Phiên bản WebGL 3D chạy trực tiếp trên trình duyệt máy tính và điện thoại.
* `Phan_Tich_Thiet_Ke_Game_RollABall.docx` & `.md`: Báo cáo phân tích và thiết kế đề tài.
