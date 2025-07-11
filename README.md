# 个人主页项目

这是一个使用ASP.NET Core MVC构建的个人主页网站，用于展示个人技能、项目和提供联系方式。

## 功能特点

- 响应式设计，适配各种设备
- 技能展示区域，使用进度条直观显示技能熟练度
- 项目展示区域，展示个人项目成果
- 联系表单，访客可以发送邮件联系
- 容器化部署支持，使用Docker快速部署

## 技术栈

- ASP.NET Core MVC
- Bootstrap 5
- Docker

## 如何运行

### 本地开发环境

1. 确保已安装 .NET SDK
2. 克隆仓库到本地
3. 在项目根目录运行：

```bash
dotnet run
```

4. 打开浏览器访问 `https://localhost:5001` 或 `http://localhost:5000`

### 使用Docker部署

1. 确保已安装Docker和Docker Compose
2. 在项目根目录运行：

```bash
docker-compose up -d
```

3. 打开浏览器访问 `http://localhost:8080`

## 自定义内容

要更新个人信息和技能，请修改以下文件：

- `Controllers/HomeController.cs` - 更新技能列表和个人信息
- `Views/Home/Index.cshtml` - 修改首页内容和项目展示
- `Views/Home/Contact.cshtml` - 更新联系信息

## 邮件发送配置

要启用邮件发送功能，需要在 `appsettings.json` 中添加SMTP配置：

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@example.com",
    "SmtpPassword": "your-password",
    "SenderEmail": "your-email@example.com",
    "SenderName": "Your Name"
  }
}
```

然后在 `HomeController.cs` 中实现邮件发送逻辑。

## 许可证

MIT