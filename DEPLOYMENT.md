# PersonalHomepage 部署指南

本文档提供了PersonalHomepage项目的详细部署指南，包括Docker容器化部署和传统部署方式。

## 🐳 Docker 部署（推荐）

### 前置要求
- Docker Engine 20.10+
- Docker Compose 2.0+
- 至少 2GB 可用内存
- 至少 5GB 可用磁盘空间

### 快速部署

1. **克隆项目**
```bash
git clone <repository-url>
cd PersonalHomepage
```

2. **启动服务**
```bash
docker-compose up -d
```

3. **等待服务启动**
```bash
# 查看服务状态
docker-compose ps

# 查看日志
docker-compose logs -f webapp
```

4. **访问应用**
- 应用地址：http://localhost:8080
- 数据库端口：localhost:15433

### 服务架构

```
┌─────────────────┐    ┌─────────────────┐
│   Web App       │    │   SQL Server    │
│   Port: 8080    │◄──►│   Port: 15433   │
│   Container     │    │   Container     │
└─────────────────┘    └─────────────────┘
         │                       │
         └───────────────────────┘
              Docker Network
```

### 容器详情

#### Web应用容器 (webapp)
- **基础镜像**: mcr.microsoft.com/dotnet/aspnet:9.0
- **端口映射**: 8080:8080
- **数据卷**: webapp_uploads (用于存储用户上传的头像)
- **健康检查**: HTTP GET /health
- **重启策略**: unless-stopped

#### 数据库容器 (sqlserver)
- **基础镜像**: mcr.microsoft.com/mssql/server:2022-latest
- **端口映射**: 15433:1433
- **数据卷**: sqlserver_data (持久化数据库数据)
- **健康检查**: SQL Server连接测试
- **重启策略**: unless-stopped

### 环境变量配置

#### 数据库配置
```yaml
SA_PASSWORD: YourStrong@Passw0rd123  # SQL Server SA密码
ACCEPT_EULA: Y                       # 接受SQL Server许可协议
MSSQL_PID: Express                   # SQL Server版本
```

#### Web应用配置
```yaml
ASPNETCORE_ENVIRONMENT: Production
ASPNETCORE_URLS: http://+:8080
ConnectionStrings__DefaultConnection: Server=sqlserver,1433;Database=PersonalHomepageDB;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true;MultipleActiveResultSets=true;Encrypt=false
```

### 数据持久化

项目使用Docker卷来持久化重要数据：

- **sqlserver_data**: 存储SQL Server数据库文件
- **webapp_uploads**: 存储用户上传的头像文件

### 常用命令

```bash
# 启动所有服务
docker-compose up -d

# 停止所有服务
docker-compose down

# 重启特定服务
docker-compose restart webapp

# 查看日志
docker-compose logs webapp
docker-compose logs sqlserver

# 进入容器
docker-compose exec webapp bash
docker-compose exec sqlserver bash

# 查看服务状态
docker-compose ps

# 更新镜像
docker-compose pull
docker-compose up -d --build
```

### 故障排除

#### 1. 数据库连接失败
```bash
# 检查SQL Server容器状态
docker-compose logs sqlserver

# 确认数据库健康状态
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd123 -Q "SELECT 1"
```

#### 2. Web应用启动失败
```bash
# 查看应用日志
docker-compose logs webapp

# 检查端口占用
netstat -tulpn | grep 8080
```

#### 3. 文件上传问题
```bash
# 检查上传目录权限
docker-compose exec webapp ls -la /app/wwwroot/uploads/

# 重新创建上传目录
docker-compose exec webapp mkdir -p /app/wwwroot/uploads/profiles
```

## 🖥️ 传统部署

### 前置要求
- .NET 9.0 SDK
- SQL Server 2019+ 或 SQL Server Express
- IIS 10+ (Windows) 或 Nginx/Apache (Linux)

### 部署步骤

1. **准备环境**
```bash
# 安装.NET 9.0 SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version latest
```

2. **配置数据库**
```sql
-- 创建数据库
CREATE DATABASE PersonalHomepageDB;

-- 创建登录用户（可选）
CREATE LOGIN PersonalHomepageUser WITH PASSWORD = 'YourPassword123';
USE PersonalHomepageDB;
CREATE USER PersonalHomepageUser FOR LOGIN PersonalHomepageUser;
ALTER ROLE db_owner ADD MEMBER PersonalHomepageUser;
```

3. **配置应用**
```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=PersonalHomepageDB;User Id=PersonalHomepageUser;Password=YourPassword123;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

4. **发布应用**
```bash
# 发布到生产环境
dotnet publish -c Release -o ./publish

# 运行数据库迁移
dotnet ef database update --project ./publish/PersonalHomepage.dll
```

5. **配置Web服务器**

#### Nginx配置示例
```nginx
server {
    listen 80;
    server_name your-domain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

## 🔧 生产环境优化

### 性能优化

1. **启用响应压缩**
```csharp
// Program.cs
builder.Services.AddResponseCompression();
app.UseResponseCompression();
```

2. **配置缓存**
```csharp
// Program.cs
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

3. **静态文件缓存**
```csharp
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
    }
});
```

### 安全配置

1. **HTTPS重定向**
```csharp
app.UseHttpsRedirection();
app.UseHsts();
```

2. **安全头**
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

### 监控和日志

1. **结构化日志**
```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.File", "Serilog.Sinks.Console"],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

2. **健康检查**
```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

## 🔄 更新和维护

### Docker环境更新

```bash
# 1. 备份数据
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd123 -Q "BACKUP DATABASE PersonalHomepageDB TO DISK = '/var/opt/mssql/backup/PersonalHomepageDB.bak'"

# 2. 停止服务
docker-compose down

# 3. 拉取最新代码
git pull origin main

# 4. 重新构建和启动
docker-compose up -d --build

# 5. 验证服务
docker-compose ps
curl http://localhost:8080/health
```

### 数据库备份

```bash
# 自动备份脚本
#!/bin/bash
BACKUP_DIR="/backup/personalhomepage"
DATE=$(date +%Y%m%d_%H%M%S)

mkdir -p $BACKUP_DIR

docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd123 -Q "BACKUP DATABASE PersonalHomepageDB TO DISK = '/var/opt/mssql/backup/PersonalHomepageDB_$DATE.bak'"

# 复制备份文件到主机
docker cp personalhomepage-sqlserver:/var/opt/mssql/backup/PersonalHomepageDB_$DATE.bak $BACKUP_DIR/

# 清理旧备份（保留30天）
find $BACKUP_DIR -name "*.bak" -mtime +30 -delete
```

## 📞 技术支持

如果在部署过程中遇到问题：

1. 查看项目 [Issues](../../issues)
2. 检查日志文件
3. 确认系统要求
4. 提交详细的错误报告

---

**注意**: 在生产环境中，请务必修改默认密码并配置适当的安全措施。