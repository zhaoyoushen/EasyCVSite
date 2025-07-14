# PersonalHomepage - Personal Homepage Platform

A comprehensive personal homepage platform that supports user registration, login, personal information management, and multiple display themes. Users can create and customize their own personal homepages to showcase skills, projects, and contact information.

## 🌟 Key Features

### User Management
- **User Registration/Login System**: Complete authentication based on ASP.NET Core Identity
- **Profile Management**: Users can manage their personal information
- **Security**: Password encryption, session management, access control

### Personal Homepage Features
- **Personal Information Display**: Name, position, personal description, avatar, etc.
- **Avatar Upload System**: Support image upload, automatically stored by user ID, supports JPG/PNG/GIF/WebP formats
- **Skills Showcase**: Visual skill list with proficiency percentage display
- **Project Showcase**: Project details, tech stack, demo links, source code links
- **Contact Information**: Email, phone, address, social media links
- **Responsive Design**: Adapts to desktop, tablet, mobile, and other devices

### Theme System
- **5 Display Styles**:
  - 🎨 Modern Style - Gradient colors, modern design
  - 📚 Classic Style - Traditional layout, stable and elegant
  - ✨ Minimal Style - Minimalist design, black and white color scheme
  - 🎭 Creative Style - Vibrant colors, creative layout
  - 💼 Professional Style - Business style, suitable for workplace

### Dynamic Configuration System
- **Visual Editing**: Edit personal information through dashboard interface
- **JSON Configuration Support**: Support import/export JSON configuration files
- **Real-time Preview**: Preview effects immediately after editing
- **Conditional Rendering**: Only display modules with filled content, avoiding blank areas

### Management Features
- **Dashboard**: Unified management interface
- **Data Statistics**: Page view statistics
- **URL Management**: Custom personal homepage URL
- **Publish Control**: Control homepage public/private status

## 🛠️ Tech Stack

### Backend Technologies
- **Framework**: ASP.NET Core 9.0 MVC
- **Authentication**: ASP.NET Core Identity
- **Database**: Entity Framework Core + SQL Server
- **Dependency Injection**: Built-in DI container
- **Logging**: ASP.NET Core Logging

### Frontend Technologies
- **UI Framework**: Bootstrap 5
- **JavaScript Library**: jQuery
- **Icons**: Font Awesome
- **Responsive Design**: CSS Grid + Flexbox
- **Animation Effects**: CSS3 Animations

### Development Tools
- **Containerization**: Docker + Docker Compose
- **Version Control**: Git
- **IDE Support**: Visual Studio Code configuration

### Database Design
- **User Table**: ApplicationUser (extends Identity user)
- **Configuration Table**: PageConfiguration (page configuration)
- **Relationship Design**: One-to-one user configuration relationship

## 🚀 Quick Start

### Requirements
- .NET 9.0 SDK
- SQL Server or SQL Server LocalDB
- Docker (optional, for containerized deployment)

### Local Development

1. **Clone Project**
```bash
git clone <repository-url>
cd PersonalHomepage
```

2. **Configure Database**
```bash
# Update database connection string (appsettings.json)
# Run database migration
dotnet ef database update
```

3. **Run Project**
```bash
dotnet run
```

4. **Access Application**
- Application URL: http://localhost:5241
- Register new user or login with existing account

### Docker Deployment

#### Method 1: Using Quick Start Script (Recommended)

**Windows Users:**
```cmd
# Double-click to run or execute in command line
start.bat
```

**Linux/macOS Users:**
```bash
# Give execute permission to script
chmod +x start.sh
# Run script
./start.sh
```

#### Method 2: Manual Docker Compose

1. **Using Docker Compose**
```bash
docker-compose up -d
```

2. **Access Application**
- Application URL: http://localhost:8080
- Database: localhost:15433

## 📁 Project Structure

```
PersonalHomepage/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs    # Account management
│   ├── DashboardController.cs  # Dashboard
│   └── HomeController.cs       # Home controller
├── Models/               # Data Models
│   ├── ApplicationUser.cs      # User model
│   ├── PageConfiguration.cs    # Page configuration model
│   └── ViewModels.cs          # View models
├── Views/                # Razor Views
│   ├── Account/               # Account related views
│   ├── Dashboard/             # Dashboard views
│   ├── Home/                  # Home views
│   └── Shared/                # Shared views
├── Services/             # Business Services
│   ├── ConfigurationService.cs
│   └── UserConfigurationService.cs
├── Data/                 # Data Access
│   └── ApplicationDbContext.cs
└── wwwroot/              # Static Resources
    ├── css/
    ├── js/
    └── lib/
```

## 🎯 User Guide

### 1. Registration and Login
- Visit homepage, click "Register" to create new account
- Login with email and password

### 2. Configure Personal Homepage
- After login, automatically redirect to dashboard
- Fill basic information in "Personal Info" tab
- Add skills and proficiency in "Skills" tab
- Add project showcase in "Projects" tab
- Fill contact information in "Contact" tab

### 3. Choose Theme Style
- Select preferred display style in "Theme" tab
- Real-time preview of different style effects

### 4. Publish Homepage
- Set custom URL in "Publish" tab
- Control homepage public/private status
- View access statistics

### 5. Access Personal Homepage
- Access personal homepage via `/profile/{username}`
- Share link for others to view

## 🔧 Configuration

### Database Configuration
Configure database connection in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PersonalHomepageDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Email Configuration (Optional)
To enable email functionality, add to `appsettings.json`:
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

## 🔒 Security Features

- **Password Policy**: Requires uppercase, lowercase, numbers, minimum 6 characters
- **Account Lockout**: Lock for 5 minutes after 5 failed login attempts
- **Session Management**: 30-day auto expiration, supports sliding expiration
- **Access Control**: Role-based permission management
- **Data Validation**: Frontend and backend dual validation

## 🎨 Custom Development

### Adding New Themes
1. Add new value to `DisplayStyle` enum
2. Add corresponding CSS styles in `PublicProfile.cshtml`
3. Add theme option in dashboard

### Extending Feature Modules
1. Add new properties to `PageConfiguration` model
2. Update database migration
3. Add editing interface in views
4. Display new content in public pages

## 📝 Changelog

### v1.0.0-beta.1 (Current Version)
- ✅ Added user authentication system
- ✅ Implemented multi-theme support (5 styles)
- ✅ Added avatar upload feature with file upload and preview
- ✅ Added conditional rendering functionality
- ✅ Improved dashboard management interface
- ✅ Optimized responsive design
- ✅ Basic personal homepage functionality
- ✅ JSON configuration system
- ✅ Docker containerization support

## 🤝 Contributing

1. Fork the project
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## 📞 Support

If you encounter issues during use, please:
1. Check [Issues](../../issues) for similar problems
2. Create new Issue describing the problem
3. Provide detailed error information and reproduction steps

---

**PersonalHomepage** - Let everyone have a professional personal homepage 🚀