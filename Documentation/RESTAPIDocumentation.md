# API Specification Doc

**Makes Cents**

| Version | Date             | Author      | Description            |
| ------- | ---------------- | ----------- | ---------------------- |
| 1.0     | 8 November 2024  | Gianna Ross | Initial draft          |
| 2.0     | 21 December 2024 | Gianna Ross | Revisions for GET APIs |

---

## Table of Contents

---

## 1. Common Concepts

### Request/Response Format

- All requests and responses use JSON format
- Content-Type header must be set to `application/json`
- Dates are formatted as `YYYY-MM-DD`
- Decimal values use up to 2 decimal places for currency amounts

### Nullable Fields

Fields marked as "nullable" in the documentation are optional and can be omitted or set to `null`.

### ID References

All resources are identified by integer IDs. When referencing related resources, use the appropriate ID field (e.g., `budgetId`, `envelopeId`, etc.).

### Soft Deletes

Transactions use soft deletes - they are marked with a `deleted_at` timestamp rather than being permanently removed.

---

## 2. API Reference

### 1. Create a new user account

**Description:**  
Register a new user in the Makes Cents system.

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| POST   | `/api/user/register` |

#### **Parameters**

| Type | Name           | Data Type | Description                                              |
| ---- | -------------- | --------- | -------------------------------------------------------- |
| Body | `username`     | `string`  | The unique username for the new user (max 30 characters) |
| Body | `email`        | `string`  | The unique email for the new user (max 320 characters)   |
| Body | `passwordHash` | `string`  | The hashed password for the user                         |

#### **Request Example**

```JSON
{
  "username": "My Username",
  "email": "example@gmail.com",
  "passwordHash": "akjfhen48w9ruibfwle"
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "userId": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwiaWF0IjoxNjA5NDU5MjAwfQ.kJhZyW8K3vR9Xg5bN2cQ8mL7pT0wE6fH9dA4sB1vC2u",
  "message": "User registered successfully"
}
```

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Username already exists"
}
```

[↑ Back to top](#api-specification-doc)

---

### 2. Authenticate a User

**Description:**  
Authenticate an existing user and return a JWT token for subsequent requests.

#### **Request**

| Method | URL               |
| ------ | ----------------- |
| POST   | `/api/user/login` |

#### **Parameters**

| Type | Name           | Data Type | Description                                  |
| ---- | -------------- | --------- | -------------------------------------------- |
| Body | `username`     | `string`  | The username or email of the user logging in |
| Body | `passwordHash` | `string`  | The hashed password of the user logging in   |

#### **Request Example**

```JSON
{
  "username": "johndoe",
  "passwordHash": "akjfhen48w9ruibfwle"
}
```

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "userId": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwiaWF0IjoxNjA5NDU5MjAwfQ.kJhZyW8K3vR9Xg5bN2cQ8mL7pT0wE6fH9dA4sB1vC2u",
  "message": "Login successful"
}
```

**Unauthorized (401)**

```json
{
  "status": 401,
  "error": "Invalid username or password"
}
```

[↑ Back to top](#api-specification-doc)

---

### 3. Get User Profile

**Description:**  
Retrieve user profile information.

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| GET    | `/api/user/{userId}` |

#### **Parameters**

| Type   | Name     | Data Type | Description        |
| ------ | -------- | --------- | ------------------ |
| Path   | `userId` | `int`     | The ID of the user |
| Header | `token`  | `string`  | Bearer JWT token   |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "user": {
    "userId": 1,
    "username": "johndoe",
    "email": "john.doe@example.com",
    "isDarkMode": false
  }
}
```

---

### 4. Update a User

**Description:**  
Update user profile information including username, email, or password.

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| PUT    | `/api/user/{userId}` |

#### **Parameters**

| Type   | Name             | Data Type | Description                                                      |
| ------ | ---------------- | --------- | ---------------------------------------------------------------- |
| Path   | `userId`         | `int`     | The ID of the user                                               |
| Header | `token`          | `string ` | JWT authorization token                                          |
| Body   | `username`       | `string`  | The updated username of the user - optional                      |
| Body   | `email`          | `string`  | The updated email of the user - optional                         |
| Body   | `hashedPassword` | `string`  | The updated hashed password of the user - optional               |
| Body   | `isDarkMode`     | `boolean` | The updated value for the users appearance preference - optional |

#### **Request Example**

```JSON
{
  "email": "updatedEmail@gmail.com"
}
```

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "userId": 1,
  "message": "User profile updated successfully"
}
```

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Email already in use"
}
```

[↑ Back to top](#api-specification-doc)

---

### 5. Delete a User

**Description:**  
Permanently delete a user account and all associated data.

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| DELETE | `/api/user/{userId}` |

#### **Parameters**

| Type   | Name     | Data Type | Description             |
| ------ | -------- | --------- | ----------------------- |
| Path   | `userId` | `int`     | The Id of the user      |
| Header | `token`  | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "message": "User account deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 6. Create a Budget

**Description:**  
Create a new blank budget specific month and year.

#### **Request**

| Method | URL            |
| ------ | -------------- |
| POST   | `/api/budgets` |

#### **Parameters**

| Type   | Name         | Data Type | Description                        |
| ------ | ------------ | --------- | ---------------------------------- |
| Header | `token`      | `string ` | JWT authorization token            |
| Body   | `userId`     | `int`     | The Id of the user logged in       |
| Body   | `monthId`    | `int`     | The id of the month for the budget |
| Body   | `year`       | `int`     | The year for the budget            |
| Body   | `budgetName` | `string`  | The name of the budget             |

#### **Request Example**

```JSON
{
  "userId": 1,
  "monthId": 11,
  "year": 2025,
  "budgetName": "My budget"
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "budgetId": 1,
  "message": "Budget created successfully"
}
```

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Budget already exists for this user, month, and year"
}
```

[↑ Back to top](#api-specification-doc)

---

### 7. Get a Budget

**Description:**  
Get the initial data for a budget, including the budget name, envelope categories, and envelopes based on the userId, month, and year.

#### **Request**

| Method | URL                                                      |
| ------ | -------------------------------------------------------- |
| GET    | `/api/budgets/user/{userId}/year/{year}/month/{monthId}` |

#### **Parameters**

| Type   | Name      | Data Type | Description             |
| ------ | --------- | --------- | ----------------------- |
| Path   | `userId`  | `int`     | The ID of the user      |
| Path   | `year`    | `int`     | The year                |
| Path   | `monthId` | `int`     | The month ID (1-12)     |
| Header | `token`   | `string ` | JWT authorization token |

#### **Responses**

```JSON
{
  "status": 200,
  "budget": {
    "budgetId": 1,
    "userId": 11,
    "monthId": 11,
    "year": 2025,
    "budgetName": "My Budget",
    "envelopeCategories": [
      {
        "envelopeCategoryId": 2,
        "budgetId": 1,
        "envelopeCategoryName": "Envelope Category 1",
        "envelopes": [
          {
            "envelopeId": 3,
            "envelopeCategoryId": 2,
            "envelopeName": "Envelope 1",
            "remainingAmount": 54.99
          },
          {
            "envelopeId": 4,
            "envelopeCategoryId": 2,
            "envelopeName": "Envelope 2",
            "remainingAmount": 22.99
          }
        ]
      }
    ]
  }
}
```

[↑ Back to top](#api-specification-doc)

---

### 8. Update a Budget

**Description:**  
Update an existing budget

#### **Request**

| Method | URL                       |
| ------ | ------------------------- |
| PUT    | `/api/budgets/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description                       |
| ------ | ---------- | --------- | --------------------------------- |
| Path   | `budgetId` | `int`     | The ID of the budget              |
| Header | `token`    | `string ` | JWT authorization token           |
| Body   | `name`     | `string`  | The name of the budget - optional |

#### **Request Example**

```JSON
{
  "name": "Updated Budget"
}
```

#### **Responses**

```json
{
  "status": 200,
  "budgetId": 1,
  "message": "Budget updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 9. Delete a Budget

**Description:**  
Permanently delete a budget and all associated data (envelope categories, envelopes, transactions, etc.).

#### **Request**

| Method | URL                       |
| ------ | ------------------------- |
| DELETE | `/api/budgets/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description             |
| ------ | ---------- | --------- | ----------------------- |
| Path   | `budgetId` | `int`     | The Id of the budget    |
| Header | `token`    | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "message": "Budget deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 10. Create an Envelope Category

**Description:**  
Create a new envelope category within a budget.

#### **Request**

| Method | URL                        |
| ------ | -------------------------- |
| POST   | `/api/envelope-categories` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                                 |
| ------ | ---------------------- | --------- | ------------------------------------------- |
| Header | `token`                | `string ` | JWT authorization token                     |
| Body   | `budgetId`             | `int`     | The budget id for the new envelope category |
| Body   | `envelopeCategoryName` | `string`  | The name for the envelope category          |

#### Responses

**Success (201 Created)**

```json
{
  "status": 201,
  "envelopeCategoryId": 1,
  "message": "Envelope category created successfully"
}
```

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Category name already exists in this budget"
}
```

[↑ Back to top](#api-specification-doc)

---

### 11. Get All Categories for a Budget

**Description:**  
Retrieve all envelope categories for a specific budget.

#### **Request**

| Method | URL                                          |
| ------ | -------------------------------------------- |
| GET    | `/api/envelope-categories/budget/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description          |
| ------ | ---------- | --------- | -------------------- |
| Path   | `budgetId` | `int`     | The ID of the budget |
| Header | `token`    | `string`  | Bearer JWT token     |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "envelopeCategories": [
    {
      "envelopeCategoryId": 2,
      "budgetId": 1,
      "envelopeCategoryName": "Envelope Category 1",
      "envelopes": [
        {
          "envelopeId": 3,
          "envelopeCategoryId": 2,
          "envelopeName": "Envelope 1",
          "remainingAmount": 54.99
        },
        {
          "envelopeId": 4,
          "envelopeCategoryId": 2,
          "envelopeName": "Envelope 2",
          "remainingAmount": 22.99
        }
      ]
    }
  ]
}
```

---

### 12. Update an Envelope Category

**Description:**  
Update an existing envelope category

#### **Request**

| Method | URL                                             |
| ------ | ----------------------------------------------- |
| PUT    | `/api/envelope-categories/{envelopeCategoryId}` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                                          |
| ------ | ---------------------- | --------- | ---------------------------------------------------- |
| Path   | `envelopeCategoryId`   | `int`     | The ID of the envelope category                      |
| Header | `token`                | `string ` | JWT authorization token                              |
| Body   | `envelopeCategoryName` | `string`  | The updated name of the envelope category - optional |

#### **Request Example**

```JSON
{
  "envelopeCategoryName": "New Category Name"
}
```

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "envelopeCategoryId": 1,
  "message": "Envelope category updated successfully"
}
```

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Category name already exists in this budget"
}
```

[↑ Back to top](#api-specification-doc)

---

### 13. Delete an Envelope Category

**Description:**  
Permanently delete an envelope category and all its envelopes.

#### **Request**

| Method | URL                                             |
| ------ | ----------------------------------------------- |
| DELETE | `/api/envelope-categories/{envelopeCategoryId}` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                     |
| ------ | -------------------- | --------- | ------------------------------- |
| Path   | `envelopeCategoryId` | `int`     | The Id of the envelope category |
| Header | `token`              | `string ` | JWT authorization token         |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "message": "Envelope category deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 14. Create an Envelope

**Description:**  
Create a new envelope within an envelope category.

#### **Request**

| Method | URL              |
| ------ | ---------------- |
| POST   | `/api/envelopes` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                                                                            |
| ------ | -------------------- | --------- | ------------------------------------------------------------------------------------------------------ |
| Header | `token`              | `string ` | JWT authorization token                                                                                |
| Body   | `envelopeCategoryId` | `int`     | The envelope category id for the new envelope                                                          |
| Body   | `envelopeName`       | `string`  | The name for the envelope                                                                              |
| Body   | `plannedAmount`      | `decimal` | The planned amount for the envelope                                                                    |
| Body   | `remainingAmount`    | `decimal` | The remaining amount for the envelope                                                                  |
| Body   | `isSinkingFund`      | `boolean` | If the envelope is a sinking fun                                                                       |
| Body   | `goalAmount`         | `decimal` | The goal amount for the envelope if it is a sinking fund - nullable                                    |
| Body   | `goalEndDate`        | `Date`    | The end date for the goal if the envelope is a sinking fund - nullable                                 |
| Body   | `transferEnvelopeId` | `int`     | The id of the envelope to transfer remaining funds to if the envelope is not a sinking fund - nullable |

#### **Request Example**

```JSON
{
  "envelopeCategoryId": 1,
  "envelopeName": "Envelope 1",
  "plannedAmount": 234.56,
  "remainingAmount": 123.34,
  "isSinkingFund": true,
  "goalAmount": 1234.56,
  "goalEndDate": "YYYY-MM-DD",
  "transferEnvelopeId": null
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "envelopeId": 1,
  "message": "Envelope created successfully"
}
```

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Envelope name already exists in this category"
}
```

[↑ Back to top](#api-specification-doc)

---

### 15. Get a Specific Envelope

**Description:**  
Retrieve detailed information for a specific envelope.

#### **Request**

| Method | URL                           |
| ------ | ----------------------------- |
| GET    | `/api/envelopes/{envelopeId}` |

#### **Parameters**

| Type   | Name         | Data Type | Description            |
| ------ | ------------ | --------- | ---------------------- |
| Path   | `envelopeId` | `int`     | The ID of the envelope |
| Header | `token`      | `string`  | Bearer JWT token       |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "envelope": {
    "envelopeId": 1,
    "envelopeCategoryId": 1,
    "envelopeName": "Rent",
    "plannedAmount": 1200.0,
    "remainingAmount": 1200.0,
    "isSinkingFund": false,
    "goalAmount": null,
    "goalEndDate": null,
    "transferEnvelopeId": 3,
    "transactions": [
      {
        "transactionId": 5,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 10.99,
        "transactionType": "Payment",
        "paymentTypeId": 3,
        "merchantSourceName": "Walmart"
      },
      {
        "transactionId": 15,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 75.0,
        "transactionType": "Transfer",
        "fromId": 4,
        "toId": 3,
        "transferTypeId": 1
      }
    ]
  }
}
```

---

### 16. Update an Envelope

**Description:**
Update the details of an existing envelope including amounts, name, and sinking fund configuration.

#### **Request**

| Method | URL                           |
| ------ | ----------------------------- |
| PUT    | `/api/envelopes/{envelopeId}` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                                                                                       |
| ------ | -------------------- | --------- | ----------------------------------------------------------------------------------------------------------------- |
| Path   | `envelopeId`         | `int`     | The Id of the envelope                                                                                            |
| Header | `token`              | `string ` | JWT authorization token                                                                                           |
| Body   | `envelopeCategoryId` | `int`     | The envelope category id for the envelope - optional                                                              |
| Body   | `envelopeName`       | `string`  | The name for the envelope - optional                                                                              |
| Body   | `plannedAmount`      | `decimal` | The planned amount for the envelope - optional                                                                    |
| Body   | `remainingAmount`    | `decimal` | The remaining amount for the envelope - optional                                                                  |
| Body   | `isSinkingFund`      | `boolean` | If the envelope is a sinking fun - nullable + optional                                                            |
| Body   | `goalAmount`         | `decimal` | The goal amount for the envelope if it is a sinking fund - nullable + optional                                    |
| Body   | `goalEndDate`        | `Date`    | The end date for the goal if the envelope is a sinking fund - nullable + optional                                 |
| Body   | `transferEnvelopeId` | `int`     | The id of the envelope to transfer remaining funds to if the envelope is not a sinking fund - nullable + optional |

#### **Request Example**

```JSON
{
  "transferEnvelopeId": 4
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "envelopeId": 1,
  "message": "Envelope updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 17. Delete an Envelope

**Description:**
Delete an existing envelope and its data

#### **Request**

| Method | URL                           |
| ------ | ----------------------------- |
| DELETE | `/api/envelopes/{envelopeId}` |

#### **Parameters**

| Type   | Name         | Data Type | Description             |
| ------ | ------------ | --------- | ----------------------- |
| Path   | `envelopeId` | `int`     | The Id of the envelope  |
| Header | `token`      | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Envelope deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 18. Create a Bank Account

**Description:**
Create a new bank account

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| POST   | `/api/bank-accounts` |

#### **Parameters**

| Type   | Name                | Data Type | Description                                    |
| ------ | ------------------- | --------- | ---------------------------------------------- |
| Header | `token`             | `string ` | JWT authorization token                        |
| Body   | `budgetId`          | `int`     | The budget id for the new bank account         |
| Body   | `accountName`       | `string`  | The name of the bank account                   |
| Body   | `institution`       | `string`  | The institution of the bank account            |
| Body   | `balance`           | `decimal` | The balance of the bank account                |
| Body   | `bankAccountTypeId` | `int`     | The int corresponding to the bank account type |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "accountName": "Bank Account 1",
  "institution": "Bank Institution",
  "balance": 12300.34,
  "bankAccountTypeId": 1
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "bankAccountId": 1,
  "message": "Bank account created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 19. Create a Debt Account

**Description:**
Create a new debt account

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| POST   | `/api/debt-accounts` |

#### **Parameters**

| Type   | Name                  | Data Type  | Description                                                                                             |
| ------ | --------------------- | ---------- | ------------------------------------------------------------------------------------------------------- |
| Header | `token`               | `string `  | JWT authorization token                                                                                 |
| Body   | `budgetId`            | `int`      | The budget id for the new debt account                                                                  |
| Body   | `accountName`         | `string`   | The name of the debt account                                                                            |
| Body   | `institution`         | `string`   | The institution of the debt account                                                                     |
| Body   | `balance`             | `decimal`  | The balance of the debt account                                                                         |
| Body   | `debtAccountTypeId`   | `int`      | The int corresponding to the debt account type                                                          |
| Body   | `accountNumber`       | `int`      | The account number of the debt account - nullable                                                       |
| Body   | `dateOfNextBill`      | `Date`     | The date the next bill is due - nullable                                                                |
| Body   | `amountOfNextBill`    | `decimal ` | The amount of the next bill due - nullable                                                              |
| Body   | `paymentRegularityId` | `int`      | The int corresponding to the regularity of payment if "setup reoccurring payment" is checked - nullable |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "accountName": "Debt Account 1",
  "institution": "Debt Institution",
  "balance": 12300.34,
  "debtAccountTypeId": 4,
  "accountNumber": null,
  "dateOfNextBill": "YYYY-MM-DD",
  "amountOfNextBill": 1234.56,
  "paymentRegularityId": 4
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "debtAccountId": 1,
  "message": "Debt account created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 20. Create an Investment Account

**Description:**
Create a new investment account

#### **Request**

| Method | URL                        |
| ------ | -------------------------- |
| POST   | `/api/investment-accounts` |

#### **Parameters**

| Type   | Name                      | Data Type  | Description                                             |
| ------ | ------------------------- | ---------- | ------------------------------------------------------- |
| Header | `token`                   | `string `  | JWT authorization token                                 |
| Body   | `budgetId`                | `int`      | The budget id for the new investment account            |
| Body   | `accountName`             | `string`   | The name of the investment account                      |
| Body   | `institution`             | `string`   | The institution of the investment account               |
| Body   | `balance`                 | `decimal`  | The balance of the investment account                   |
| Body   | `investmentAccountTypeId` | `int`      | The int corresponding to the investment account type    |
| Body   | `accountNumber`           | `int`      | The account number of the investment account - nullable |
| Body   | `isTaxDeferred`           | `boolean`  | If the investment account is tax deferred               |
| Body   | `isTaxExempt`             | `boolean ` | If the investment account is tax exempt                 |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "accountName": "Investment Account 1",
  "institution": "Investment Institution",
  "balance": 12300.34,
  "investmentAccountTypeId": 2,
  "accountNumber": null,
  "isTaxDeferred": false,
  "isTaxExempt": true,
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "investmentAccountId": 1,
  "message": "Investment account created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 21. Get All Accounts for a Budget

**Description**
Get all bank, debt, and investment accounts for a budget

#### **Request**

| Method | URL                               |
| ------ | --------------------------------- |
| GET    | `/api/accounts/budget/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description             |
| ------ | ---------- | --------- | ----------------------- |
| Path   | `budgetId` | `int`     | The Id of the budget    |
| Header | `token`    | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "accounts": [
    {
        "accountId": 8,
        "budgetId": 1,
        "accountTypeId": 1,
        "accountName": "Bank Account 1",
        "institution": "Bank Institution",
        "balance": 1234.56,
        "bankAccountId": 9,
        "bankAccountTypeId": 2
      },
      {
        "accountId": 8,
        "budgetId": 1,
        "accountTypeId": 1,
        "accountName": "Bank Account 1",
        "institution": "Bank Institution",
        "balance": 1234.56,
        "debtAccountId": 10,
        "debtAccountTypeId": 4,
        "debtAccountNumber": null,
        "dateOfNextBill": "YYYY-MM-DD",
        "amountOfNextBill": 12.45,
        "debtPaymentRegularityId": 2
      },
      {
        "accountId": 8,
        "budgetId": 1,
        "accountTypeId": 1,
        "accountName": "Bank Account 1",
        "institution": "Bank Institution",
        "balance": 1234.56,
        "investmentAccountId": 11,
        "investmentAccountTypeId": 2,
        "investmentAccountNumber": null,
        "isTaxDeferred": true,
        "isTaxExempt": false
      }
  ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 22. Get a Specific Account

**Description:**  
Retrieve detailed information for a specific account.

#### **Request**

| Method | URL                         |
| ------ | --------------------------- |
| GET    | `/api/accounts/{accountId}` |

#### **Parameters**

| Type   | Name        | Data Type | Description           |
| ------ | ----------- | --------- | --------------------- |
| Path   | `accountId` | `int`     | The ID of the account |
| Header | `token`     | `string`  | Bearer JWT token      |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "account": {
    "accountId": 12,
    "budgetId": 1,
    "accountTypeId": 2,
    "accountName": "Debt Account 1",
    "institution": "Debt Institution",
    "balance": 1234.56,
    "debtAccountType": 4,
    "accountNumber": 12345678,
    "dateOfNextBill": "YYYY-MM-DD",
    "amountOfNextBill": 123.45,
    "paymentRegularity": 3,
    "transactions": [
      {
        "transactionId": 5,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 10.99,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Payment",
        "accountId": 13,
        "paymentType": 3,
        "merchantSourceName": "Walmart",
        "checkNumber": null,
        "transactionSplits": [
          {
            "transactionSplitId": 6,
            "transactionId": 5,
            "envelopeId": 3,
            "amount": 3.99
          },
          {
            "transactionSplitId": 7,
            "transactionId": 5,
            "envelopeId": 4,
            "amount": 7.0
          }
        ]
      },
      {
        "transactionId": 15,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 75.0,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Transfer",
        "FromId": 4,
        "ToId": 3,
        "TransferType": 1
      }
    ]
  }
}
```

---

### 23. Get a Specific Bank Account

**Description:**  
Retrieve detailed information for a specific bank account.

#### **Request**

| Method | URL                                  |
| ------ | ------------------------------------ |
| GET    | `/api/bank-accounts/{bankAccountId}` |

#### **Parameters**

| Type   | Name            | Data Type | Description                |
| ------ | --------------- | --------- | -------------------------- |
| Path   | `bankAccountId` | `int`     | The ID of the bank account |
| Header | `token`         | `string`  | Bearer JWT token           |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "account": {
    "accountId": 12,
    "budgetId": 1,
    "accountTypeId": 1,
    "accountName": "Bank Account 1",
    "institution": "Bank Institution",
    "balance": 1234.56,
    "bankAccountType": 3,
    "transactions": [
      {
        "transactionId": 5,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 10.99,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Payment",
        "accountId": 13,
        "paymentType": 3,
        "merchantSourceName": "Walmart",
        "checkNumber": null,
        "transactionSplits": [
          {
            "transactionSplitId": 6,
            "transactionId": 5,
            "envelopeId": 3,
            "amount": 3.99
          },
          {
            "transactionSplitId": 7,
            "transactionId": 5,
            "envelopeId": 4,
            "amount": 7.0
          }
        ]
      },
      {
        "transactionId": 15,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 75.0,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Transfer",
        "FromId": 4,
        "ToId": 3,
        "TransferType": 1
      }
    ]
  }
}
```

---

### 24. Get a Specific Debt Account

**Description:**  
Retrieve detailed information for a specific debt account.

#### **Request**

| Method | URL                                  |
| ------ | ------------------------------------ |
| GET    | `/api/debt-accounts/{debtAccountId}` |

#### **Parameters**

| Type   | Name            | Data Type | Description                |
| ------ | --------------- | --------- | -------------------------- |
| Path   | `debtAccountId` | `int`     | The ID of the debt account |
| Header | `token`         | `string`  | Bearer JWT token           |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "account": {
    "accountId": 12,
    "budgetId": 1,
    "accountTypeId": 2,
    "accountName": "Debt Account 1",
    "institution": "Debt Institution",
    "balance": 1234.56,
    "debtAccountType": 4,
    "accountNumber": 12345678,
    "dateOfNextBill": "YYYY-MM-DD",
    "amountOfNextBill": 123.45,
    "paymentRegularity": 3,
    "transactions": [
      {
        "transactionId": 5,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 10.99,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Payment",
        "accountId": 13,
        "paymentType": 3,
        "merchantSourceName": "Walmart",
        "checkNumber": null,
        "transactionSplits": [
          {
            "transactionSplitId": 6,
            "transactionId": 5,
            "envelopeId": 3,
            "amount": 3.99
          },
          {
            "transactionSplitId": 7,
            "transactionId": 5,
            "envelopeId": 4,
            "amount": 7.0
          }
        ]
      },
      {
        "transactionId": 15,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 75.0,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Transfer",
        "FromId": 4,
        "ToId": 3,
        "TransferType": 1
      }
    ]
  }
}
```

---

### 25. Get a Specific Investment Account

**Description:**  
Retrieve detailed information for a specific investment account.

#### **Request**

| Method | URL                                              |
| ------ | ------------------------------------------------ |
| GET    | `/api/investment-accounts/{investmentAccountId}` |

#### **Parameters**

| Type   | Name                  | Data Type | Description                      |
| ------ | --------------------- | --------- | -------------------------------- |
| Path   | `investmentAccountId` | `int`     | The ID of the investment account |
| Header | `token`               | `string`  | Bearer JWT token                 |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "account": {
    "accountId": 12,
    "budgetId": 1,
    "accountTypeId": 3,
    "accountName": "Investment Account 1",
    "institution": "Investment Institution",
    "balance": 1234.56,
    "investmentAccountType": 4,
    "accountNumber": 12345678,
    "isTaxDeferred": false,
    "isTaxExempt": true,
    "transactions": [
      {
        "transactionId": 5,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 10.99,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Payment",
        "accountId": 13,
        "paymentType": 3,
        "merchantSourceName": "Walmart",
        "checkNumber": null,
        "transactionSplits": [
          {
            "transactionSplitId": 6,
            "transactionId": 5,
            "envelopeId": 3,
            "amount": 3.99
          },
          {
            "transactionSplitId": 7,
            "transactionId": 5,
            "envelopeId": 4,
            "amount": 7.0
          }
        ]
      },
      {
        "transactionId": 15,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 75.0,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Transfer",
        "FromId": 4,
        "ToId": 3,
        "TransferType": 1
      }
    ]
  }
}
```

---

### 26. Update a Bank Account

**Description:**
Update an existing bank account

#### **Request**

| Method | URL                                  |
| ------ | ------------------------------------ |
| PUT    | `/api/bank-accounts/{bankAccountId}` |

#### **Parameters**

| Type   | Name              | Data Type | Description                                               |
| ------ | ----------------- | --------- | --------------------------------------------------------- |
| Path   | `bankAccountId`   | `int`     | The Id of the bank account                                |
| Header | `token`           | `string ` | JWT authorization token                                   |
| Body   | `accountName`     | `string`  | The updated name of the bank account - optional           |
| Body   | `institution`     | `string`  | The institution of the updated bank account - optional    |
| Body   | `balance`         | `decimal` | The balance of the bank account - optional                |
| Body   | `bankAccountType` | `int`     | The int corresponding to the bank account type - optional |

#### **Request Example**

```JSON
{
  "accountName": "Updated Bank Account"
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "bankAccountId": 1,
  "message": "Bank account updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 27. Update a Debt Account

**Description:**
Update an existing debt account

#### **Request**

| Method | URL                                  |
| ------ | ------------------------------------ |
| PUT    | `/api/debt-accounts/{debtAccountId}` |

#### **Parameters**

| Type   | Name                | Data Type  | Description                                                              |
| ------ | ------------------- | ---------- | ------------------------------------------------------------------------ |
| Path   | `debtAccountId`     | `int`      | The Id of the debt account                                               |
| Header | `token`             | `string `  | JWT authorization token                                                  |
| Body   | `accountName`       | `string`   | The updated name of the debt account - optional                          |
| Body   | `institution`       | `string`   | The institution of the updated debt account - optional                   |
| Body   | `balance`           | `decimal`  | The balance of the debt account - optional                               |
| Body   | `debtAccountType`   | `int`      | The int corresponding to the debt account type - optional                |
| Body   | `accountNumber`     | `int`      | The account number of the debt account - nullable - nullable + optional  |
| Body   | `dateOfNextBill`    | `Date`     | The date the next bill is due - nullable + optional                      |
| Body   | `amountOfNextBill`  | `decimal ` | The amount of the next bill due - nullable + optional                    |
| Body   | `paymentRegularity` | `int`      | The int corresponding to the regularity of payment - nullable + optional |

#### **Request Example**

```JSON
{
  "accountName": "Updated Debt Account",
  "amountOfNextBill": 123.45,
  "paymentRegularity": 2
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "debtAccountId": 1,
  "message": "Debt account created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 28. Update an Investment Account

**Description:**
Update an existing investment account

#### **Request**

| Method | URL                                              |
| ------ | ------------------------------------------------ |
| PUT    | `/api/investment-accounts/{investmentAccountId}` |

#### **Parameters**

| Type   | Name                    | Data Type  | Description                                                        |
| ------ | ----------------------- | ---------- | ------------------------------------------------------------------ |
| Path   | `investmentAccountId`   | `int`      | The Id of the investment account                                   |
| Header | `token`                 | `string `  | JWT authorization token                                            |
| Body   | `accountName`           | `string`   | The name of the investment account - optional                      |
| Body   | `institution`           | `string`   | The institution of the investment account - optional               |
| Body   | `balance`               | `decimal`  | The balance of the investment account - optional                   |
| Body   | `investmentAccountType` | `int`      | The int corresponding to the investment account type - optional    |
| Body   | `accountNumber`         | `int`      | The account number of the investment account - nullable + optional |
| Body   | `isTaxDeferred`         | `boolean`  | If the investment account is tax deferred - nullable               |
| Body   | `isTaxExempt`           | `boolean ` | If the investment account is tax exempt - nullable                 |

#### **Request Example**

```JSON
{
  "accountName": "Updated Investment Account"
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "investmentAccountId": 1,
  "message": "Investment account created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 29. Delete an Account

**Description:**
Delete an existing account

#### **Request**

| Method | URL                         |
| ------ | --------------------------- |
| DELETE | `/api/accounts/{accountId}` |

#### **Parameters**

| Type   | Name        | Data Type | Description             |
| ------ | ----------- | --------- | ----------------------- |
| Path   | `accountId` | `int`     | The Id of the account   |
| Header | `token`     | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Account deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 30. Create a Paycheck

**Description:**
Create a new paycheck

#### **Request**

| Method | URL              |
| ------ | ---------------- |
| POST   | `/api/paychecks` |

#### **Parameters**

| Type   | Name            | Data Type | Description                                             |
| ------ | --------------- | --------- | ------------------------------------------------------- |
| Header | `token`         | `string ` | JWT authorization token                                 |
| Body   | `budgetId`      | `int`     | The budget id for the new paycheck                      |
| Body   | `startingDate`  | `Date`    | The starting date for the paycheck                      |
| Body   | `secondaryDate` | `Date`    | The secondary date for the paycheck - nullable          |
| Body   | `totalAmount`   | `decimal` | The total amount of the paycheck                        |
| Body   | `regularityId`  | `int`     | The int corresponding to the regularity of the paycheck |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "startingDate": "YYYY-MM-DD",
  "secondaryDate": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "regularityId": 2
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "paycheckId": 1,
  "message": "Paycheck created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 31. Get All Paychecks

**Description:**
Get all paycheck for a specific budget

#### **Request**

| Method | URL                                |
| ------ | ---------------------------------- |
| GET    | `/api/paychecks/budget/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description                         |
| ------ | ---------- | --------- | ----------------------------------- |
| Path   | `budgetId` | `int`     | The budget id for the paycheck list |
| Header | `token`    | `string ` | JWT authorization token             |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "paychecks": [
    {
        "paycheckId": 9,
        "budgetId": 1,
        "amount": 8000.29,
        "regularityId": 1
      }
  ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 32. Get a Specific Paycheck

**Description:**
Get the details of a single paycheck with the paycheck splits

#### **Request**

| Method | URL                           |
| ------ | ----------------------------- |
| GET    | `/api/paychecks/{paycheckId}` |

#### **Parameters**

| Type   | Name         | Data Type | Description             |
| ------ | ------------ | --------- | ----------------------- |
| Path   | `paycheckId` | `int`     | The paycheck id         |
| Header | `token`      | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "paycheck": {
    "paycheckId": 9,
    "budgetId": 1,
    "startingDate": "YYYY-MM-DD",
    "secondaryDate": null,
    "amount": 8000.29,
    "regularityId": 1,
    "paycheckSplits": [
      {
        "paycheckSplitId": 10,
        "paycheckId": 9,
        "envelopeId": 4,
        "amount": 45.00,
        "orderNumber": 1
      }
    ]
  }
}
```

[↑ Back to top](#api-specification-doc)

---

### 33. Update a Paycheck

**Description:**
Update an existing paycheck

#### **Request**

| Method | URL                           |
| ------ | ----------------------------- |
| PUT    | `/api/paychecks/{paycheckId}` |

#### **Parameters**

| Type   | Name            | Data Type | Description                                                        |
| ------ | --------------- | --------- | ------------------------------------------------------------------ |
| Path   | `paycheckId`    | `int`     | The Id of the paycheck                                             |
| Header | `token`         | `string ` | JWT authorization token                                            |
| Body   | `startingDate`  | `Date`    | The starting date for the paycheck - optional                      |
| Body   | `secondaryDate` | `Date`    | The secondary date for the paycheck - nullable + optional          |
| Body   | `totalAmount`   | `decimal` | The total amount of the paycheck - optional                        |
| Body   | `regularityId`  | `int`     | The int corresponding to the regularity of the paycheck - optional |

#### **Request Example**

```JSON
{
  "startingDate": "YYYY-MM-DD",
  "regularityId": 3
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "paycheckId": 1,
  "message": "Paycheck updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 34. Delete a Paycheck

**Description:**
Delete an existing paycheck

#### **Request**

| Method | URL                           |
| ------ | ----------------------------- |
| DELETE | `/api/paychecks/{paycheckId}` |

#### **Parameters**

| Type   | Name         | Data Type | Description             |
| ------ | ------------ | --------- | ----------------------- |
| Path   | `paycheckId` | `int`     | The Id of the paycheck  |
| Header | `token`      | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Paycheck deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 35. Create a Paycheck Split

**Description:**
Create a new paycheck split

#### **Request**

| Method | URL                     |
| ------ | ----------------------- |
| POST   | `/api/paycheck-splits/` |

#### **Parameters**

| Type   | Name         | Data Type | Description                                |
| ------ | ------------ | --------- | ------------------------------------------ |
| Header | `token`      | `string ` | JWT authorization token                    |
| Body   | `paycheckId` | `int`     | The paycheck id for the new paycheck split |
| Body   | `envelopeId` | `int`     | The envelope id for the paycheck split     |
| Body   | `amount`     | `decimal` | The amount of the paycheck split           |
| Body   | `orderIndex` | `int`     | The order index for the paycheck split     |

#### **Request Example**

```JSON
{
  "paycheckId": 3,
  "envelopeId": 4,
  "amount": 234.56,
  "orderIndex": 2
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "paycheckSplitId": 1,
  "message": "Paycheck split created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 36. Update a Paycheck Split

**Description:**
Update an existing paycheck split

#### **Request**

| Method | URL                                      |
| ------ | ---------------------------------------- |
| PUT    | `/api/paycheck-splits/{paycheckSplitId}` |

#### **Parameters**

| Type   | Name              | Data Type | Description                                        |
| ------ | ----------------- | --------- | -------------------------------------------------- |
| Path   | `paycheckSplitId` | `int`     | The Id of the paycheck split                       |
| Header | `token`           | `string ` | JWT authorization token                            |
| Body   | `envelopeId`      | `int`     | The envelope id for the paycheck split - optional  |
| Body   | `amount`          | `decimal` | The amount of the paycheck - optional              |
| Body   | `orderNumber`     | `int`     | The order number for the paycheck split - optional |

#### **Request Example**

```JSON
{
  "envelopeId": 4
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "paycheckSplitId": 1,
  "message": "Paycheck split updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 37. Delete a Paycheck Split

**Description:**
Delete an existing paycheck split

#### **Request**

| Method | URL                                      |
| ------ | ---------------------------------------- |
| DELETE | `/api/paycheck-splits/{paycheckSplitId}` |

#### **Parameters**

| Type   | Name              | Data Type | Description                  |
| ------ | ----------------- | --------- | ---------------------------- |
| Path   | `paycheckSplitId` | `int`     | The Id of the paycheck split |
| Header | `token`           | `string ` | JWT authorization token      |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Paycheck slit deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 38. Create a Payment Transaction

**Description:**
Create a new payment transaction

#### **Request**

| Method | URL                         |
| ------ | --------------------------- |
| POST   | `/api/payment-transactions` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                                     |
| ------ | -------------------- | --------- | --------------------------------------------------------------- |
| Header | `token`              | `string ` | JWT authorization token                                         |
| Body   | `budgetId`           | `int`     | The budget id for the new payment transaction                   |
| Body   | `transactionDate`    | `Date`    | The date of the payment transaction                             |
| Body   | `totalAmount`        | `decimal` | The total amount of the payment transaction                     |
| Body   | `isReconciled`       | `boolean` | If the payment transaction has been reconciled                  |
| Body   | `notes`              | `string`  | Notes for the payment transaction - nullable                    |
| Body   | `accountId`          | `int`     | The account id for the new payment transaction                  |
| Body   | `paymentTypeId`      | `int`     | The payment type for the new payment transaction                |
| Body   | `merchantSourceName` | `string`  | The name of the merchant/source for the new payment transaction |
| Body   | `checkNumber`        | `int`     | The check number for the new payment transaction - nullable     |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "transactionDate": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "isReconciled": false,
  "notes": "A note for my payment transaction",
  "accountId": 3,
  "paymentTypeId": 3,
  "merchantSourceName": "Walmart",
  "checkNumber": null
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "paymentTransactionId": 1,
  "message": "Payment transaction created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 39. Create a Transfer Transaction

**Description:**
Create a new transfer transaction

#### **Request**

| Method | URL                                            |
| ------ | ---------------------------------------------- |
| POST   | `/api/transfer-transactions/budget/{budgetId}` |

#### **Parameters**

| Type   | Name             | Data Type | Description                                                     |
| ------ | ---------------- | --------- | --------------------------------------------------------------- |
| Header | `token`          | `string ` | JWT authorization token                                         |
| Path   | `budgetId`       | `int`     | The budget id for the new transfer transaction                  |
| Body   | `date`           | `Date`    | The date of the transfer transaction                            |
| Body   | `totalAmount`    | `decimal` | The total amount of the transfer transaction                    |
| Body   | `isReconciled`   | `boolean` | If the transfer transaction has been reconciled                 |
| Body   | `notes`          | `string`  | Notes for the transfer transaction - nullable                   |
| Body   | `fromId`         | `int`     | The id for account or envelope the transfer transaction is from |
| Body   | `toId`           | `int`     | The id for account or envelope the transfer transaction is to   |
| Body   | `transferTypeId` | `int`     | The transfer type for the new transfer transaction              |

#### **Request Example**

```JSON
{
  "date": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "isReconciled": false,
  "notes": "A note for my transfer transaction",
  "fromId": 3,
  "toId": 1,
  "transferTypeId": 1
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "transferTransactionId": 1,
  "message": "Transfer transaction created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 40. Get All Transactions

**Description:**
Get all transactions for a specific budget

#### **Request**

| Method | URL                                   |
| ------ | ------------------------------------- |
| GET    | `/api/transactions/budget/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description                            |
| ------ | ---------- | --------- | -------------------------------------- |
| Path   | `budgetId` | `int`     | The budget id for the transaction list |
| Header | `token`    | `string ` | JWT authorization token                |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "transactions": [
      {
        "transactionId": 5,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 10.99,
        "transactionTypeId": 2,
        "accountId": 13,
        "paymentType": 3,
        "merchantSourceName": "Walmart",
        "transactionSplits": [
          {
            "transactionSplitId": 6,
            "transactionId": 5,
            "envelopeId": 3
          },
          {
            "transactionSplitId": 7,
            "transactionId": 5,
            "envelopeId": 4
          }
        ]
      },
      {
        "transactionId": 15,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 75.00,
        "transactionTypeId": 3,
        "FromId": 4,
        "ToId": 3,
        "TransferType": 1,
      },
    ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 41. Get a Specific Transaction

**Description:**
Get the details of a single transaction with the transaction splits

#### **Request**

| Method | URL                                 |
| ------ | ----------------------------------- |
| GET    | `/api/transactions/{transactionId}` |

#### **Parameters**

| Type   | Name            | Data Type | Description             |
| ------ | --------------- | --------- | ----------------------- |
| Path   | `transactionId` | `int`     | The transaction id      |
| Header | `token`         | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "transaction": {
    "transactionId": 5,
    "budgetId": 1,
    "date": "YYYY-MM-DD",
    "totalAmount": 10.99,
    "isReconciled": false,
    "notes": "This is a note",
    "transactionType": "Payment",
    "accountId": 13,
    "paymentType": 3,
    "merchantSourceName": "Walmart",
    "checkNumber": null,
    "transactionSplits": [
      {
        "transactionSplitId": 6,
        "transactionId": 5,
        "envelopeId": 3,
        "amount": 3.99
      },
      {
        "transactionSplitId": 7,
        "transactionId": 5,
        "envelopeId": 4,
        "amount": 7.00
      }
    ]
  }
}
```

[↑ Back to top](#api-specification-doc)

---

### 42. Get a Specific Payment Transaction

**Description:**
Get the details of a single payment transaction with the transaction splits

#### **Request**

| Method | URL                                                |
| ------ | -------------------------------------------------- |
| GET    | `/api/payment-transactions/{paymentTransactionId}` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                |
| ------ | ---------------------- | --------- | -------------------------- |
| Path   | `paymentTransactionId` | `int`     | The payment transaction id |
| Header | `token`                | `string ` | JWT authorization token    |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "transaction": {
    "transactionId": 5,
    "budgetId": 1,
    "date": "YYYY-MM-DD",
    "totalAmount": 10.99,
    "isReconciled": false,
    "notes": "This is a note",
    "transactionType": "Payment",
    "accountId": 13,
    "paymentType": 3,
    "merchantSourceName": "Walmart",
    "checkNumber": null,
    "transactionSplits": [
      {
        "transactionSplitId": 6,
        "transactionId": 5,
        "envelopeId": 3,
        "amount": 3.99
      },
      {
        "transactionSplitId": 7,
        "transactionId": 5,
        "envelopeId": 4,
        "amount": 7.00
      }
    ]
  }
}
```

[↑ Back to top](#api-specification-doc)

---

### 43. Get a Specific Transfer Transaction

**Description:**
Get the details of a single transfer transaction

#### **Request**

| Method | URL                                                  |
| ------ | ---------------------------------------------------- |
| GET    | `/api/transfer-transactions/{transferTransactionId}` |

#### **Parameters**

| Type   | Name                    | Data Type | Description                 |
| ------ | ----------------------- | --------- | --------------------------- |
| Path   | `transferTransactionId` | `int`     | The transfer transaction id |
| Header | `token`                 | `string ` | JWT authorization token     |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "transactionId": 15,
  "budgetId": 1,
  "date": "YYYY-MM-DD",
  "totalAmount": 75.00,
  "transactionType": "Transfer",
  "FromId": 4,
  "ToId": 3,
  "TransferType": 1,
}
```

[↑ Back to top](#api-specification-doc)

---

### 44. Update a Payment Transaction

**Description:**
Update an existing payment transaction

#### **Request**

| Method | URL                                                |
| ------ | -------------------------------------------------- |
| PUT    | `/api/payment-transactions/{paymentTransactionId}` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                                                                    |
| ------ | ---------------------- | --------- | ------------------------------------------------------------------------------ |
| Header | `token`                | `string ` | JWT authorization token                                                        |
| Path   | `paymentTransactionId` | `int`     | The Id of the payment transaction                                              |
| Body   | `transactionDate`      | `Date`    | The date of the payment transaction - optional                                 |
| Body   | `totalAmount`          | `decimal` | The total amount of the payment transaction - optional                         |
| Body   | `isReconciled`         | `boolean` | If the payment transaction has been reconciled - optional                      |
| Body   | `notes`                | `string`  | Notes for the payment transaction - nullable + optional                        |
| Body   | `accountId`            | `int`     | The account id for the updated payment transaction - optional                  |
| Body   | `paymentTypeId`        | `int`     | The payment type id for the updated payment transaction - optional             |
| Body   | `merchantSourceName`   | `string`  | The name of the merchant/source for the updated payment transaction - optional |
| Body   | `checkNumber`          | `int`     | The check number for the updated payment transaction - nullable + optional     |

#### **Request Example**

```JSON
{
  "date": "YYYY-MM-DD",
  "merchantSourceName": "Frys"
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "paymentTransactionId": 1,
  "message": "Payment transaction updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 45. Update a Transfer Transaction

**Description:**
Update an existing transfer transaction

#### **Request**

| Method | URL                                                  |
| ------ | ---------------------------------------------------- |
| PUT    | `/api/transfer-transactions/{transferTransactionId}` |

#### **Parameters**

| Type   | Name                    | Data Type | Description                                                                |
| ------ | ----------------------- | --------- | -------------------------------------------------------------------------- |
| Header | `token`                 | `string ` | JWT authorization token                                                    |
| Path   | `transferTransactionId` | `int`     | The Id of the transfer transaction                                         |
| Body   | `transactionDate`       | `Date`    | The date of the transfer transaction - optional                            |
| Body   | `totalAmount`           | `decimal` | The total amount of the transfer transaction - optional                    |
| Body   | `isReconciled`          | `boolean` | If the transfer transaction has been reconciled - optional                 |
| Body   | `notes`                 | `string`  | Notes for the transfer transaction - nullable + optional                   |
| Body   | `fromId`                | `int`     | The id for account or envelope the transfer transaction is from - optional |
| Body   | `toId`                  | `int`     | The id for account or envelope the transfer transaction is to - optional   |
| Body   | `transferType`          | `int`     | The transfer type for the updated transfer transaction - optional          |

#### **Request Example**

```JSON
{
  "toId": 5
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "transferTransactionId": 1,
  "message": "Transfer transaction updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 46. Soft Delete a Transaction

**Description:**
Soft delete an existing transaction (set the deleted at, but don't actually delete to create a recently deleted record)

#### **Request**

| Method | URL                                 |
| ------ | ----------------------------------- |
| DELETE | `/api/transactions/{transactionId}` |

#### **Parameters**

| Type   | Name            | Data Type | Description               |
| ------ | --------------- | --------- | ------------------------- |
| Path   | `transactionId` | `int`     | The Id of the transaction |
| Header | `token`         | `string ` | JWT authorization token   |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Transaction deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 47. Create a Transaction Split

**Description:**
Create a new transaction split

#### **Request**

| Method | URL                       |
| ------ | ------------------------- |
| POST   | `/api/transaction-splits` |

#### **Parameters**

| Type   | Name            | Data Type | Description                                      |
| ------ | --------------- | --------- | ------------------------------------------------ |
| Header | `token`         | `string ` | JWT authorization token                          |
| Body   | `transactionId` | `int`     | The transaction id for the new transaction split |
| Body   | `envelopeId`    | `int`     | The envelope id for the transaction split        |
| Body   | `amount`        | `decimal` | The amount of the transaction split              |

#### **Request Example**

```JSON
{
  "transactionId": 1,
  "envelopeId": 4,
  "amount": 234.56
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "transactionSplitId": 1,
  "message": "Transaction split created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 48. Update a Transaction Split

**Description:**
Update an existing transaction split

#### **Request**

| Method | URL                                            |
| ------ | ---------------------------------------------- |
| PUT    | `/api/transaction-splits/{transactionSplitId}` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                          |
| ------ | -------------------- | --------- | ---------------------------------------------------- |
| Header | `token`              | `string ` | JWT authorization token                              |
| Path   | `transactionSplitId` | `int`     | The Id of the transaction split                      |
| Body   | `envelopeId`         | `int`     | The envelope id for the transaction split - optional |
| Body   | `amount`             | `decimal` | The amount of the transaction - optional             |

#### **Request Example**

```JSON
{
  "envelopeId": 2
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "transactionSplitId": 1,
  "message": "Transaction split updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 49. Delete a Transaction Split

**Description:**
Delete an existing transaction split

#### **Request**

| Method | URL                                            |
| ------ | ---------------------------------------------- |
| DELETE | `/api/transaction-splits/{transactionSplitId}` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                     |
| ------ | -------------------- | --------- | ------------------------------- |
| Path   | `transactionSplitId` | `int`     | The Id of the transaction split |
| Header | `token`              | `string ` | JWT authorization token         |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Transaction split deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 50. Create a Planned Expense

**Description:**
Create a new planned expense

#### **Request**

| Method | URL                     |
| ------ | ----------------------- |
| POST   | `/api/planned-expenses` |

#### **Parameters**

| Type   | Name                         | Data Type | Description                                             |
| ------ | ---------------------------- | --------- | ------------------------------------------------------- |
| Header | `token`                      | `string ` | JWT authorization token                                 |
| Body   | `budgetId`                   | `int`     | The budget id for the new planned expense               |
| Body   | `envelopeId`                 | `int`     | The envelope id for the planned expense                 |
| Body   | `plannedExpenseRegularityId` | `int`     | The Id for the regularity of the planned expense        |
| Body   | `dayOfMonth`                 | `int`     | The day of the month for the planned expense - nullable |
| Body   | `weekdayId`                  | `int`     | The Id of the weekday for the regularity - nullable     |
| Body   | `plannedExpenseOccurrenceId` | `int`     | The Id for the planned expense occurrence - nullable    |
| Body   | `amount`                     | `decimal` | The amount of the planned expense                       |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "envelopeId": 2,
  "plannedExpenseRegularityId": 2,
  "dayOfMonth": 10,
  "weekdayId": null,
  "plannedExpenseOccurrenceId": null,
  "amount": 12.34
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "plannedExpenseId": 1,
  "message": "Planned expense created successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 51. Get All Planned Expenses For a Budget

**Description:**
Get all planned expenses for a specific budget

#### **Request**

| Method | URL                                       |
| ------ | ----------------------------------------- |
| GET    | `/api/planned-expenses/budget/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description                                 |
| ------ | ---------- | --------- | ------------------------------------------- |
| Path   | `budgetId` | `int`     | The budget id for the planned expenses list |
| Header | `token`    | `string ` | JWT authorization token                     |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "plannedExpenses": [
    {
      "plannedExpenseId": 14,
      "budgetId": 1,
      "envelopeId": 2,
      "plannedExpenseRegularityId": 2,
      "dayOfMonth": 10,
      "weekdayId": null,
      "plannedExpenseOccurrenceId": null,
      "amount": 12.34
    }
  ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 52. Get All Planned Expenses For an Envelope

**Description:**
Get all planned expenses for a specific envelope

#### **Request**

| Method | URL                                                             |
| ------ | --------------------------------------------------------------- |
| GET    | `/api/planned-expenses/budget/{budgetId}/envelope/{envelopeId}` |

#### **Parameters**

| Type   | Name         | Data Type | Description                                   |
| ------ | ------------ | --------- | --------------------------------------------- |
| Path   | `budgetId`   | `int`     | The budget id for the planned expenses list   |
| Path   | `envelopeId` | `int`     | The envelope id for the planned expenses list |
| Header | `token`      | `string ` | JWT authorization token                       |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "plannedExpenses": [
    {
      "plannedExpenseId": 14,
      "budgetId": 1,
      "envelopeId": 2,
      "plannedExpenseRegularityId": 2,
      "dayOfMonth": 10,
      "weekdayId": null,
      "plannedExpenseOccurrenceId": null,
      "amount": 12.34
    }
  ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 53. Get All Planned Expenses For an Account

**Description:**
Get all planned expenses for a specific account

#### **Request**

| Method | URL                                                           |
| ------ | ------------------------------------------------------------- |
| GET    | `/api/planned-expenses/budget/{budgetId}/account/{accountId}` |

#### **Parameters**

| Type   | Name        | Data Type | Description                                  |
| ------ | ----------- | --------- | -------------------------------------------- |
| Path   | `budgetId`  | `int`     | The budget id for the planned expenses list  |
| Path   | `accountId` | `int`     | The account id for the planned expenses list |
| Header | `token`     | `string ` | JWT authorization token                      |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "plannedExpenses": [
    {
      "plannedExpenseId": 14,
      "budgetId": 1,
      "envelopeId": 2,
      "plannedExpenseRegularityId": 2,
      "dayOfMonth": 10,
      "weekdayId": null,
      "plannedExpenseOccurrenceId": null,
      "amount": 12.34
    }
  ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 54. Get a Specific Planned Expense

**Description:**
Get the details of a single planned expense

#### **Request**

| Method | URL                                        |
| ------ | ------------------------------------------ |
| GET    | `/api/planned-expenses/{plannedExpenseId}` |

#### **Parameters**

| Type   | Name               | Data Type | Description             |
| ------ | ------------------ | --------- | ----------------------- |
| Path   | `plannedExpenseId` | `int`     | The planned expense id  |
| Header | `token`            | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "plannedExpenses": [
    {
      "plannedExpenseId": 14,
      "budgetId": 1,
      "envelopeId": 2,
      "plannedExpenseRegularityId": 2,
      "dayOfMonth": 10,
      "weekdayId": null,
      "plannedExpenseOccurrenceId": null,
      "amount": 12.34
    }
  ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 55. Update a Planned Expense

**Description:**
Update an existing planned expense

#### **Request**

| Method | URL                                        |
| ------ | ------------------------------------------ |
| PUT    | `/api/planned-expenses/{plannedExpenseId}` |

#### **Parameters**

| Type   | Name                         | Data Type | Description                                                        |
| ------ | ---------------------------- | --------- | ------------------------------------------------------------------ |
| Header | `token`                      | `string ` | JWT authorization token                                            |
| Path   | `plannedExpenseId`           | `int`     | The Id of the planned expense                                      |
| Body   | `envelopeId`                 | `int`     | The envelope id for the planned expense - optional                 |
| Body   | `plannedExpenseRegularityId` | `int`     | The Id for the regularity of the planned expense - optional        |
| Body   | `dayOfMonth`                 | `int`     | The day of the month for the planned expense - nullable + optional |
| Body   | `weekdayId`                  | `int`     | The Id of the weekday for the regularity - nullable + optional     |
| Body   | `plannedExpenseOccurrenceId` | `int`     | The Id for the planned expense occurrence - nullable + optional    |
| Body   | `amount`                     | `decimal` | The amount of the planned expense - optional                       |

#### **Request Example**

```JSON
{
  "dayOfMonth": 19
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "plannedExpenseId": 1,
  "message": "Planned expense updated successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

### 56. Delete a Planned Expense

**Description:**
Delete an existing planned expense

#### **Request**

| Method | URL                                        |
| ------ | ------------------------------------------ |
| DELETE | `/api/planned-expenses/{plannedExpenseId}` |

#### **Parameters**

| Type   | Name               | Data Type | Description                   |
| ------ | ------------------ | --------- | ----------------------------- |
| Path   | `plannedExpenseId` | `int`     | The Id of the planned Expense |
| Header | `token`            | `string ` | JWT authorization token       |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Planned expense deleted successfully"
}
```

[↑ Back to top](#api-specification-doc)

---

## 3. Common Error Codes

**Unauthorized(401)**

```JSON
{
  "status": 401,
  "error": "User not authenticated"
}
```

**Forbidden (403)**

```json
{
  "status": 403,
  "error": "Forbidden. You do not have access to this resource."
}
```

**Not Found(404)**

```JSON
{
  "status": 404,
  "error": "[Resource] not found."
}
```

**Internal Server Error(500)**

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

## 4. Glossary

### Status Codes

All status codes follow the standard HTTP conventions.

**Categories:**

- **2XX** – Success of some kind
- **4XX** – Error occurred on client’s part
- **5XX** – Error occurred on server’s part

| Status Code | Name                  | Description                                                                                                                   |
| ----------- | --------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| 200         | OK                    | The request was successful. The response body contains the requested data.                                                    |
| 201         | Created               | A new resource was successfully created. The response typically includes the new resource in the body a                       |
| 202         | Accepted              | The request has been accepted for processing but is not yet complete. Used for asynchronous operations.                       |
| 204         | No Content            | The request was successful but there is no content to return. Commonly used for DELETE operations.                            |
| 400         | Bad Request           | The request was malformed or contains invalid syntax. Check that all required parameters are included and properly formatted. |
| 401         | Unauthorized          | Authentication credentials are missing, invalid, or expired. Verify your API key or access token.                             |
| 403         | Forbidden             | The request is valid, but you don't have permission to access this resource. Check your account permissions.                  |
| 404         | Not Found             | The requested resource does not exist. Verify the resource ID and endpoint URL.                                               |
| 405         | Method Not Allowed    | The HTTP method used is not supported for this endpoint. Check the allowed methods in the endpoint documentation.             |
| 409         | Conflict              | The request conflicts with the current state of the resource. This often occurs when trying to create a duplicate resource.   |
| 422         | Unprocessable Entity  | The request is well-formed but contains semantic errors. Check that field values meet validation requirements.                |
| 429         | Too Many Requests     | You have exceeded the rate limit. Wait before making additional requests or upgrade your plan.                                |
| 500         | Internal Server Error | An unexpected error occurred on the server. If this persists, contact support.                                                |
| 502         | Bad Gateway           | The server received an invalid response from an upstream server. This is usually temporary—try again.                         |
| 503         | Service Unavailable   | The service is temporarily unavailable, often due to maintenance or high load. Retry your request after a short delay.        |
| 504         | Gateway Timeout       | The server did not receive a timely response from an upstream server. Retry your request.                                     |

[↑ Back to top](#api-specification-doc)

---

## 5. Authentication

### Authentication Method

Makes Cents uses JWT (JSON Web Token) bearer authentication.

### How to Authenticate Requests

Include the token in the Authorization header for all authenticated endpoints:

```http
Authorization: Bearer your_token_here
```

### Obtaining a Token

Tokens are obtained through:

- User registration (POST /api/user/register)
- User login (POST /api/user/login)

### Token Expiration

Tokens expire after 24 hours. When a token expires, you'll receive a 401 Unauthorized response. Simply log in again to obtain a new token.

```

```
