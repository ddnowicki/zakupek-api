# Schemat Bazy Danych - MySQL

## 1. Tabele

### Tabela: Users
- id: INT AUTO_INCREMENT PRIMARY KEY
- email: VARCHAR(255) NOT NULL UNIQUE
- hashedPassword: TEXT NOT NULL
- userName: VARCHAR(100) NOT NULL
- householdSize: INT
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL

### Tabela: UserDietaryPreferences
- id: INT AUTO_INCREMENT PRIMARY KEY
- userId: INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- preference: VARCHAR(100) NOT NULL
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL

### Tabela: UserAges
- id: INT AUTO_INCREMENT PRIMARY KEY
- userId: INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- age: INT NOT NULL
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL

### Tabela: Stores
- id: INT AUTO_INCREMENT PRIMARY KEY
- userId: INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- name: VARCHAR(100) NOT NULL
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
- updatedAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL

### Tabela: Statuses
- id: INT AUTO_INCREMENT PRIMARY KEY
- name: VARCHAR(50) NOT NULL UNIQUE

### Tabela: ShoppingLists
- id: INT AUTO_INCREMENT PRIMARY KEY
- userId: INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- title: VARCHAR(255)
- sourceId: INT NOT NULL REFERENCES Statuses(id) ON DELETE RESTRICT
- storeId: INT REFERENCES Stores(id) ON DELETE SET NULL
- plannedShoppingDate: DATETIME
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
- updatedAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL

### Tabela: Products
- id: INT AUTO_INCREMENT PRIMARY KEY
- shoppingListId: INT NOT NULL REFERENCES ShoppingLists(id) ON DELETE CASCADE
- name: TEXT NOT NULL
- quantity: INT NOT NULL
- statusId: INT NOT NULL REFERENCES Statuses(id) ON DELETE RESTRICT
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
- updatedAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL

## 2. Relacje
- Jeden użytkownik (Users) może mieć wiele list zakupów (ShoppingLists).
- Jeden użytkownik (Users) może mieć wiele sklepów (Stores).
- Jeden sklep (Stores) może być powiązany z wieloma listami zakupów (ShoppingLists).
- Jedna lista zakupów (ShoppingLists) może zawierać wiele produktów (Products).
- Jeden użytkownik (Users) może mieć wiele preferencji dietetycznych (UserDietaryPreferences).
- Jeden użytkownik (Users) może mieć wiele grup wiekowych (UserAges) dla domowników.
- Status (Statuses) może być przypisany do wielu list zakupów (jako źródło) i produktów.

## 3. Indeksy
- Domyślne indeksy na kolumnach PRIMARY KEY.
- Indeks na ShoppingLists.userId dla przyspieszenia zapytań filtrowania po użytkowniku.
- Indeks na ShoppingLists.storeId dla szybkiego wyszukiwania list dla danego sklepu.
- Indeks na Products.shoppingListId dla szybkiego wyszukiwania produktów na liście.
- Indeks na Products.statusId dla szybkiego filtrowania po statusie produktu.
- Indeks na UserDietaryPreferences.userId dla szybkiego wyszukiwania preferencji użytkownika.
- Indeks na UserAges.userId dla szybkiego wyszukiwania grup wiekowych użytkownika.
- Indeks na Stores.userId dla szybkiego wyszukiwania sklepów użytkownika.

## 4. Zasady MySQL
- Użycie silnika InnoDB dla transakcji i ograniczeń kluczy obcych.
- Wykorzystanie TIMESTAMP z DEFAULT CURRENT_TIMESTAMP oraz ON UPDATE CURRENT_TIMESTAMP dla pól śledzących czas.
- Zastosowanie CASCADE dla usuwania powiązanych rekordów gdy usuwany jest użytkownik lub lista zakupów.
- Zastosowanie RESTRICT dla statusów, aby nie można było usunąć statusu używanego przez produkty.

## 5. Dodatkowe Uwagi
- Schemat został zaktualizowany zgodnie z aktualną strukturą modeli C# w projekcie.
- Dodano tabelę Stores, która pozwala użytkownikom na przechowywanie informacji o sklepach.
- Zrezygnowano z tabeli Sections, którą pierwotnie planowano w MVP.
- Status jest teraz używany zarówno dla list zakupów (jako źródło) jak i dla produktów.
- ShoppingList ma nowe pole plannedShoppingDate, które pozwala na planowanie zakupów.
- Weryfikacja i logika bezpieczeństwa powinna być obsługiwana na poziomie aplikacji.
- Dane użytkownika (preferencje dietetyczne, wiek domowników) są przechowywane w oddzielnych tabelach, aby umożliwić elastyczne zarządzanie wieloma wartościami.
