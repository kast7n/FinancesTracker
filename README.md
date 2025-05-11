# FinancesTracker

FinancesTracker is a modular .NET application designed to help users track and manage their personal finances. The solution is organized into multiple projects following clean architecture principles, making it easy to maintain and extend.

## Features
- Account management (add, edit, delete accounts)
- Transaction tracking (add, edit, delete transactions)
- Filtering and searching for accounts and transactions
- Data persistence using Entity Framework Core with SQLite
- ASP.NET Core Web frontend
- DTOs and Managers for business logic separation
- Repository pattern for data access

## Project Structure
- **FinancesTracker.Domain**: Contains core domain entities and repository interfaces.
- **FinancesTracker.Application**: Business logic, DTOs, and managers.
- **FinancesTracker.Infrastructure**: Data access layer, EF Core DbContext, migrations, and repository implementations.
- **FinancesTracker.Web**: ASP.NET Core web application (frontend and API).

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQLite](https://www.sqlite.org/download.html) (optional, database file is included)

### Build and Run
1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/FinancesTracker.git
   cd FinancesTracker
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Apply database migrations (optional, if you want to reset or update the schema):
   ```sh
   dotnet ef database update --project FinancesTracker.Infrastructure --startup-project FinancesTracker.Web
   ```
4. Run the web application:
   ```sh
   dotnet run --project FinancesTracker.Web
   ```
5. Open your browser and navigate to `http://localhost:5000` (or the port specified in your launch settings).

## Database
- Uses SQLite by default (`finances.db` in the `FinancesTracker.Web` folder).
- Migrations are managed in the `FinancesTracker.Infrastructure/Migrations` folder.

## Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License
This project is licensed under the MIT License.
