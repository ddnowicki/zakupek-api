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

### Tabela: ShoppingLists
- id: INT AUTO_INCREMENT PRIMARY KEY
- userId: INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- title: TEXT -- opcjonalna nazwa listy
- source: ENUM('manual', 'aiGenerated', 'partiallyAIGenerated') NOT NULL DEFAULT 'manual'
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
- updatedAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL

### Tabela: Sections
- id: INT AUTO_INCREMENT PRIMARY KEY
- userId: INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- name: VARCHAR(100) NOT NULL
- order: INT NOT NULL -- pozwala na sortowanie sekcji
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
- updatedAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL

### Tabela: ProductStatuses
- id: INT AUTO_INCREMENT PRIMARY KEY
- name: VARCHAR(50) NOT NULL UNIQUE

-- Dodaj przykładowe statusy: 
-- 'AI generated', 'Manually added', 'Partially AI generated', 'Deleted'

### Tabela: Products
- id: INT AUTO_INCREMENT PRIMARY KEY
- shoppingListId: INT NOT NULL REFERENCES ShoppingLists(id) ON DELETE CASCADE
- name: TEXT NOT NULL
- quantity: INT NOT NULL
- statusId: INT NOT NULL REFERENCES ProductStatuses(id) ON DELETE RESTRICT
- sectionId: INT REFERENCES Sections(id) ON DELETE SET NULL -- powiązanie z sekcją
- orderInSection: INT -- kolejność produktu w sekcji
- createdAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
- updatedAt: TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL

## 2. Relacje
- Jeden użytkownik (Users) może mieć wiele list zakupów (ShoppingLists).
- Jedna lista zakupów (ShoppingLists) może zawierać wiele produktów (Products).
- Jeden użytkownik (Users) może mieć wiele preferencji dietetycznych (UserDietaryPreferences).
- Jeden użytkownik (Users) może mieć wiele grup wiekowych (UserAges) dla domowników.
- Jeden użytkownik (Users) może mieć wiele sekcji (Sections).
- Jedna sekcja (Sections) może zawierać wiele produktów (Products).

## 3. Indeksy
- Domyślne indeksy na kolumnach PRIMARY KEY.
- Indeks na ShoppingLists.userId dla przyspieszenia zapytań filtrowania po użytkowniku.
- Indeks na Products.shoppingListId dla szybkiego wyszukiwania produktów na liście.
- Indeks na Products.sectionId dla szybkiego wyszukiwania produktów w sekcji.
- Złożony indeks na Products(shoppingListId, sectionId, orderInSection) dla optymalnego sortowania produktów.
- Indeks na UserDietaryPreferences.userId dla szybkiego wyszukiwania preferencji użytkownika.
- Indeks na UserAges.userId dla szybkiego wyszukiwania grup wiekowych użytkownika.
- Indeks na Sections.userId dla szybkiego wyszukiwania sekcji użytkownika.

## 4. Zasady MySQL
- Użycie silnika InnoDB dla transakcji i ograniczeń kluczy obcych.
- Wykorzystanie TIMESTAMP z DEFAULT CURRENT_TIMESTAMP oraz ON UPDATE CURRENT_TIMESTAMP.
- Wykorzystanie ENUM dla pól z predefiniowanymi wartościami jak source w ShoppingLists.

## 5. Dodatkowe Uwagi
- Schemat został zaprojektowany zgodnie z założeniami MVP i oczekiwaniami API opisanego w api-plan.md.
- Weryfikacja i logika bezpieczeństwa powinna być obsługiwana na poziomie aplikacji.
- Dane użytkownika (preferencje dietetyczne, wiek domowników) są przechowywane w oddzielnych tabelach, aby umożliwić elastyczne zarządzanie wieloma wartościami.
- Sekcje są teraz oddzielną tabelą powiązaną z użytkownikiem, co pozwala na personalizowane kategoryzowanie produktów według preferencji każdego użytkownika.
- Produkty są powiązane z sekcjami poprzez klucz obcy sectionId, co umożliwia efektywne sortowanie i grupowanie produktów w obrębie list zakupów.
- Pole orderInSection w tabeli Products pozwala na określenie kolejności produktów w ramach danej sekcji.
- Nazwy pól zostały zmienione na styl camelCase dla lepszej zgodności z konwencją nazewnictwa w kodzie .NET.
