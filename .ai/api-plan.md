# REST API Plan

## 1. Resources
- **Users** – Represents registered users. (Based on the Users table)
- **ShoppingLists** – Represents shopping list entities created by users. (Based on the ShoppingLists table)
- **Products** – Represents individual products attached to shopping lists. (Based on the Products table)
- **ProductStatuses** – Represents status codes for products. (Based on the ProductStatuses table)
- **Authentication** – Endpoints for user login and token management.

## 2. Endpoints

### Users
1. **User Registration**
   - **Method:** POST
   - **URL:** `/api/users/register`
   - **Description:** Registers a new user with email, password, and household profile details.
   - **Request Body Example:**
     ```json
     {
       "email": "user@example.com",
       "password": "SecurePass123!",
       "userName": "JohnDoe",
       "householdSize": 3,
       "ages": [30, 28, 5],
       "dietaryPreferences": ["vegetarian", "gluten-free"]
     }
     ```
   - **Response Example:**
     ```json
     {
       "id": 1,
       "email": "user@example.com",
       "userName": "JohnDoe",
       "createdAt": "2025-04-10T12:00:00Z",
       "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
       "refreshToken": "eyJhbGciOiJIAzI1NiIsInR5cCI6IkpXVCJ9..."
     }
     ```
   - **Success Codes:** 201 Created
   - **Error Codes:** 400 Bad Request, 409 Conflict (Email already exists)

2. **Get User Profile**
   - **Method:** GET
   - **URL:** `/api/users/me`
   - **Description:** Retrieves authenticated user's profile details.
   - **Headers:** `Authorization: Bearer {token}`
   - **Response Example:**
     ```json
     {
       "id": 1,
       "email": "user@example.com",
       "userName": "JohnDoe",
       "householdSize": 3,
       "ages": [30, 28, 5],
       "dietaryPreferences": ["vegetarian", "gluten-free"],
       "createdAt": "2025-04-10T12:00:00Z",
       "listsCount": 5
     }
     ```
   - **Success Codes:** 200 OK
   - **Error Codes:** 401 Unauthorized

### Authentication
1. **User Login**
   - **Method:** POST
   - **URL:** `/api/auth/login`
   - **Description:** Authenticates the user and returns access tokens.
   - **Request Body Example:**
     ```json
     {
       "email": "user@example.com",
       "password": "SecurePass123!"
     }
     ```
   - **Response Example:**
     ```json
     {
       "userId": 1,
       "userName": "JohnDoe",
       "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
       "refreshToken": "eyJhbGciOiJIAzI1NiIsInR5cCI6IkpXVCJ9...",
       "expiresAt": "2025-04-10T13:00:00Z"
     }
     ```
   - **Success Codes:** 200 OK
   - **Error Codes:** 401 Unauthorized, 400 Bad Request

### ShoppingLists
1. **List All User Shopping Lists**
   - **Method:** GET
   - **URL:** `/api/shoppinglists`
   - **Description:** Retrieves all shopping lists for the authenticated user.
   - **Headers:** `Authorization: Bearer {token}`
   - **Query Parameters:** `page` (default=1), `pageSize` (default=10), `sort` (values: "newest", "oldest", "name")
   - **Response Example:**
     ```json
     {
       "data": [
         {
           "id": 1, 
           "title": "Weekly Groceries", 
           "productsCount": 12, 
           "createdAt": "2025-04-10T12:00:00Z",
           "source": "manual"
         },
         {
           "id": 2, 
           "title": "Weekend Party", 
           "productsCount": 5, 
           "createdAt": "2025-04-11T09:30:00Z",
           "source": "ai_generated"
         }
       ],
       "pagination": {
         "page": 1,
         "pageSize": 10,
         "totalItems": 5,
         "totalPages": 1
       }
     }
     ```
   - **Success Codes:** 200 OK
   - **Error Codes:** 401 Unauthorized

2. **Get Single Shopping List**
   - **Method:** GET
   - **URL:** `/api/shoppinglists/{id}`
   - **Description:** Retrieves details of a specific shopping list, including all its products.
   - **Headers:** `Authorization: Bearer {token}`
   - **Response Example:**
     ```json
     {
       "id": 1,
       "title": "Weekly Groceries",
       "createdAt": "2025-04-10T12:00:00Z",
       "updatedAt": "2025-04-10T14:30:00Z",
       "source": "manual",
       "products": [
         {
           "id": 101,
           "name": "Milk",
           "quantity": 2,
           "statusId": 1,
           "status": "AI generated",
           "createdAt": "2025-04-10T12:05:00Z"
         },
         {
           "id": 102,
           "name": "Bread",
           "quantity": 1,
           "statusId": 2,
           "status": "Manually added",
           "createdAt": "2025-04-10T12:10:00Z"
         }
       ]
     }
     ```
   - **Success Codes:** 200 OK
   - **Error Codes:** 401 Unauthorized, 404 Not Found

3. **Create New Shopping List**
   - **Method:** POST
   - **URL:** `/api/shoppinglists`
   - **Description:** Creates a new shopping list with optional title and initial products.
   - **Headers:** `Authorization: Bearer {token}`
   - **Request Body Example:**
     ```json
     {
       "title": "My Custom List",
       "products": [
         { "name": "Apples", "quantity": 6 },
         { "name": "Bananas", "quantity": 3 }
       ]
     }
     ```
   - **Response Example:**
     ```json
     {
       "id": 3,
       "title": "My Custom List",
       "createdAt": "2025-04-12T15:10:00Z",
       "source": "manual",
       "products": [
         {
           "id": 201,
           "name": "Apples",
           "quantity": 6,
           "statusId": 2,
           "status": "Manually added"
         },
         {
           "id": 202,
           "name": "Bananas",
           "quantity": 3,
           "statusId": 2,
           "status": "Manually added"
         }
       ]
     }
     ```
   - **Success Codes:** 201 Created
   - **Error Codes:** 400 Bad Request, 401 Unauthorized

4. **Edit Shopping List**
   - **Method:** PUT
   - **URL:** `/api/shoppinglists/{id}`
   - **Description:** Updates the title of an existing shopping list.
   - **Headers:** `Authorization: Bearer {token}`
   - **Request Body Example:**
     ```json
     {
       "title": "Updated List Title"
     }
     ```
   - **Response Example:**
     ```json
     {
       "id": 1,
       "title": "Updated List Title",
       "updatedAt": "2025-04-12T16:30:00Z"
     }
     ```
   - **Success Codes:** 200 OK
   - **Error Codes:** 400 Bad Request, 401 Unauthorized, 404 Not Found

5. **Delete Shopping List**
   - **Method:** DELETE
   - **URL:** `/api/shoppinglists/{id}`
   - **Description:** Permanently deletes a shopping list and all its associated products.
   - **Headers:** `Authorization: Bearer {token}`
   - **Success Codes:** 204 No Content
   - **Error Codes:** 401 Unauthorized, 404 Not Found

### AI-Driven Shopping List Features
1. **Generate Shopping List using AI**
   - **Method:** POST
   - **URL:** `/api/shoppinglists/generate`
   - **Description:** Creates a new AI-generated shopping list based on user preferences and history.
   - **Headers:** `Authorization: Bearer {token}`
   - **Request Body Example:**
     ```json
     {
       "title": "Weekly Essentials",
       "dietaryPreferences": ["vegetarian"],
       "peopleCount": 3,
       "excludeRecentPurchases": true
     }
     ```
   - **Response Example:**
     ```json
     {
       "id": 4,
       "title": "Weekly Essentials",
       "createdAt": "2025-04-12T17:20:00Z",
       "source": "ai_generated",
       "products": [
         { "id": 301, "name": "Tofu", "quantity": 2, "statusId": 1, "status": "AI generated" },
         { "id": 302, "name": "Fresh Vegetables", "quantity": 1, "statusId": 1, "status": "AI generated" },
         { "id": 303, "name": "Plant-based Milk", "quantity": 2, "statusId": 1, "status": "AI generated" }
       ]
     }
     ```
   - **Success Codes:** 201 Created
   - **Error Codes:** 400 Bad Request, 401 Unauthorized, 500 Internal Server Error

2. **Sort Shopping List using AI**
   - **Method:** POST
   - **URL:** `/api/shoppinglists/{id}/sort`
   - **Description:** Reorders products in a shopping list using AI for optimal shopping experience.
   - **Headers:** `Authorization: Bearer {token}`
   - **Request Body Example:**
     ```json
     {
       "sortStrategy": "store_layout", 
       "supermarketName": "Lidl" 
     }
     ```
   - **Response Example:**
     ```json
     {
       "id": 1,
       "title": "Weekly Groceries",
       "products": [
         { "id": 103, "name": "Bread", "section": "Bakery" },
         { "id": 105, "name": "Cheese", "section": "Dairy" },
         { "id": 101, "name": "Milk", "section": "Dairy" },
         { "id": 102, "name": "Apples", "section": "Produce" }
       ],
       "sortedBy": "store_layout",
       "supermarket": "Lidl"
     }
     ```
   - **Success Codes:** 200 OK
   - **Error Codes:** 400 Bad Request, 401 Unauthorized, 404 Not Found

## 3. Authentication and Authorization
- **Mechanism:** JSON Web Tokens (JWT) with bearer token authentication.
- **Implementation Details:**
  - Upon successful login via `/api/auth/login` or registration, access and refresh tokens are issued.
  - Secure endpoints require the token in the `Authorization` header as `Bearer {token}`.
  - ASP.NET Core JWT middleware is used for authentication and authorization.
  - HTTPS is enforced to secure token transmission.
  - Refresh token rotation is implemented for enhanced security.

## 4. Validation and Business Logic
- **Validation Rules:**
  1. **User Endpoints:** 
     - Email must be unique and in a valid format.
     - Password must meet complexity requirements.
     - Additional profile fields (e.g., household size, dietary preferences) are validated for presence and proper data types.
  2. **ShoppingLists:**
     - Title is optional but, if provided, should be a non-empty string.
     - Every shopping list must be associated with an existing user.
  3. **Products:**
     - Product name must be provided and non-empty.
     - Quantity must be an integer greater than zero.
     - `statusId` must reference an existing status in the ProductStatuses table.
- **Business Logic:**
  - **User Registration and Login:** Handled by dedicated endpoints ensuring that credentials are validated and securely stored.
  - **AI-Driven List Generation:** Invokes an external AI service (via Openrouter.ai) to generate product suggestions based on user profile and history. The logic ensures seasonal and preference-based filtering.
  - **Cascade Deletion:** Deleting a shopping list automatically removes associated products, adhering to the database cascade rules.
  - **Pagination, Filtering, and Sorting:** Built into list endpoints to efficiently manage large data sets.
  - **Data Consistency:** Enforcement of database constraints (e.g., foreign key relationships) drives API validations.

This API plan is designed to be robust, secure, and aligned with the provided database schema, product requirements, and technology stack.
