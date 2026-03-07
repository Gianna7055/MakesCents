# API Specification Doc

**Makes Cents**

| Version | Date             | Author      | Description            |
| ------- | ---------------- | ----------- | ---------------------- |
| 1.0     | 8 November 2024  | Gianna Ross | Initial draft          |
| 2.0     | 21 December 2024 | Gianna Ross | Revisions for GET APIs |

---

## Table of Contents

1. [Common Concepts](#1-common-concepts)
   - [Request/Response Format](#requestresponse-format)
   - [Nullable Fields](#nullable-fields)
   - [ID References](#id-references)
   - [Soft Deletes](#soft-deletes)
   - [User IDs](#user-ids)

2. [API Reference](#2-api-reference)

   **User Management**
   - [1. Create a new user account](#1-create-a-new-user-account)
   - [2. Authenticate a User](#2-authenticate-a-user)
   - [3. Get User Profile](#3-get-user-profile)
   - [4. Update a User](#4-update-a-user)
   - [5. Delete a User](#5-delete-a-user)

   **Budget Management**
   - [6. Create a Budget](#6-create-a-budget)
   - [7. Get a Budget](#7-get-a-budget)
   - [8. Update a Budget](#8-update-a-budget)
   - [9. Delete a Budget](#9-delete-a-budget)

   **Envelope Categories**
   - [10. Create an Envelope Category](#10-create-an-envelope-category)
   - [11. Get All Categories for a Budget](#11-get-all-categories-for-a-budget)
   - [12. Update an Envelope Category](#12-update-an-envelope-category)
   - [13. Delete an Envelope Category](#13-delete-an-envelope-category)

   **Envelopes**
   - [14. Create an Envelope](#14-create-an-envelope)
   - [15. Get a Specific Envelope](#15-get-a-specific-envelope)
   - [16. Update an Envelope](#16-update-an-envelope)
   - [17. Delete an Envelope](#17-delete-an-envelope)

   **Accounts**
   - [18. Create a Bank Account](#18-create-a-bank-account)
   - [19. Create a Debt Account](#19-create-a-debt-account)
   - [20. Create an Investment Account](#20-create-an-investment-account)
   - [21. Get All Accounts for a Budget](#21-get-all-accounts-for-a-budget)
   - [23. Get a Specific Bank Account](#23-get-a-specific-bank-account)
   - [24. Get a Specific Debt Account](#24-get-a-specific-debt-account)
   - [25. Get a Specific Investment Account](#25-get-a-specific-investment-account)
   - [26. Update a Bank Account](#26-update-a-bank-account)
   - [27. Update a Debt Account](#27-update-a-debt-account)
   - [28. Update an Investment Account](#28-update-an-investment-account)
   - [29. Delete an Account](#29-delete-an-account)

   **Paychecks**
   - [30. Create a Paycheck](#30-create-a-paycheck)
   - [31. Get All Paychecks](#31-get-all-paychecks)
   - [32. Get a Specific Paycheck](#32-get-a-specific-paycheck)
   - [33. Update a Paycheck](#33-update-a-paycheck)
   - [34. Delete a Paycheck](#34-delete-a-paycheck)

   **Transactions**
   - [35. Create a Payment Transaction](#35-create-a-payment-transaction)
   - [36. Create a Transfer Transaction](#36-create-a-transfer-transaction)
   - [37. Get All Transactions](#37-get-all-transactions)
   - [38. Get a Specific Transaction](#38-get-a-specific-transaction)
   - [39. Get a Specific Payment Transaction](#39-get-a-specific-payment-transaction)
   - [40. Get a Specific Transfer Transaction](#40-get-a-specific-transfer-transaction)
   - [41. Update a Payment Transaction](#41-update-a-payment-transaction)
   - [42. Update a Transfer Transaction](#42-update-a-transfer-transaction)
   - [43. Soft Delete a Transaction](#43-soft-delete-a-transaction)

3. [Common Error Codes](#3-common-error-codes)

4. [Glossary](#4-glossary)
   - [Status Codes](#status-codes)

5. [Authentication](#5-authentication)
   - [Authentication Method](#authentication-method)
   - [How to Authenticate Requests](#how-to-authenticate-requests)
   - [Obtaining a Token](#obtaining-a-token)
   - [Token Expiration](#token-expiration)

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

### User IDs

If needed, the user's Id will be retrieved from the JWT token

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

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Invalid email format"
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

| Type | Name              | Data Type | Description                                  |
| ---- | ----------------- | --------- | -------------------------------------------- |
| Body | `usernameOrEmail` | `string`  | The username or email of the user logging in |
| Body | `password`        | `string`  | The hashed password of the user logging in   |

#### **Request Example**

```JSON
{
  "usernameOrEmail": "johndoe",
  "password": "akjfhen48w9ruibfwle"
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

| Method | URL         |
| ------ | ----------- |
| GET    | `/api/user` |

#### **Parameters**

| Type   | Name    | Data Type | Description      |
| ------ | ------- | --------- | ---------------- |
| Header | `token` | `string`  | Bearer JWT token |

#### **Responses**

**Success (200 OK)**

```json
{
  "status": 200,
  "message": "User found",
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

| Method | URL         |
| ------ | ----------- |
| PUT    | `/api/user` |

#### **Parameters**

| Type   | Name             | Data Type | Description                                                      |
| ------ | ---------------- | --------- | ---------------------------------------------------------------- |
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

| Method | URL         |
| ------ | ----------- |
| DELETE | `/api/user` |

#### **Parameters**

| Type   | Name    | Data Type | Description             |
| ------ | ------- | --------- | ----------------------- |
| Header | `token` | `string ` | JWT authorization token |

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
Create a new blank budget for a specific month and year.

#### **Request**

| Method | URL            |
| ------ | -------------- |
| POST   | `/api/budgets` |

#### **Parameters**

| Type   | Name         | Data Type | Description              |
| ------ | ------------ | --------- | ------------------------ |
| Header | `token`      | `string ` | JWT authorization token  |
| Body   | `month`      | `string`  | The month for the budget |
| Body   | `year`       | `int`     | The year for the budget  |
| Body   | `budgetName` | `string`  | The name of the budget   |

#### **Request Example**

```JSON
{
  "month": "November",
  "year": 2025,
  "budgetName": "My budget"
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "message": "Budget created successfully",
  "budgetId": 1
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

| Method | URL                                        |
| ------ | ------------------------------------------ |
| GET    | `/api/budgets/year/{year}/month/{monthId}` |

#### **Parameters**

| Type   | Name      | Data Type | Description             |
| ------ | --------- | --------- | ----------------------- |
| Path   | `year`    | `int`     | The year                |
| Path   | `monthId` | `int`     | The month ID (1-12)     |
| Header | `token`   | `string ` | JWT authorization token |

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Budget found",
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

**Not Found (404)**

```json
{
  "status": 404,
  "error": "Budget not found",
  "userId": 11,
  "monthId": 11,
  "year": 2025
}
```

[↑ Back to top](#api-specification-doc)

---

### 8. Update a Budget

**Description:**  
Update an existing budget

#### **Request**

| Method | URL             |
| ------ | --------------- |
| PUT    | `/api/budgets/` |

#### **Parameters**

| Type   | Name       | Data Type | Description                       |
| ------ | ---------- | --------- | --------------------------------- |
| Path   | `budgetId` | `int`     | The ID of the budget              |
| Header | `token`    | `string ` | JWT authorization token           |
| Body   | `name`     | `string`  | The name of the budget - optional |

#### **Request Example**

```JSON
{
  "budgetName": "Updated Budget"
}
```

#### **Responses**

```json
{
  "status": 200,
  "message": "Budget updated successfully",
  "budgetId": 1
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

#### **Request Example**

```JSON
{
  "budgetId": 2,
  "envelopeCategoryName": "Envelope Category 1"
}
```

#### Responses

**Success (201 Created)**

```json
{
  "status": 201,
  "message": "Envelope category created successfully",
  "envelopeCategoryId": 1
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
  "message": "Envelope categories found",
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

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Sinking funds require a goal amount and goal end date"
}
```

**Bad Request (400)**

```json
{
  "status": 400,
  "error": "Rollover funds require a transfer envelope"
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
        "date": "YYYY-MM-DD",
        "location": "Walmart",
        "envelopes": "Groceries, Clothing",
        "totalAmount": 10.99
      },
      {
        "transactionId": 15,
        "date": "YYYY-MM-DD",
        "location": "Account Transfer",
        "envelopes": "Squirrel Fund -> Rent",
        "totalAmount": 75.0
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

| Type   | Name              | Data Type | Description                            |
| ------ | ----------------- | --------- | -------------------------------------- |
| Header | `token`           | `string ` | JWT authorization token                |
| Body   | `budgetId`        | `int`     | The budget id for the new bank account |
| Body   | `accountName`     | `string`  | The name of the bank account           |
| Body   | `institution`     | `string`  | The institution of the bank account    |
| Body   | `balance`         | `decimal` | The balance of the bank account        |
| Body   | `bankAccountType` | `string`  | The bank account type                  |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "accountName": "Bank Account 1",
  "institution": "Bank Institution",
  "balance": 12300.34,
  "bankAccountType": "Savings"
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "message": "Bank account created successfully",
  "bankAccountId": 1
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

| Type   | Name                    | Data Type  | Description                                                                                             |
| ------ | ----------------------- | ---------- | ------------------------------------------------------------------------------------------------------- |
| Header | `token`                 | `string `  | JWT authorization token                                                                                 |
| Body   | `budgetId`              | `int`      | The budget id for the new debt account                                                                  |
| Body   | `accountName`           | `string`   | The name of the debt account                                                                            |
| Body   | `institution`           | `string`   | The institution of the debt account                                                                     |
| Body   | `balance`               | `decimal`  | The balance of the debt account                                                                         |
| Body   | `debtAccountType`       | `int`      | The debt account type (matches enums)                                                                   |
| Body   | `accountNumber`         | `int`      | The account number of the debt account - optional                                                       |
| Body   | `dateOfNextBill`        | `Date`     | The date the next bill is due - optional                                                                |
| Body   | `amountOfNextBill`      | `decimal ` | The amount of the next bill due - optional                                                              |
| Body   | `debtPaymentRegularity` | `int`      | The int corresponding to the regularity of payment if "setup reoccurring payment" is checked - optional |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "accountName": "Debt Account 1",
  "institution": "Debt Institution",
  "balance": 12300.34,
  "debtAccountType": "CreditCard",
  "accountNumber": null,
  "dateOfNextBill": "YYYY-MM-DD",
  "amountOfNextBill": 1234.56,
  "debtPaymentRegularity": "Monthly"
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

| Type   | Name                    | Data Type  | Description                                             |
| ------ | ----------------------- | ---------- | ------------------------------------------------------- |
| Header | `token`                 | `string `  | JWT authorization token                                 |
| Body   | `budgetId`              | `int`      | The budget id for the new investment account            |
| Body   | `accountName`           | `string`   | The name of the investment account                      |
| Body   | `institution`           | `string`   | The institution of the investment account               |
| Body   | `balance`               | `decimal`  | The balance of the investment account                   |
| Body   | `investmentAccountType` | `int`      | The int corresponding to the investment account type    |
| Body   | `accountNumber`         | `int`      | The account number of the investment account - optional |
| Body   | `isTaxDeferred`         | `boolean`  | If the investment account is tax deferred               |
| Body   | `isTaxExempt`           | `boolean ` | If the investment account is tax exempt                 |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "accountName": "Investment Account 1",
  "institution": "Investment Institution",
  "balance": 12300.34,
  "investmentAccountTypeId": "IRA",
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
  "message": "Accounts found",
  "accounts": [
    {
        "accountId": 8,
        "budgetId": 1,
        "accountTypeId": 1,
        "accountName": "Bank Account 1",
        "balance": 1234.56,
        "bankAccountId": 9,
        "bankAccountType": "Savings"
      },
      {
        "accountId": 9,
        "budgetId": 1,
        "accountTypeId": 1,
        "accountName": "Debt Account 1",
        "balance": 1234.56,
        "debtAccountId": 10,
        "debtAccountType": "CarLoan",
      },
      {
        "accountId": 10,
        "budgetId": 1,
        "accountTypeId": 1,
        "accountName": "Investment Account 1",
        "balance": 1234.56,
        "investmentAccountId": 11,
        "investmentAccountTypeId": "IRA",
      }
  ]
}
```

[↑ Back to top](#api-specification-doc)

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
  "bankAccount": {
    "accountId": 12,
    "budgetId": 1,
    "accountType": "Bank",
    "accountName": "Bank Account 1",
    "institution": "Bank Institution",
    "balance": 1234.56,
    "bankAccountId": 2,
    "bankAccountType": "Checking",
    "transactions": [
      {
        "transactionId": 5,
        "date": "YYYY-MM-DD",
        "location": "Walmart",
        "envelopes": "Groceries, Clothing",
        "totalAmount": 10.99
      },
      {
        "transactionId": 15,
        "date": "YYYY-MM-DD",
        "location": "Account Transfer",
        "envelopes": "Squirrel Fund -> Rent",
        "totalAmount": 75.0
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
    "debtAccountId": 4,
    "debtAccountType": "Mortgage",
    "accountNumber": 12345678,
    "dateOfNextBill": "YYYY-MM-DD",
    "amountOfNextBill": 123.45,
    "debtPaymentRegularity": "Monthly",
    "transactions": [
      {
        "transactionId": 5,
        "date": "YYYY-MM-DD",
        "location": "Walmart",
        "envelopes": "Groceries, Clothing",
        "totalAmount": 10.99
      },
      {
        "transactionId": 15,
        "date": "YYYY-MM-DD",
        "location": "Account Transfer",
        "envelopes": "Squirrel Fund -> Rent",
        "totalAmount": 75.0
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
    "accountType": "Investment",
    "accountName": "Investment Account 1",
    "institution": "Investment Institution",
    "balance": 1234.56,
    "investmentAccountId": 9,
    "investmentAccountType": "Brokerage",
    "accountNumber": 12345678,
    "isTaxDeferred": false,
    "isTaxExempt": true,
    "transactions": [
      {
        "transactionId": 5,
        "date": "YYYY-MM-DD",
        "location": "Walmart",
        "envelopes": "Groceries, Clothing",
        "totalAmount": 10.99
      },
      {
        "transactionId": 15,
        "date": "YYYY-MM-DD",
        "location": "Account Transfer",
        "envelopes": "Squirrel Fund -> Rent",
        "totalAmount": 75.0
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

| Type   | Name                    | Data Type  | Description                                                              |
| ------ | ----------------------- | ---------- | ------------------------------------------------------------------------ |
| Path   | `debtAccountId`         | `int`      | The Id of the debt account                                               |
| Header | `token`                 | `string `  | JWT authorization token                                                  |
| Body   | `accountName`           | `string`   | The updated name of the debt account - optional                          |
| Body   | `institution`           | `string`   | The institution of the updated debt account - optional                   |
| Body   | `balance`               | `decimal`  | The balance of the debt account - optional                               |
| Body   | `debtAccountType`       | `int`      | The int corresponding to the debt account type - optional                |
| Body   | `accountNumber`         | `int`      | The account number of the debt account - nullable - nullable + optional  |
| Body   | `dateOfNextBill`        | `Date`     | The date the next bill is due - nullable + optional                      |
| Body   | `amountOfNextBill`      | `decimal ` | The amount of the next bill due - nullable + optional                    |
| Body   | `debtPaymentRegularity` | `int`      | The int corresponding to the regularity of payment - nullable + optional |

#### **Request Example**

```JSON
{
  "accountName": "Updated Debt Account",
  "amountOfNextBill": 123.45,
  "debtPaymentRegularity": 2
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

| Type   | Name                 | Data Type | Description                                             |
| ------ | -------------------- | --------- | ------------------------------------------------------- |
| Header | `token`              | `string ` | JWT authorization token                                 |
| Body   | `budgetId`           | `int`     | The budget id for the new paycheck                      |
| Body   | `startingDate`       | `Date`    | The starting date for the paycheck                      |
| Body   | `secondaryDate`      | `Date`    | The secondary date for the paycheck - nullable          |
| Body   | `totalAmount`        | `decimal` | The total amount of the paycheck                        |
| Body   | `paycheckRegularity` | `int`     | The int corresponding to the regularity of the paycheck |
| Body   | `envelopeId`         | `int`     | The envelope id for the paycheck split                  |
| Body   | `amount`             | `decimal` | The amount of the paycheck split                        |
| Body   | `orderIndex`         | `int`     | The order index for the paycheck split                  |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "startingDate": "YYYY-MM-DD",
  "secondaryDate": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "paycheckRegularity": "BiWeeklyEveryTwoWeeks;",
  "paycheckSplits": [
    {
      "envelopeId": 4,
      "amount": 234.56
    },
    {
      "envelopeId": 3,
      "amount": 1000
    },
  ]
}
```

#### **Responses**

**Success (201 Created)**

```json
{
  "status": 201,
  "message": "Paycheck created successfully",
  "paycheckId": 1,
  "paycheckSplitIds": [1, 2, 3]
}
```

**Bad Request(400)**

```JSON
{
  "status": 400,
  "error": "Paycheck must contain at least one split"
}
```

**Bad Request(400)**

```JSON
{
  "status": 400,
  "error": "Split cannot be null"
}
```

**Bad Request(400)**

```JSON
{
  "status": 400,
  "error": "Split amount must be greater than 0"
}
```

**Bad Request(400)**

```JSON
{
  "status": 400,
  "error": "Split envelope is required"
}
```

**Bad Request(400)**

```JSON
{
  "status": 400,
  "error": "Split totals must equal paycheck total"
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
      "paycheckId": 2,
      "startingDate": "YYYY-MM-DD",
      "secondaryDate": null,
      "totalAmount": 1234.56,
      "paycheckRegularity": "BiWeeklyEveryTwoWeeks;"
    },
    {
      "paycheckId": 8,
      "startingDate": "YYYY-MM-DD",
      "secondaryDate": "YYYY-MM-DD",
      "totalAmount": 876.43,
      "paycheckRegularity": "BiMonthlyTwiceAMonth;"
    },
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
  "regularityId": 3,
  "paycheckSplits": [
    {
      "envelopeId": 2,
      "orderIndex": 2
    },
    {
      "amount": 500,
      "orderIndex": 1
    },
  ]
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Paycheck updated successfully",
  "paycheckId": 1,
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

### 35. Create a Payment Transaction

**Description:**
Create a new payment transaction

#### **Request**

| Method | URL                         |
| ------ | --------------------------- |
| POST   | `/api/payment-transactions` |

#### **Parameters**

| Type   | Name                     | Data Type | Description                                                     |
| ------ | ------------------------ | --------- | --------------------------------------------------------------- |
| Header | `token`                  | `string ` | JWT authorization token                                         |
| Body   | `budgetId`               | `int`     | The budget id for the new payment transaction                   |
| Body   | `transactionDate`        | `Date`    | The date of the payment transaction                             |
| Body   | `totalAmount`            | `decimal` | The total amount of the payment transaction                     |
| Body   | `isReconciled`           | `boolean` | If the payment transaction has been reconciled                  |
| Body   | `notes`                  | `string`  | Notes for the payment transaction - nullable                    |
| Body   | `accountId`              | `int`     | The account id for the new payment transaction                  |
| Body   | `paymentTransactionType` | `string`  | The payment type for the new payment transaction                |
| Body   | `merchantSourceName`     | `string`  | The name of the merchant/source for the new payment transaction |
| Body   | `checkNumber`            | `int`     | The check number for the new payment transaction - nullable     |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "transactionDate": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "notes": "A note for my payment transaction",
  "accountId": 3,
  "paymentTypeId": "DebitCard",
  "merchantSourceName": "Walmart",
  "checkNumber": null,
  "transactionSplits": [
    {
      "transactionId": 1,
      "envelopeId": 4,
      "amount": 234.56
    },
    {
      "transactionId": 1,
      "envelopeId": 2,
      "amount": 1000
    }
  ]
}
```

#### **Responses**

**Success (200 OK)**

```JSON
{
  "status": 200,
  "message": "Payment transaction created successfully",
  "transactionId": 3,
  "paymentTransactionId": 1,
  "transactionSplitIds": [1, 2, 3]
}
```

[↑ Back to top](#api-specification-doc)

---

### 36. Create a Transfer Transaction

**Description:**
Create a new transfer transaction

#### **Request**

| Method | URL                          |
| ------ | ---------------------------- |
| POST   | `/api/transfer-transactions` |

#### **Parameters**

| Type   | Name                      | Data Type | Description                                                     |
| ------ | ------------------------- | --------- | --------------------------------------------------------------- |
| Header | `token`                   | `string ` | JWT authorization token                                         |
| Body   | `budgetId`                | `int`     | The budget id for the new transfer transaction                  |
| Body   | `date`                    | `Date`    | The date of the transfer transaction                            |
| Body   | `totalAmount`             | `decimal` | The total amount of the transfer transaction                    |
| Body   | `isReconciled`            | `boolean` | If the transfer transaction has been reconciled                 |
| Body   | `notes`                   | `string`  | Notes for the transfer transaction - nullable                   |
| Body   | `fromId`                  | `int`     | The id for account or envelope the transfer transaction is from |
| Body   | `toId`                    | `int`     | The id for account or envelope the transfer transaction is to   |
| Body   | `transferTransactionType` | `string`  | The transfer type for the new transfer transaction              |

#### **Request Example**

```JSON
{
  "budgetId": 1,
  "date": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "isReconciled": false,
  "notes": "A note for my transfer transaction",
  "fromId": 3,
  "toId": 1,
  "transferTransactionType": "Envelope"
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

### 37. Get All Transactions

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
        "date": "YYYY-MM-DD",
        "location": "Walmart",
        "envelopes": "Groceries, Clothing",
        "totalAmount": 10.99
      },
      {
        "transactionId": 15,
        "date": "YYYY-MM-DD",
        "location": "Account Transfer",
        "envelopes": "Squirrel Fund -> Rent",
        "totalAmount": 75.0
      },
    ]
}
```

[↑ Back to top](#api-specification-doc)

---

### 39. Get a Specific Payment Transaction

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
  "paymentTransaction": {
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

### 40. Get a Specific Transfer Transaction

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
  "status": 201,
  "message": "Transfer transaction found",
  "transferTransaction":
  {
    "transactionId": 15,
    "budgetId": 1,
    "date": "YYYY-MM-DD",
    "totalAmount": 75.00,
    "isReconciled": false,
    "notes": "This is a note",
    "transactionType": "Transfer",
    "transferFromId": 4,
    "transferToId": 3,
    "TransferType": "Account",
  }
}
```

[↑ Back to top](#api-specification-doc)

---

### 41. Update a Payment Transaction

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
  "merchantSourceName": "Frys",
  "transactionSplits": [
    {
      "transactionId": 1,
      "envelopeId": 1
    },
    {
      "transactionId": 1,
      "amount": 500
    }
  ]
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

### 42. Update a Transfer Transaction

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

### 43. Soft Delete a Transaction

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

## 3. Common Error Codes

**Bad Request(400)**

```JSON
{
  "status": 400,
  "error": "Missing information for [resource] creation/update"
}
```

**Unauthorized(401)**

```JSON
{
  "status": 401,
  "error": "Unauthorized"
}
```

**Forbidden (403)**

```json
{
  "status": 403,
  "error": "[Resource] does not belong to the current user."
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

Tokens expire after 60 minutes. When a token expires, you'll receive a 401 Unauthorized response. Simply log in again to obtain a new token.

```

```
