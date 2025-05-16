# Zakupek API
*This project was created by me when I was **16 years old** as part of the **10xDevs** course, making me the youngest participant.*

A .NET 8 based ASP.NET Core Web API for AI-assisted shopping list generation and manual management. It provides JWT-secured endpoints for user profiles, AI-generated and CRUD operations on shopping lists, integrating with OpenRouter.ai for AI models.

## Tech Stack
- .NET 8 & ASP.NET Core
- Entity Framework Core (Pomelo MySQL)
- FastEndpoints (Security & Swagger)
- FluentValidation
- Scrutor for DI scanning
- ErrorOr for functional error handling
- OpenRouter.ai for AI model integration
- GitHub Actions CI

## Getting Started

### Prerequisites
- .NET 8 SDK: https://dotnet.microsoft.com/download
- MySQL instance
- (Optional) Docker for containerized setup

### Setup

1. Clone the repository  
   ```bash
   git clone https://github.com/ddnowicki/zakupek-api.git
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
- `dotnet test tests/` — run all unit and integration tests
- Launch profile `Development` opens Swagger UI at `/swagger`

## Project Scope (MVP)
- User registration & JWT authentication
- Profile retrieval
- CRUD on shopping lists (create, read, update, delete)
- AI-generated lists via `/api/shoppinglists/generate`
- Pagination, sorting, and basic product search
- Status tracking (“to buy” vs “bought”)

## License
This project is released under the [MIT License](LICENSE).

> **Note:** Replace `<owner>`, connection string, secrets, and badge URLs with real values once repository details are finalized.
