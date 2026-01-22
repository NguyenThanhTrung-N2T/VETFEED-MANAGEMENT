# VETFEED Backend - Docker Setup

Hướng dẫn chạy Backend API bằng Docker, kết nối với SQL Server local trên máy.

## Yêu cầu

- Docker Desktop (hoặc Docker Engine + Docker Compose)
- SQL Server đang chạy trên máy local (port 1433)
- Database `VetFeedManagement` đã được tạo

## Cấu hình

### 1. Tạo file docker-compose.yml

Copy file `docker-compose.example.yml` thành `docker-compose.yml`:

```bash
cd VETFEED.Backend
cp docker-compose.example.yml docker-compose.yml
```

### 2. Chỉnh sửa cấu hình

Mở file `docker-compose.yml` và cập nhật:

**Connection String** - Thay `<YOUR_LOCAL_SQL_PASSWORD>` bằng password SQL Server local của bạn:

```yaml
- ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=VetFeedManagement;User Id=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True;
```

**JWT Key** - Thay `<YOUR_JWT_SECRET_KEY_HERE>` bằng key bí mật (ít nhất 32 ký tự):

```yaml
- Jwt__Key=YourSecretKeyHere123456789012345678
```

## Cách sử dụng

### 1. Đảm bảo SQL Server local đang chạy

Kiểm tra SQL Server trên máy:

- Port: 1433
- User: sa
- Database: VetFeedManagement đã tồn tại

### 2. Khởi động Backend container

Từ thư mục `VETFEED.Backend`:

```bash
docker-compose up -d --build
```

### 3. Kiểm tra trạng thái

```bash
docker-compose ps
```

### 4. Xem logs

```bash
docker logs vetfeed-backend -f
```

### 5. Truy cập ứng dụng

- **Backend API**: http://localhost:5186
- **Swagger UI**: http://localhost:5186/swagger

### 6. Dừng container

```bash
# Dừng container
docker-compose stop

# Dừng và xóa container
docker-compose down
```

## Rebuild sau khi thay đổi code

```bash
docker-compose up -d --build
```

## Troubleshooting

### Backend không connect được database

**Lỗi**: "❌ Không thể kết nối database"

**Giải pháp**:

1. Kiểm tra SQL Server local đang chạy
2. Kiểm tra password trong `docker-compose.yml` đúng
3. Kiểm tra database `VetFeedManagement` đã tồn tại
4. Kiểm tra SQL Server cho phép TCP/IP connections
5. Kiểm tra firewall không block port 1433

### Kiểm tra kết nối từ container

```bash
# Vào trong container
docker exec -it vetfeed-backend bash

# Test connection (nếu có sqlcmd)
# hoặc xem logs
docker logs vetfeed-backend
```

## Lưu ý

- ✅ File `docker-compose.yml` đã được thêm vào `.gitignore`
- ✅ Chỉ push `docker-compose.example.yml` lên GitHub
- ✅ Backend chạy trong Docker, SQL Server chạy local
- ✅ `host.docker.internal` cho phép container kết nối với host machine
- ✅ Frontend chạy riêng bằng `npm run dev` ở thư mục Frontend
