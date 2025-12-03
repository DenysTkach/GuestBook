# GuestBook

A simple ASP.NET MVC web application for Visual Studio 2022 that allows visitors to post guestbook messages and administrators to manage them. Features user authentication with ASP.NET Core Identity and real-time updates via SignalR.

## Features

### Visitor Features
- **View Approved Messages**: Browse all approved guestbook messages on the home page
- **Add New Messages**: Submit new messages with your name, message text, and optional category
- **View Employee Replies**: See official employee responses to messages
- **User Registration**: Create an account to associate messages with your profile
- **Automatic Database Storage**: All messages are automatically saved to a local SQLite database
- **Real-Time Updates**: Pages automatically refresh when database changes occur via SignalR
- **Success Notifications**: Receive confirmation when messages are submitted

### Authentication Features
- **User Registration**: Create a new account with email and password
- **User Login/Logout**: Secure authentication with ASP.NET Core Identity
- **Role-Based Access**: Administrator, Employee, and User roles
- **Remember Me**: Option to stay logged in

### Employee Features
- **Employee Dashboard**: Dedicated panel for employees to manage message replies
- **Reply to Messages**: Respond to approved guest messages with official replies
- **View Reply History**: See who replied and when for each message
- **Reply Validation**: Character limits and validation for quality responses

### Admin Features
- **Secure Admin Panel**: Only accessible to users with Administrator role
- **View All Messages**: See both approved and pending messages in a comprehensive table
- **Approve Messages**: Approve pending messages with a single click
- **Edit Messages**: Modify approved message content
- **Delete Messages**: Remove any message (approved or pending)
- **Status Badges**: Visual indicators to distinguish between approved and pending messages
- **User & Category Info**: View which user posted each message and its category

### Admin User Management
- **User Management Dashboard**: View all registered users
- **Create Users**: Add new users with email, password, and profile details
- **Edit Users**: Update user information and reset passwords
- **Delete Users**: Remove user accounts (with protection against self-deletion)
- **Role Assignment**: Assign or remove Employee role from users
- **View User Roles**: See current roles for each user

## Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Authentication**: ASP.NET Core Identity
- **Real-Time Communication**: SignalR
- **Database**: SQLite with Entity Framework Core 9.0
- **UI**: Razor Views with Bootstrap 5
- **Architecture**: MVC (Model-View-Controller)

## Entity Model

The application uses 4 main entity types with relationships:

1. **ApplicationUser** (extends IdentityUser)
   - FirstName, LastName, CreatedAt
   - Has many Messages
   - Has many Replies (as RepliedByUser)
   - Has one UserProfile

2. **GuestBookMessage**
   - Name, Message, CreatedAt, IsApproved
   - Reply, RepliedAt, RepliedByUserId (employee reply functionality)
   - Belongs to User (optional)
   - Belongs to Category (optional)
   - Belongs to RepliedByUser (optional - employee who replied)

3. **Category**
   - Name, Description, CreatedAt
   - Has many Messages
   - Seeded with: General, Feedback, Questions

4. **UserProfile**
   - Bio, Location, WebsiteUrl
   - Belongs to User

## Project Structure

```
GuestBook/
├── Controllers/
│   ├── HomeController.cs      # Visitor functionality
│   ├── AdminController.cs     # Admin functionality (messages & user management)
│   ├── EmployeeController.cs  # Employee reply functionality
│   └── AccountController.cs   # Authentication (Login/Register/Logout)
├── Models/
│   ├── ApplicationUser.cs     # Extended Identity user
│   ├── GuestBookMessage.cs    # Message entity with reply support
│   ├── Category.cs            # Category entity model
│   ├── UserProfile.cs         # User profile entity
│   └── ViewModels/            # Login/Register/User view models
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml       # View approved messages with replies
│   │   └── Create.cshtml      # Add new message
│   ├── Admin/
│   │   ├── Index.cshtml       # Manage all messages
│   │   ├── Edit.cshtml        # Edit approved messages
│   │   ├── UserManagement.cshtml # User list and management
│   │   ├── CreateUser.cshtml  # Create new user
│   │   └── EditUser.cshtml    # Edit user details
│   ├── Employee/
│   │   ├── Index.cshtml       # Employee dashboard
│   │   └── Reply.cshtml       # Reply to messages
│   ├── Account/
│   │   ├── Login.cshtml       # Login page
│   │   ├── Register.cshtml    # Registration page
│   │   └── AccessDenied.cshtml # Access denied page
│   └── Shared/
│       └── _Layout.cshtml     # Main layout with SignalR integration
├── Hubs/
│   └── DatabaseHub.cs         # SignalR hub for real-time notifications
├── Services/
│   └── DatabaseWatcherService.cs # Background service for database monitoring
├── Data/
│   └── ApplicationDbContext.cs # EF Core DbContext with Identity
├── Migrations/                 # EF Core migrations
└── Program.cs                  # Application startup with Identity & SignalR config
```

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or Visual Studio Code
- Git (for cloning the repository)

### Installation & Running

1. **Clone the repository**
   ```bash
   git clone https://github.com/DenysTkach/GuestBook.git
   cd GuestBook
   ```

2. **Open the solution**
   - Open `GuestBook.sln` in Visual Studio 2022, or
   - Navigate to the `GuestBook` folder in your terminal

3. **Restore dependencies**
   ```bash
   cd GuestBook
   dotnet restore
   ```

4. **Apply database migrations** (if not already applied)
   ```bash
   dotnet ef database update
   ```

5. **Build and run the application**
   
   **Using Visual Studio:**
   - Press `F5` or click the "Run" button
   
   **Using Command Line:**
   ```bash
   dotnet run
   ```

6. **Access the application**
   - Open your browser and navigate to `https://localhost:5022` (or the port shown in the terminal)

## Default Accounts

When the application starts for the first time, it automatically creates the following roles and accounts:

### Roles
- **Administrator** - Full access to all features including user management
- **Employee** - Can view approved messages and reply to them
- **User** - Standard registered user

### Default Admin Account
- **Email**: `admin@guestbook.com`
- **Password**: `Admin123!`

### Default Employee Account
- **Email**: `employee@guestbook.com`
- **Password**: `Work123!`

## Usage

### For Visitors

1. Navigate to the home page to view approved messages and employee replies
2. Click "Add New Message" to submit a new message
3. Fill in your name, message, and optionally select a category
4. Your message will be saved and awaiting admin approval

### For Registered Users

1. Click "Register" to create a new account
2. Or click "Login" if you already have an account
3. Once logged in, your messages will be associated with your account

### For Employees

1. Login with an employee account (or any account with Employee role)
2. Click "Employee" in the navigation menu (visible only to employees)
3. View all approved messages in the employee dashboard
4. Click "Reply" to respond to any guest message
5. Replies are displayed publicly alongside the original message

### For Administrators

1. Login with the admin account (or any account with Administrator role)
2. Click "Admin" in the navigation menu to manage messages
3. View all messages with their approval status, user, and category
4. Click "Approve" to approve pending messages
5. Click "Edit" to modify approved message content
6. Click "Delete" to remove any message
7. Click "User Management" to manage user accounts
   - Create new users
   - Edit user details and reset passwords
   - Assign or remove Employee role
   - Delete user accounts

## Real-Time Updates

The application uses SignalR to provide real-time page updates:

- When the SQLite database file changes, all connected clients are automatically notified
- Pages refresh automatically to show the latest data
- No manual refresh needed when other users or administrators make changes

## Database

The application uses SQLite for data storage. The database file (`guestbook.db`) is automatically created in the project root directory when you first run the application.

**Note**: The database file is excluded from version control via `.gitignore`.

## Entity Framework Core Commands

- **Add a new migration**:
  ```bash
  dotnet ef migrations add MigrationName
  ```

- **Update database**:
  ```bash
  dotnet ef database update
  ```

- **Remove last migration**:
  ```bash
  dotnet ef migrations remove
  ```

## Development

The application follows standard ASP.NET Core MVC conventions:

- **Models**: Define data entities and business logic
- **Views**: Razor templates for UI rendering
- **Controllers**: Handle HTTP requests and coordinate between models and views
- **DbContext**: Manages database operations with Entity Framework Core
- **Identity**: Handles user authentication and authorization
- **SignalR**: Provides real-time communication for database change notifications
- **Hosted Services**: Background services for database file monitoring

## License

This project is open source and available for educational purposes.
