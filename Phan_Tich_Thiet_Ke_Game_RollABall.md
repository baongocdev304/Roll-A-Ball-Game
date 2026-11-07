# BÁO CÁO PHÂN TÍCH VÀ THIẾT KẾ ĐỀ TÀI
**Môn học:** Đồ án tương tác giữa các đối tượng trong môi trường ảo  
**Đề tài:** Lập trình game 3D Lăn bóng thu thập vật phẩm (Roll-a-Ball) sử dụng Unity  
**Công cụ thực hiện:** Unity Engine & Ngôn ngữ lập trình C#

---

## 1. Giới thiệu game

Trong một thế giới hình học không gian ba chiều (3D) đầy màu sắc, nguồn năng lượng duy trì sự cân bằng của vùng đất bị phân tán thành các khối tinh thể năng lượng nằm rải rác khắp các mê cung nổi trên không trung. Những cạm bẫy và chướng ngại vật cơ học liên tục di chuyển để ngăn cản bất kỳ ai muốn thu hồi chúng. 

Người chơi sẽ hóa thân thành **Quả cầu Năng lượng (Energy Ball)** với nhiệm vụ lăn qua các địa hình hiểm trở, khéo léo né tránh chướng ngại vật và thu thập toàn bộ các khối tinh thể để mở cổng dịch chuyển sang vùng đất tiếp theo. *"Sự khéo léo và khả năng làm chủ quán tính sẽ quyết định chiến thắng của bạn!"*

---

## 2. Các quy luật chơi chính

### Di chuyển
* Nhân vật chính (Quả cầu) xuất phát tại vị trí khởi đầu (Spawn Point) ở trung tâm hoặc đầu bản đồ.
* Người chơi sử dụng các phím điều hướng (`W, A, S, D` hoặc 4 phím mũi tên `Lên, Xuống, Trái, Phải`) để tác động lực đẩy vật lý giúp quả cầu lăn theo 4 hướng trên mặt sàn.
* Quả cầu chịu tác động của **vật lý thực tế (quán tính, ma sát và trọng lực)**: khi di chuyển nhanh sẽ cần khoảng trớn để dừng lại hoặc đổi hướng. Người chơi phải tính toán tốc độ nhanh – chậm hợp lý để không bị văng khỏi mép bản đồ hoặc đâm vào chướng ngại vật.

### Thu thập vật phẩm và Nâng cấp / Tương tác
* Trên bản đồ có các **Khối tinh thể (Pickup Items)** tự xoay tròn tại các vị trí cố định hoặc ẩn sau các bức tường.
* Khi quả cầu lăn chạm vào khối tinh thể:
  * Khối tinh thể biến mất và người chơi được cộng **+10 điểm** (hoặc +1 vật phẩm vào bộ đếm).
  * Một số vật phẩm đặc biệt (Power-up) giúp tăng tốc độ di chuyển tạm thời (Speed Boost) hoặc tạo lớp khiên bảo vệ trong 5 giây để vượt qua vùng nguy hiểm.

### Chướng ngại vật và Tấn công / Phòng thủ
* Trong mỗi màn chơi sẽ xuất hiện các **Khối chướng ngại vật (Obstacles / Enemies)** di chuyển tuần tra qua lại giữa hai điểm cố định hoặc xoay tròn với tốc độ nhanh – chậm khác nhau.
* Nếu quả cầu va chạm trực tiếp với chướng ngại vật đỏ (hoặc rơi khỏi mặt sàn xuống vực):
  * Người chơi bị trừ **1 mạng (Life)** và quả cầu được đưa về điểm xuất phát.
  * Nếu đang có vật phẩm Khiên bảo vệ (Shield Power-up), quả cầu có thể húc văng chướng ngại vật mà không bị mất mạng.
* Sau khi kết thúc màn thứ nhất, người chơi bước sang các màn tiếp theo với bản đồ hẹp hơn, số lượng tinh thể cần thu thập nhiều hơn và các chướng ngại vật di chuyển với tốc độ nhanh hơn.

### Điều kiện Thắng – Thua
* **Chiến thắng (Win):** Thu thập đủ 100% số khối tinh thể trên màn chơi trước khi hết thời gian quy định.
* **Thất bại (Game Over):** Rơi khỏi sàn đấu quá 3 lần (hết mạng) hoặc hết thời gian đếm ngược mà chưa thu thập đủ vật phẩm.

---

## 3. Thiết kế các đối tượng và Tương tác trong môi trường ảo (Unity)

| Tên đối tượng | Hình khối 3D (Mesh) | Thành phần vật lý (Components) | Quy luật tương tác trong môi trường ảo |
| :--- | :--- | :--- | :--- |
| **Player (Quả cầu)** | `Sphere` (Hình cầu) | `Rigidbody`, `Sphere Collider`, `PlayerController.cs` | Nhận lực đẩy từ bàn phím (`AddForce`), chịu trọng lực và va chạm vật lý với mặt sàn, tường và vật phẩm. |
| **Ground & Walls (Sàn & Tường)** | `Plane` & `Cube` | `Mesh Collider` / `Box Collider` (Static) | Giữ quả cầu lăn trên mặt phẳng, tạo phản lực bật lại khi quả cầu va vào tường chắn. |
| **Pickup (Tinh thể năng lượng)** | `Cube` (xoay nghiêng 45°) | `Box Collider` (`Is Trigger = true`), `Rotator.cs` | Tự xoay tại chỗ. Khi `Player` đi xuyên qua vùng Trigger (`OnTriggerEnter`), đối tượng tự ẩn đi và tăng biến điểm số. |
| **Obstacle (Chướng ngại vật)** | `Cylinder` / `Cube` | `Box Collider`, `Rigidbody (Kinematic)` | Di chuyển qua lại theo quỹ đạo. Khi xảy ra va chạm (`OnCollisionEnter`) với `Player`, kích hoạt hàm trừ mạng và đưa `Player` về vị trí ban đầu. |
| **Main Camera (Góc nhìn)** | `Camera` | `CameraController.cs` | Luôn di chuyển bám theo vị trí của `Player` với một khoảng cách (offset) cố định từ trên cao nhìn xuống. |

