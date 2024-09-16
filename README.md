# ShoppingStore

ShoppingStore is an e-commerce application designed to manage articles and shopping carts. This project is structured into several layers to ensure separation of concerns and maintainability. It try to employs Object-Oriented Programming (OOP), Test-Driven Development (TDD), Domain-Driven Design (DDD), and follows SOLID principles and Clean Architecture.

## Project Structur

- **ShoppingStore.sln**
- **ShoppingStore.Domain**
  - **Entities**
    - `Article.cs`
    - `CartRequest.cs`
    - `CartResponse.cs`
    - `CartItem.cs`
    - `CartUpdateResponse.cs`
    - `ShoppingCart.cs`
  - **Interfaces**
    - `IArticleRepository.cs`
    - `IArticleService.cs`
    - `IShoppingCartManager.cs`
    - `IShoppingCartRepository.cs`
- **ShoppingStore.Application**
  - **Services**
    - `ArticleService.cs`
    - `ShoppingCartManager.cs`
- **ShoppingStore.Infrastructure**
  - **Data**
    - `ShoppingStoreContextFactory.cs`
    - `DbShoppingCartManager.cs`
    - `ShoppingStoreContext.cs`
  - **Repositories**
    - `ArticleRepository.cs`
    - `ShoppingCartRepository.cs`
  - **Migrations**
    - `20240913194234_InitialCreate.cs`
    - `ShoppingStoreContextModelSnapshot.cs`
- **ShoppingStore.Presentation**
  - **Controllers**
    - `ArticleController.cs`
    - `CartController.cs`
  - `Program.cs`
- **ShoppingStore.Test**
  - `DbShoppingCartTests.cs`
  - `ArticleControllerTests.cs`
  - `ShoppingCartTests.cs`

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or any other database you are using)
- [Visual Studio](https://visualstudio.microsoft.com/)

### Installation

1. **Clone the repository**:  
   git clone https://github.com/numnachapos/ShoppingStore.git  
   cd ShoppingStore

2. **Set up the database**:
    - Update the connection string in `appsettings.json` located in `ShoppingStore.Presentation` project.
    - Apply migrations to create the database schema:
      dotnet ef database update --project ShoppingStore.Infrastructure
        
3. **Build the solution**:  
   dotnet build

### Running Tests

To run the tests, use the following command:  
  dotnet test


## Project Details

### Domain Layer

- **Entities**: Contains the core business entities like `Article`, `ShoppingCart`, etc.
- **Interfaces**: Defines the contracts for repositories and services.

### Application Layer

- **Services**: Implements the business logic and interacts with the domain layer.

### Infrastructure Layer

- **Data**: Contains the database context and migrations.
- **Repositories**: Implements the data access logic.
- **Migrations**: Contains the database migration files.

### Presentation Layer

- **Controllers**: Contains the API controllers to handle HTTP requests.

### Test Layer

- **Unit Tests**: Contains unit tests for various components of the application.

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or suggestions, feel free to open an issue or contact the repository owner.
