# PersonalHomepage - 个人主页平台

一个功能完整的个人主页平台，支持用户注册、登录、个人信息管理和多种显示风格。用户可以创建和自定义自己的个人主页，展示技能、项目和联系信息。

## 🌟 主要功能

### 用户管理
- **用户注册/登录系统**：基于ASP.NET Core Identity的完整身份验证
- **个人资料管理**：用户可以管理自己的个人信息
- **安全性**：密码加密、会话管理、访问控制

### 个人主页功能
- **个人信息展示**：姓名、职位、个人描述、头像等
- **头像上传系统**：支持图片上传，自动按用户ID存储，支持JPG/PNG/GIF/WebP格式
- **技能展示**：可视化技能列表，支持熟练度百分比显示
- **项目展示**：项目详情、技术栈、演示链接、源码链接
- **联系信息**：邮箱、电话、地址、社交媒体链接
- **响应式设计**：适配桌面、平板、手机等各种设备

### 主题系统
- **5种显示风格**：
  - 🎨 现代风格（Modern）- 渐变色彩，现代化设计
  - 📚 经典风格（Classic）- 传统布局，稳重大方
  - ✨ 简约风格（Minimal）- 极简设计，黑白配色
  - 🎭 创意风格（Creative）- 鲜艳色彩，创意布局
  - 💼 专业风格（Professional）- 商务风格，适合职场

### 动态配置系统
- **可视化编辑**：通过仪表板界面编辑个人信息
- **JSON配置支持**：支持导入/导出JSON配置文件
- **实时预览**：编辑后可立即预览效果
- **条件渲染**：只显示已填写内容的模块，避免空白区域

### 管理功能
- **仪表板**：统一的管理界面
- **数据统计**：页面访问量统计
- **URL管理**：自定义个人主页URL
- **发布控制**：控制主页的公开/私有状态

## 🛠️ 技术栈

### 后端技术
- **框架**：ASP.NET Core 9.0 MVC
- **身份验证**：ASP.NET Core Identity
- **数据库**：Entity Framework Core + SQL Server
- **依赖注入**：内置DI容器
- **日志系统**：ASP.NET Core Logging

### 前端技术
- **UI框架**：Bootstrap 5
- **JavaScript库**：jQuery
- **图标**：Font Awesome
- **响应式设计**：CSS Grid + Flexbox
- **动画效果**：CSS3 Animations

### 开发工具
- **容器化**：Docker + Docker Compose
- **版本控制**：Git
- **IDE支持**：Visual Studio Code配置

### 数据库设计
- **用户表**：ApplicationUser（扩展Identity用户）
- **配置表**：PageConfiguration（页面配置）
- **关系设计**：一对一用户配置关系

## 🚀 快速开始

### 环境要求
- .NET 9.0 SDK
- SQL Server 或 SQL Server LocalDB
- Docker（可选，用于容器化部署）

### 本地开发

1. **克隆项目**
```bash
git clone <repository-url>
cd PersonalHomepage
```

2. **配置数据库**
```bash
# 更新数据库连接字符串（appsettings.json）
# 运行数据库迁移
dotnet ef database update
```

3. **运行项目**
```bash
dotnet run
```

4. **访问应用**
- 应用地址：http://localhost:5241
- 注册新用户或使用现有账户登录

### Docker部署

#### 方式一：使用快速启动脚本（推荐）

**Windows用户：**
```cmd
# 双击运行或在命令行执行
start.bat
```

**Linux/macOS用户：**
```bash
# 给脚本执行权限
chmod +x start.sh
# 运行脚本
./start.sh
```

#### 方式二：手动使用Docker Compose

1. **使用Docker Compose**
```bash
docker-compose up -d
```

2. **访问应用**
- 应用地址：http://localhost:8080
- 数据库：localhost:15433

## 📁 项目结构

```
PersonalHomepage/
├── Controllers/           # MVC控制器
│   ├── AccountController.cs    # 账户管理
│   ├── DashboardController.cs  # 仪表板
│   └── HomeController.cs       # 主页控制器
├── Models/               # 数据模型
│   ├── ApplicationUser.cs      # 用户模型
│   ├── PageConfiguration.cs    # 页面配置模型
│   └── ViewModels.cs          # 视图模型
├── Views/                # Razor视图
│   ├── Account/               # 账户相关视图
│   ├── Dashboard/             # 仪表板视图
│   ├── Home/                  # 主页视图
│   └── Shared/                # 共享视图
├── Services/             # 业务服务
│   ├── ConfigurationService.cs
│   └── UserConfigurationService.cs
├── Data/                 # 数据访问
│   └── ApplicationDbContext.cs
└── wwwroot/              # 静态资源
    ├── css/
    ├── js/
    └── lib/
```

## 🎯 使用指南

### 1. 注册和登录
- 访问首页，点击"注册"创建新账户
- 使用邮箱和密码登录系统

### 2. 配置个人主页
- 登录后自动跳转到仪表板
- 在"个人信息"标签页填写基本信息
- 在"技能"标签页添加技能和熟练度
- 在"项目"标签页添加项目展示
- 在"联系方式"标签页填写联系信息

### 3. 选择主题风格
- 在"主题"标签页选择喜欢的显示风格
- 实时预览不同风格的效果

### 4. 发布主页
- 在"发布"标签页设置自定义URL
- 控制主页的公开/私有状态
- 查看访问统计数据

### 5. 访问个人主页
- 通过 `/profile/{username}` 访问个人主页
- 分享链接给他人查看

## 🔧 配置说明

### 数据库配置
在 `appsettings.json` 中配置数据库连接：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PersonalHomepageDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 邮件配置（可选）
如需启用邮件功能，在 `appsettings.json` 中添加：
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

## 🔒 安全特性

- **密码策略**：要求大小写字母、数字，最少6位
- **账户锁定**：5次失败登录后锁定5分钟
- **会话管理**：30天自动过期，支持滑动过期
- **访问控制**：基于角色的权限管理
- **数据验证**：前后端双重验证

## 🎨 自定义开发

### 添加新主题
1. 在 `DisplayStyle` 枚举中添加新值
2. 在 `PublicProfile.cshtml` 中添加对应的CSS样式
3. 在仪表板中添加主题选项

### 扩展功能模块
1. 在 `PageConfiguration` 模型中添加新属性
2. 更新数据库迁移
3. 在视图中添加编辑界面
4. 在公共页面中显示新内容

## 📝 更新日志

### v1.0.0-beta.1 (当前版本)
- ✅ 添加用户身份验证系统
- ✅ 实现多主题支持（5种风格）
- ✅ 添加头像上传功能，支持文件上传和预览
- ✅ 添加条件渲染功能
- ✅ 完善仪表板管理界面
- ✅ 优化响应式设计
- ✅ 基础个人主页功能
- ✅ JSON配置系统
- ✅ Docker容器化支持

## 🤝 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 📞 支持

如果您在使用过程中遇到问题，请：
1. 查看 [Issues](../../issues) 中是否有类似问题
2. 创建新的 Issue 描述问题
3. 提供详细的错误信息和复现步骤

---

**PersonalHomepage** - 让每个人都能拥有专业的个人主页 🚀