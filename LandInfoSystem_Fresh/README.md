# Land Information Analysis System

A complete ASP.NET Core web application for buying and selling land properties without broker interference.

## 🏗️ Project Structure

```
LandInfoSystem/
├── Controllers/              # API Endpoints
│   ├── AuthController.cs
│   ├── LandPropertiesController.cs
│   └── UsersController.cs
├── Models/                   # Database Models
│   ├── User.cs
│   ├── LandProperty.cs
│   └── Request.cs
├── Services/                 # Business Logic
│   ├── AuthService.cs
│   ├── PropertyService.cs
│   └── UserService.cs
├── Data/                     # Database Context
│   └── ApplicationDbContext.cs
├── DTOs/                     # Data Transfer Objects
│   └── DTOs.cs
├── wwwroot/                  # Frontend Files
│   ├── index.html
│   ├── css/
│   │   └── style.css
│   └── js/
│       ├── main.js
│       ├── auth.js
│       └── properties.js
├── Program.cs                # Startup Configuration
├── appsettings.json          # Configuration
└── LandInfoSystem.csproj     # Project File
```

## 📋 Prerequisites

- **.NET 8.0 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **SQL Server 2022** - Developer Edition (Free) from [microsoft.com/sql-server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **SQL Server Management Studio (SSMS)** - Download from [microsoft.com/ssms](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- **Visual Studio Code** or **Visual Studio** (Optional)

## 🚀 Quick Start

### Step 1: Create Database

Open **SQL Server Management Studio** and run this script:

```sql
-- Create Database
CREATE DATABASE LandInfoDB;
GO

USE LandInfoDB;
GO

-- Users Table
CREATE TABLE [Users] (
    [UserId] INT PRIMARY KEY IDENTITY(1,1),
    [Username] NVARCHAR(100) NOT NULL UNIQUE,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(100),
    [LastName] NVARCHAR(100),
    [Address] NVARCHAR(255),
    [Phone] NVARCHAR(15),
    [UserType] NVARCHAR(50),
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [IsActive] BIT DEFAULT 1
);

-- LandProperties Table
CREATE TABLE [LandProperties] (
    [PropertyId] INT PRIMARY KEY IDENTITY(1,1),
    [SellerId] INT NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Area] DECIMAL(10,2),
    [Price] DECIMAL(15,2),
    [Location] NVARCHAR(255),
    [City] NVARCHAR(100),
    [State] NVARCHAR(100),
    [PinCode] NVARCHAR(10),
    [ImageUrl] NVARCHAR(500),
    [IsAvailable] BIT DEFAULT 1,
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [UpdatedDate] DATETIME,
    FOREIGN KEY ([SellerId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
);

-- Requests Table
CREATE TABLE [Requests] (
    [RequestId] INT PRIMARY KEY IDENTITY(1,1),
    [BuyerId] INT NOT NULL,
    [PropertyId] INT NOT NULL,
    [Message] NVARCHAR(MAX),
    [Status] NVARCHAR(50),
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [UpdatedDate] DATETIME,
    FOREIGN KEY ([BuyerId]) REFERENCES [Users]([UserId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [LandProperties]([PropertyId]) ON DELETE CASCADE
);

-- Create Indexes
CREATE INDEX IX_LandProperties_SellerId ON LandProperties(SellerId);
CREATE INDEX IX_Requests_BuyerId ON Requests(BuyerId);
CREATE INDEX IX_Requests_PropertyId ON Requests(PropertyId);
```

### Step 2: Install Dependencies

```bash
# Navigate to project directory
cd LandInfoSystem

# Restore NuGet packages
dotnet restore
```

### Step 3: Update Connection String

Edit `appsettings.json` and verify the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LandInfoDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

If using SQL Server Authentication (SA user):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LandInfoDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

### Step 4: Run the Application

```bash
# Run the application
dotnet run

# Or using watch mode for development
dotnet watch run
```

The application will start on: **https://localhost:7200**

(If port 7200 is already in use, .NET will use a different port - check the console output)

## 📱 Features

### For Buyers
- ✅ Browse all available land properties
- ✅ Search properties by city, price range
- ✅ View detailed property information
- ✅ Contact sellers directly
- ✅ Register and manage account

### For Sellers
- ✅ Post land properties for sale
- ✅ Upload property details (area, price, location, etc.)
- ✅ Manage posted properties (edit, delete)
- ✅ View buyer inquiries
- ✅ Respond to buyer requests

### For Admin
- ✅ View all users and properties
- ✅ Manage user accounts
- ✅ Monitor property listings
- ✅ Deactivate problematic users

## 🔌 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/logout` - Logout user

### Land Properties
- `GET /api/landproperties` - Get all properties
- `GET /api/landproperties/available` - Get available properties
- `GET /api/landproperties/{id}` - Get property details
- `GET /api/landproperties/seller/{sellerId}` - Get seller's properties
- `POST /api/landproperties` - Create new property
- `PUT /api/landproperties/{id}` - Update property
- `DELETE /api/landproperties/{id}` - Delete property
- `GET /api/landproperties/search?city=...&minPrice=...&maxPrice=...` - Search properties

### Users
- `GET /api/users/{id}` - Get user details
- `GET /api/users` - Get all users (Admin)
- `GET /api/users/sellers/all` - Get all sellers
- `PUT /api/users/{id}` - Update user
- `PUT /api/users/{id}/deactivate` - Deactivate user

## 🔐 Security Features

- **Password Hashing** - Using BCrypt for secure password storage
- **Input Validation** - Both client and server-side validation
- **SQL Injection Prevention** - Using Entity Framework with parameterized queries
- **CORS Configuration** - Controlled cross-origin requests
- **Session Management** - User session tracking

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core 8.0
- **Database:** SQL Server 2022
- **ORM:** Entity Framework Core 8.0
- **Password Hashing:** BCrypt.Net-Next
- **Frontend:** HTML5, CSS3, Vanilla JavaScript
- **API Documentation:** Swagger/OpenAPI

## 📚 Project Files Explanation

### Models
- **User.cs** - User entity (Buyers, Sellers, Admins)
- **LandProperty.cs** - Land property entity
- **Request.cs** - Buyer request entity

### Services
- **AuthService.cs** - Authentication logic (register, login)
- **PropertyService.cs** - Property CRUD operations
- **UserService.cs** - User management

### Controllers
- **AuthController.cs** - Authentication endpoints
- **LandPropertiesController.cs** - Property management endpoints
- **UsersController.cs** - User management endpoints

### Frontend
- **index.html** - Main page with all sections
- **style.css** - Styling and responsive design
- **main.js** - Core JavaScript logic
- **auth.js** - Authentication form handlers
- **properties.js** - Property management functions

## 🧪 Testing the Application

### Register as a Seller
1. Click "Login/Register"
2. Go to "Create Account" tab
3. Fill form:
   - Username: `seller1`
   - Email: `seller@example.com`
   - Password: `password123`
   - User Type: `Seller`
4. Click Register

### Register as a Buyer
1. Repeat above with:
   - Username: `buyer1`
   - User Type: `Buyer`

### Post a Property (as Seller)
1. Login as seller
2. Click "Dashboard"
3. Click "+ Add New Property"
4. Fill details:
   - Title: `Peaceful Land in Coimbatore`
   - Area: `5000`
   - Price: `500000`
   - Location: `Nehru Colony`
   - City: `Coimbatore`
   - State: `Tamil Nadu`
   - Pin Code: `641001`
5. Click "Post Property"

### Browse Properties (as Buyer)
1. Click "Properties"
2. Browse available properties
3. Click "View Details" for more info
4. Click "Contact Seller" to express interest

## ⚙️ Configuration

### appsettings.json Options

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LandInfoDB;Trusted_Connection=true;"
  },
  "AppSettings": {
    "JwtSecret": "your-secret-key",
    "JwtExpireMinutes": 1440
  }
}
```

## 🐛 Troubleshooting

### Cannot Connect to Database
```bash
# Check SQL Server is running
sqlcmd -S localhost -U sa -P YourPassword -Q "SELECT @@version"
```

### Port Already in Use
```bash
# Use different port
dotnet run --urls "https://localhost:7201"
```

### NuGet Package Errors
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

### HTTPS Certificate Issues
```bash
# Trust development certificate
dotnet dev-certs https --trust
```

## 📦 Deployment

### Publishing for Production

```bash
# Create release build
dotnet publish -c Release -o ./publish

# This creates optimized files ready for deployment
```

### Deploy to IIS
1. Install .NET Hosting Bundle
2. Create IIS Application
3. Copy published files
4. Configure connection string for production database
5. Set appropriate permissions

## 🎓 Learning Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [RESTful API Design](https://restfulapi.net/)
- [JavaScript Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API)

## 📝 Future Enhancements

- [ ] JWT Token Authentication
- [ ] Email Notifications
- [ ] Payment Integration
- [ ] Property Image Upload
- [ ] User Ratings & Reviews
- [ ] Advanced Search Filters
- [ ] Chat/Messaging System
- [ ] Admin Dashboard
- [ ] Mobile App (React Native)
- [ ] API Rate Limiting

## 👥 Support

For issues or questions:
1. Check the troubleshooting section
2. Review API documentation in Swagger (https://localhost:7200/swagger)
3. Check browser console for frontend errors
4. Review Visual Studio Output window for backend errors

## 📄 License

This project is created for educational purposes.

## ✅ Checklist for Getting Started

- [ ] Install .NET 8.0 SDK
- [ ] Install SQL Server 2022
- [ ] Install SSMS
- [ ] Create database using provided SQL script
- [ ] Update connection string in appsettings.json
- [ ] Run `dotnet restore`
- [ ] Run `dotnet run`
- [ ] Open https://localhost:7200 in browser
- [ ] Test registration and login
- [ ] Post a property as seller
- [ ] Browse properties as buyer

**Happy Coding! 🚀**
