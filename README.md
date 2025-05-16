# Zakupek API
*This project was created by me when I was **16 years old** as part of the **10xDevs** course, making me the youngest participant.*

A .NET 8 based ASP.NET Core Web API for AI-assisted shopping list generation and manual management. It provides JWT-secured endpoints for user profiles, AI-generated and CRUD operations on shopping lists, integrating with OpenRouter.ai for AI models.

## Tech Stack
- .NET 8 & ASP.NET Core
- Entity Framework Core with Pomelo MySQL Provider
- FastEndpoints for API endpoints with validation
- FluentValidation for request validation
- JWT Authentication with Bearer tokens
- ErrorOr for functional error handling
- OpenRouter.ai integration (deepseek/deepseek-prover-v2:free)
- Scrutor for dependency injection scanning
- Entity Framework Core migrations
- GitHub Actions CI pipeline

## Getting Started

### Prerequisites
- .NET 8 SDK: https://dotnet.microsoft.com/download
- MySQL instance
- (Optional) Docker for containerized setup

### Setup

1. Clone the repository  
   ```bash
   git clone https://github.com/<owner>/zakupek-api.git
   ```
2. Configure settings in `appsettings.json` or environment variables:
   ```json
   {
     "JwtSettings": {
       "SecretKey": "<at least 32 chars>",
       "Issuer": "ZakupekApi",
       "Audience": "ZakupekApiClients"
     },
     "OpenRouter": {
       "ApiKey": "<your-api-key>",
       "BaseUrl": "https://openrouter.ai/api/v1/",
       "Model": "anthropic/claude-3-haiku"
     },
     "ConnectionStrings": {
       "Default": "Server=localhost;Database=zakupek;User=root;Password=..."
     }
   }
   ```
3. Apply EF Core migrations  
   ```bash
   dotnet ef database update --project apps/src/ZakupekApi/ZakupekApi.csproj
   ```
4. Run the API  
   ```bash
   cd apps/src/ZakupekApi
   dotnet run
   ```

## Available Commands
- `dotnet run` — launch API (profiles defined in `launchSettings.json`)
- `dotnet ef migrations add <Name>` — create a new migration
- `dotnet ef database update` — apply migrations
- Launch profile `Development` opens Swagger UI at `/swagger`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user with email, password, username, household size, ages and dietary preferences
- `POST /api/auth/login` - Login with email/password to obtain JWT token

### User Profile
- `GET /api/users/profile` - Get current user profile details

### Shopping Lists
- `GET /api/shoppinglists` - List all shopping lists with pagination & sorting
- `GET /api/shoppinglists/{id}` - Get detailed view of a specific shopping list
- `POST /api/shoppinglists` - Create new shopping list manually
- `PUT /api/shoppinglists/{id}` - Update shopping list (title, products, date, store)
- `DELETE /api/shoppinglists/{id}` - Delete shopping list
- `POST /api/shoppinglists/generate` - Generate AI-assisted shopping list

## Data Models

### Main Entities
- **Users** - Account information with household size
- **UserDietaryPreferences** - Dietary preferences linked to users
- **UserAges** - Ages of household members
- **ShoppingLists** - Lists with metadata (title, date, source)
- **Products** - Items within shopping lists with quantities
- **Stores** - Shopping locations that can be linked to lists
- **Statuses** - Status information for lists and products (AI generated, Manual, etc.)

## Project Scope (MVP)
- User registration & JWT authentication
- Profile management with household details
- CRUD operations on shopping lists
- AI-generated lists via OpenRouter.ai integration
- Pagination, sorting, and filtering
- Product status tracking
- Store management for shopping lists

## License
This project is released under the [MIT License](LICENSE).

> **Note:** Replace `<owner>`, connection string, secrets, and badge URLs with real values once repository details are finalized.
