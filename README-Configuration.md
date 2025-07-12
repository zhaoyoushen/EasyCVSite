# 动态配置系统使用指南

## 概述

这个个人主页项目现在支持通过JSON配置文件动态更新页面内容，无需修改代码即可更新个人信息、技能、项目等内容。

## 功能特性

### 1. 动态内容管理
- **个人信息**：姓名、职位、描述、头像、欢迎信息等
- **技能展示**：技能名称和熟练度百分比
- **项目展示**：项目详情、技术栈、链接等
- **联系信息**：邮箱、电话、社交媒体链接
- **主题设置**：颜色、字体等样式配置

### 2. 配置管理界面
- **上传配置**：通过Web界面上传JSON配置文件
- **下载配置**：导出当前配置为JSON文件
- **重置配置**：恢复到默认设置
- **格式指南**：提供完整的JSON格式说明

## 使用方法

### 1. 访问管理界面
访问 `/Home/Admin` 页面进行配置管理。

### 2. 配置文件格式

配置文件应为JSON格式，包含以下主要部分：

```json
{
  "personalInfo": {
    "name": "您的姓名",
    "title": "您的职位",
    "description": "个人描述",
    "profileImageUrl": "头像URL",
    "welcomeMessage": "欢迎信息",
    "highlights": ["亮点1", "亮点2"]
  },
  "skills": [
    {
      "name": "技能名称",
      "proficiency": 90
    }
  ],
  "projects": [
    {
      "id": 1,
      "name": "项目名称",
      "description": "项目描述",
      "imageUrl": "项目图片URL",
      "detailedDescription": "详细描述",
      "technologies": ["技术1", "技术2"],
      "demoUrl": "演示链接",
      "sourceCodeUrl": "源码链接",
      "completedDate": "2024-01-01T00:00:00"
    }
  ],
  "contactInfo": {
    "email": "邮箱地址",
    "phone": "电话号码",
    "location": "所在地",
    "socialLinks": [
      {
        "platform": "平台名称",
        "url": "链接地址",
        "icon": "图标类名"
      }
    ]
  },
  "theme": {
    "primaryColor": "#007bff",
    "secondaryColor": "#6c757d",
    "backgroundColor": "#ffffff",
    "textColor": "#333333",
    "fontFamily": "字体设置"
  }
}
```

### 3. 示例配置文件

项目中包含了一个示例配置文件 `wwwroot/sample-config.json`，您可以下载并参考这个文件来创建自己的配置。

## 技术实现

### 架构组件

1. **模型类**
   - `PageConfiguration`: 主配置模型
   - `PersonalInfo`: 个人信息模型
   - `Project`: 项目模型
   - `ContactInfo`: 联系信息模型
   - `ThemeSettings`: 主题设置模型

2. **服务层**
   - `IConfigurationService`: 配置服务接口
   - `ConfigurationService`: 配置服务实现

3. **控制器**
   - 扩展了 `HomeController` 以支持配置管理
   - 添加了上传、下载、重置等功能

4. **视图**
   - 更新了 `Index.cshtml` 使用动态配置
   - 更新了 `ProjectDetails.cshtml` 显示项目详情
   - 新增了 `Admin.cshtml` 配置管理界面

### 数据存储

配置文件存储在 `Data/page-config.json`，服务会自动创建目录和默认配置文件。

### 安全考虑

- 配置文件上传包含格式验证
- 错误处理和日志记录
- 文件类型限制（仅允许JSON文件）

## 部署说明

1. 确保 `Data` 目录具有写入权限
2. 配置文件会在首次运行时自动创建
3. 可以通过环境变量或配置文件自定义存储路径

## 扩展功能

未来可以考虑添加：
- 配置版本管理
- 多语言支持
- 在线配置编辑器
- 配置模板库
- 用户认证和权限管理

---

通过这个动态配置系统，您的个人主页现在可以轻松地通过上传JSON文件来更新内容，无需重新部署或修改代码！