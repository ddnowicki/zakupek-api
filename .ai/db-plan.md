# Schemat Bazy Danych - MySQL

## 1. Tabele

### Tabela: Users
- id: INT AUTO_INCREMENT PRIMARY KEY
- email: VARCHAR(255) NOT NULL UNIQUE
- hashed_password: TEXT NOT NULL
- created_at: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL

### Tabela: ShoppingLists
- id: INT AUTO_INCREMENT PRIMARY KEY
- user_id: INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- title: TEXT -- opcjonalna nazwa listy
- created_at: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
- updated_at: TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL

### Tabela: ProductStatuses
- id: INT AUTO_INCREMENT PRIMARY KEY
- name: VARCHAR(50) NOT NULL UNIQUE

-- Dodaj przykładowe statusy: 
-- 'AI generated', 'Czesciowo przez ai', 'usuniete', 'manualnie'

### Tabela: Products
- id: INT AUTO_INCREMENT PRIMARY KEY
- shopping_list_id: INT NOT NULL REFERENCES ShoppingLists(id) ON DELETE CASCADE
- name: TEXT NOT NULL
- quantity: INT NOT NULL
- status_id: INT NOT NULL REFERENCES ProductStatuses(id) ON DELETE RESTRICT
- created_at: TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL

## 2. Relacje
- Jeden użytkownik (Users) może mieć wiele list zakupów (ShoppingLists).
- Jedna lista zakupów (ShoppingLists) może zawierać wiele produktów (Products).

## 3. Indeksy
- Domyślne indeksy na kolumnach PRIMARY KEY.
- Dodatkowy indeks na ShoppingLists.user_id dla przyspieszenia zapytań filtrowania po użytkowniku.

## 4. Zasady MySQL
- Użycie silnika InnoDB dla transakcji i ograniczeń kluczy obcych.
- Wykorzystanie TIMESTAMP z DEFAULT CURRENT_TIMESTAMP oraz ON UPDATE CURRENT_TIMESTAMP.

## 5. Dodatkowe Uwagi
- Schemat został zaprojektowany zgodnie z założeniami MVP i oczekiwaniami nadchodzących rozszerzeń w modelach .NET.
- Weryfikacja i logika bezpieczeństwa powinna być obsługiwana na poziomie aplikacji.
