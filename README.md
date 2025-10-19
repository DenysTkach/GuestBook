# GuestBook

A simple ASP.NET MVC web application for Visual Studio 2022 that allows visitors to post guestbook messages and administrators to manage them.

## Features

### Visitor Features
- **View Approved Messages**: Browse all approved guestbook messages on the home page
- **Add New Messages**: Submit new messages with your name and message text
- **Automatic Database Storage**: All messages are automatically saved to a local SQLite database
- **Success Notifications**: Receive confirmation when messages are submitted

### Admin Features
- **View All Messages**: See both approved and pending messages in a comprehensive table
- **Approve Messages**: Approve pending messages with a single click
- **Delete Messages**: Remove any message (approved or pending)
- **Status Badges**: Visual indicators to distinguish between approved and pending messages

## Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Database**: SQLite with Entity Framework Core 9.0
- **UI**: Razor Views with Bootstrap 5
- **Architecture**: MVC (Model-View-Controller)

## Project Structure

```
GuestBook/
├── Controllers/
│   ├── HomeController.cs      # Visitor functionality
│   └── AdminController.cs     # Admin functionality
├── Models/
│   └── GuestBookMessage.cs    # Message entity model
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml       # View approved messages
│   │   └── Create.cshtml      # Add new message
│   ├── Admin/
│   │   └── Index.cshtml       # Manage all messages
│   └── Shared/
│       └── _Layout.cshtml     # Main layout
├── Data/
│   └── ApplicationDbContext.cs # EF Core DbContext
├── Migrations/                 # EF Core migrations
└── Program.cs                  # Application startup
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

## Usage

### For Visitors

1. Navigate to the home page to view approved messages
2. Click "Add New Message" to submit a new message
3. Fill in your name and message, then click "Submit Message"
4. Your message will be saved and awaiting admin approval

### For Administrators

1. Click "Admin" in the navigation menu
2. View all messages with their approval status
3. Click "Approve" to approve pending messages
4. Click "Delete" to remove any message

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

## License

This project is open source and available for educational purposes.
