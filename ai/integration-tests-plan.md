# Plan implementacji testów integracyjnych

Dokument opisuje plan wdrożenia testów integracyjnych z użyciem FastEndpoints oraz specyfikacji z pliku `prd.md`.

## 1. Konfiguracja projektu testowego

1. Utworzenie nowego projektu testowego `ZakupekApi.IntegrationTests` w folderze `apps/tests/`.
2. Dodanie referencji do głównego projektu `ZakupekApi`.
3. Instalacja pakietów NuGet:

   * `FastEndpoints.TestTools`
   * `Shouldly`
   * `Microsoft.EntityFrameworkCore.InMemory`

4. Dodanie folderu `ai` w katalogu rozwiązania i umieszczenie w nim tego planu.

## 2. Konfiguracja `IntegrationApp`

1. Utworzenie klasy `IntegrationApp : AppFixture<Program>`:

   * `[DisableWafCache]` (jeśli okaże się konieczne).
   * `SetupAsync()`:

     * Uruchomienie migracji bazy danych testowej (in-memory).
     * Seed danych referencyjnych (np. katalog sklepów).
     * Utworzenie konta testowego admina i zalogowanie go.
     * Utworzenie klienta z tokenem JWT przy użyciu `CreateClient(c => ...)`.
   * `ConfigureApp(builder)`:

     * Ustawienie środowiska na `IntegrationTests`.
   * `ConfigureServices(services)`:

     * Usunięcie MySql z DI.
     * Podmiana dostawcy DB na in-memory.
   * `TearDownAsync()`:

     * Czyszczenie bazy po testach.
   * Dodanie właściwości `Client` (domyślny `App.Client`) oraz np. `AdminClient`, jeśli wymagany dostęp z tokenem.

## 3. Organizacja testów

1. Wszystkie klasy testowe dziedziczą z `TestBase<IntegrationApp>`.
2. Użycie `App.Client` oraz rozszerzeń takich jak:

   * `POSTAsync<Endpoint, Request, Response>(...)`
   * `GETAsync<Endpoint, Response>()` itd.
3. Ustawienie `EnableAdvancedTesting` w `AssemblyInfo.cs`.
4. Oznaczanie metod testowych za pomocą `[Priority(n)]`.
5. Przypisanie każdej klasy do kolekcji `ZakupekCollection`, np.:

   ```csharp
   [Collection(nameof(ZakupekCollection))]
   public class AuthTests : TestBase<IntegrationApp> { ... }
   ```

   ```csharp
   public class ZakupekCollection : TestCollection<IntegrationApp> { }
   ```

## 4. Scenariusze testowe

Dla każdego z endpointów opisanych w `prd.md`:

1. **US-001 Rejestracja użytkownika** (`POST /api/auth/register`):

   * Poprawne dane → `200` i token JWT.
   * Niepoprawny e-mail, zbyt krótkie hasło → `400`.
   * Rejestracja z istniejącym e-mailem → `409`.

2. **US-002 Logowanie użytkownika** (`POST /api/auth/login`):

   * Poprawne dane → `200` i token JWT.
   * Błędne hasło → `401`.
   * Niezarejestrowany e-mail → `401`.

3. **US-003 Pobranie profilu** (`GET /api/users/profile`):

   * Poprawny token → `200` i dane użytkownika.

4. **US-004 Tworzenie listy zakupów** (`POST /api/shoppinglists`):

   * Poprawne dane → `200`, dane nowej listy.
   * Brak wymaganych pól → `400`.

5. **US-005 Pobranie listy list** (`GET /api/shoppinglists`):

   * Użytkownik zalogowany → `200`, lista, paginacja, sortowanie.

6. **US-006 Pobranie szczegółów listy** (`GET /api/shoppinglists/{id}`):

   * Istniejący identyfikator → `200`.
   * Nieistniejący identyfikator → `404`.

7. **US-007 Edycja listy** (`PUT /api/shoppinglists/{id}`):

   * Poprawna aktualizacja → `200`, `true`.
   * Pominięcie pozycji → `200`, `true`.

8. **US-008 Usuwanie listy** (`DELETE /api/shoppinglists/{id}`):

   * Istniejąca lista → `200`, `true`.
   * Nieistniejąca lista → `404`.

9. **US-009 Generowanie listy przez AI** (`POST /api/shoppinglists/generate`):

   * Poprawne zapytanie → `200`, zawartość wygenerowana.
   * Brak historii zakupów → `200`, fallback.

## 5. Struktura katalogów i plików testów

```
apps/
  tests/
    ZakupekApi.IntegrationTests/
      IntegrationApp.cs
      AuthTests.cs
      UsersTests.cs
      ShoppingListsTests.cs
      AiGenerationTests.cs
      ZakupekCollection.cs
      ai/
        integration-tests-plan.md
```

## 6. Integracja z CI

1. Dodanie kroku do GitHub Actions:

   ```yaml
   - name: Run integration tests
     run: dotnet test apps/tests/ZakupekApi.IntegrationTests
   ```
2. Publikacja wyników (opcjonalnie, jeśli jest skonfigurowana):

   ```yaml
   - name: Publish Test Results
     uses: actions/upload-artifact@v3
     with:
       name: integration-test-results
       path: TestResults/
   ```

## 7. Szczegółowy plan implementacji

### 7.1. Przygotowanie klasy IntegrationApp

- Utworzyć projekt testowy `ZakupekApi.IntegrationTests` w folderze `apps/tests/`.
- Dodać referencję do głównego projektu `ZakupekApi`.
- Zainstalować pakiety NuGet:
  - `FastEndpoints.TestTools`
  - `Shouldly`
  - `Microsoft.EntityFrameworkCore.InMemory`
- Utworzyć klasę `IntegrationApp : AppFixture<Program>`:
  - Zastosować atrybut `[DisableWafCache]` (opcjonalnie).
  - Zaimplementować metody:
    - `SetupAsync()`: 
      - Uruchomić migracje bazy InMemory.
      - Seed danych testowych (np. katalog sklepów).
      - Utworzyć `App.Client` i dodatkowego klienta (np. `AdminClient`) z nagłówkiem Authorization.
    - `ConfigureApp(IWebHostBuilder builder)`: ustawić środowisko na `IntegrationTests`.
    - `ConfigureServices(IServiceCollection services)`:
      - Usunąć rejestrację MySql.
      - Dodać provider InMemory DB.
    - `TearDownAsync()`: oczyścić stan bazy po testach.

### 7.2. Konfiguracja testów i środowiska

- Dodać do `AssemblyInfo.cs`:
  - `[assembly: EnableAdvancedTesting]`.
- Klasy testowe dziedziczą z `TestBase<IntegrationApp>` (lub `TestBase<IntegrationApp, MyState>` jeśli stosowany `StateFixture`).
- Używać prekonfigurowanego `App.Client` i opcjonalnie stanu z `StateFixture`.

### 7.3. Grupowanie testów i kolejność

- Utworzyć test collection:
  - `public class IntegrationAppForCollection : AppFixture<Program> { }`
  - `public class ZakupekCollection : TestCollection<IntegrationAppForCollection> { }`
- Oznaczyć klasy testowe `[Collection(nameof(ZakupekCollection))]`.
- Stosować `[Priority(n)]` na klasach i metodach dla określenia kolejności.

### 7.4. Pisanie testów integracyjnych

- Wykorzystywać rozszerzenia FastEndpoints:
  - `await App.Client.POSTAsync<Endpoint, Request, Response>(...);`
  - `await App.Client.GETAsync<Endpoint, Response>();`
- Weryfikować odpowiedzi za pomocą Shouldly:
  - `rsp.StatusCode.ShouldBe(HttpStatusCode.OK);`

### 7.5. Integracja z CI

- Dodać krok w GitHub Actions:
  - `- name: Run integration tests`
  - `  run: dotnet test apps/tests/ZakupekApi.IntegrationTests`
- Publikować wyniki przy użyciu `actions/upload-artifact@v3`, ścieżka `TestResults/`.
