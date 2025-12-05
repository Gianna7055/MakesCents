# API Specification Doc

**Makes Cents**

| Version | Date            | Author      | Description   |
| ------- | --------------- | ----------- | ------------- |
| 1.0     | 8 November 2024 | Gianna Ross | Initial draft |

---

## Table of Contents

1. [Methods](#methods)

   **User Management**

   - [1. Create a User](#1-create-a-user)
   - [2. Authenticate a User](#2-authenticate-a-user)
   - [3. Update a User](#3-update-a-user)
   - [4. Delete a User](#4-delete-a-user)

   **Budget Management**

   - [5. Create a Budget](#5-create-a-budget)
   - [6. Get a Budget](#6-get-a-budget)
   - [7. Update a Budget](#7-update-a-budget)
   - [8. Delete a Budget](#8-delete-a-budget)

   **Envelope Categories**

   - [9. Create an Envelope Category](#9-create-an-envelope-category)
   - [10. Update an Envelope Category](#10-update-an-envelope-category)
   - [11. Delete an Envelope Category](#11-delete-an-envelope-category)

   **Envelopes**

   - [12. Create an Envelope](#12-create-an-envelope)
   - [13. Update an Envelope](#13-update-an-envelope)
   - [14. Delete an Envelope](#14-delete-an-envelope)

   **Accounts**

   - [15. Create a Bank Account](#15-create-a-bank-account)
   - [16. Update a Bank Account](#16-update-a-bank-account)
   - [17. Delete a Bank Account](#17-delete-a-bank-account)
   - [18. Create a Debt Account](#18-create-a-debt-account)
   - [19. Update a Debt Account](#19-update-a-debt-account)
   - [20. Delete a Debt Account](#20-delete-a-debt-account)
   - [21. Create an Investment Account](#21-create-an-investment-account)
   - [22. Update an Investment Account](#22-update-an-investment-account)
   - [23. Delete an Investment Account](#23-delete-an-investment-account)

   **Paychecks**

   - [24. Create a Paycheck](#24-create-a-paycheck)
   - [25. Update a Paycheck](#25-update-a-paycheck)
   - [26. Delete a Paycheck](#26-delete-a-paycheck)
   - [27. Create a Paycheck Split](#27-create-a-paycheck-split)
   - [28. Update a Paycheck Split](#28-update-a-paycheck-split)
   - [29. Delete a Paycheck Split](#29-delete-a-paycheck-split)

   **Transactions**

   - [30. Create a Payment Transaction](#30-create-a-payment-transaction)
   - [31. Update a Payment Transaction](#31-update-a-payment-transaction)
   - [32. Delete a Payment Transaction](#32-delete-a-payment-transaction)
   - [33. Create a Transfer Transaction](#33-create-a-transfer-transaction)
   - [34. Update a Transfer Transaction](#34-update-a-transfer-transaction)
   - [35. Delete a Transfer Transaction](#35-delete-a-transfer-transaction)
   - [36. Create a Transaction Split](#36-create-a-transaction-split)
   - [37. Update a Transaction Split](#37-update-a-transaction-split)
   - [38. Delete a Transaction Split](#38-delete-a-transaction-split)

   **Planned Expenses**

   - [39. Create a Planned Expense](#39-create-a-planned-expense)
   - [40. Update a Planned Expense](#40-update-a-planned-expense)
   - [41. Delete a Planned Expense](#41-delete-a-planned-expense)

2. [Glossary](#glossary)
   - [Status Codes](#status-codes)

---

## 1. Create a User

**Description:**  
Create a new user

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| POST   | `/api/user/register` |

#### **Parameters**

| Type | Name           | Data Type | Description                      |
| ---- | -------------- | --------- | -------------------------------- |
| Body | `username`     | `string`  | The username for the new user    |
| Body | `email`        | `string`  | The email for the new user       |
| Body | `passwordHash` | `string`  | The hashed password for the user |

```JSON
{
  "username": "My Username",
  "email": "example@gmail.com",
  "passwordHash": "akjfhen48w9ruibfwle"
}
```

#### **Responses**

```JSON
{
  "status": 201,
  "userId": 1,
  "token": "udtrcgjvhhoiftu.3wetsrydtfygt76r85e7.786r5erudjgkgo"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 2. Authenticate a User

**Description:**  
Login a user

#### **Request**

| Method | URL               |
| ------ | ----------------- |
| POST   | `/api/user/login` |

#### **Parameters**

| Type | Name           | Data Type | Description                                    |
| ---- | -------------- | --------- | ---------------------------------------------- |
| Body | `username`     | `string`  | The username of the user logging in - nullable |
| Body | `email`        | `string`  | The email of the user logging in - nullable    |
| Body | `passwordHash` | `string`  | The hashed password of the user logging in     |

```JSON
{
  "username": null,
  "email": "example@gmail.com",
  "passwordHash": "akjfhen48w9ruibfwle"
}
```

#### **Responses**

```JSON
{
  "status": 201,
  "userId": 1,
  "token": "udtrcgjvhhoiftu.3wetsrydtfygt76r85e7.786r5erudjgkgo"
}
```

```JSON
{
  "status": 404,
  "error": "User not found."
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 3. Update a User

**Description:**  
Update an existing user

#### **Request**

| Method | URL         |
| ------ | ----------- |
| PUT    | `/api/user` |

#### **Parameters**

| Type   | Name             | Data Type | Description                                                      |
| ------ | ---------------- | --------- | ---------------------------------------------------------------- |
| Body   | `userId`         | `int`     | The Id of the user                                               |
| Body   | `username`       | `string`  | The updated username of the user - nullable                      |
| Body   | `email`          | `string`  | The updated email of the user - nullable                         |
| Body   | `hashedPassword` | `string`  | The updated hashed password of the user - nullable               |
| Body   | `isDarkMode`     | `boolean` | The updated value for the users appearance preference - nullable |
| Header | `token`          | `string ` | JWT authorization token                                          |

```JSON
{
  "userId": 1,
  "username": null,
  "email": "updatedEmail@gmail.com",
  "hashedPassword": null,
  "isDarkMode": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "userId": 1
}
```

```JSON
{
  "status": 404,
  "error": "User not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 4. Delete a User

**Description:**  
Delete an existing user

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

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "User not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 5. Create a Budget

**Description:**  
Create a new blank budget

#### **Request**

| Method | URL           |
| ------ | ------------- |
| POST   | `/api/budget` |

#### **Parameters**

| Type   | Name         | Data Type | Description                            |
| ------ | ------------ | --------- | -------------------------------------- |
| Body   | `userId`     | `int`     | The Id of the user logged in           |
| Body   | `month`      | `int`     | The number of the month for the budget |
| Body   | `year`       | `int`     | The year for the budget                |
| Body   | `budgetName` | `string`  | The name of the budget                 |
| Header | `token`      | `string ` | JWT authorization token                |

```JSON
{
  "userId": 1,
  "month": 11,
  "year": 2025,
  "budgetName": "My budget"
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "budgetId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 6. Get a Budget

**Description:**  
Get the initial data for a budget, including budget, envelope category, envelope, bank account, debt account, investment account, paycheck, paycheck split, transaction, transaction split, and planned expense information

#### **Request**

| Method | URL           |
| ------ | ------------- |
| GET    | `/api/budget` |

#### **Parameters**

| Type   | Name     | Data Type | Description                            |
| ------ | -------- | --------- | -------------------------------------- |
| Body   | `userId` | `int`     | The Id of the user logged in           |
| Body   | `month`  | `int`     | The number of the month for the budget |
| Body   | `year`   | `int`     | The year for the budget                |
| Header | `token`  | `string ` | JWT authorization token                |

```JSON
{
  "userId": 11,
  "month": 11,
  "year": 2025
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "budget": {
    "budgetId": 1,
    "userId": 11,
    "month": 11,
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
            "plannedAmount": 100.00,
            "remainingAmount": 54.99,
            "isSinkingFund": true,
            "goalAmount": 300.00,
            "endDateForGoal": "YYYY-MM-DD",
            "envelopeIdForTransfer": null
          },
          {
            "envelopeId": 4,
            "envelopeCategoryId": 2,
            "envelopeName": "Envelope 2",
            "plannedAmount": 500.00,
            "remainingAmount": 22.99,
            "isSinkingFund": false,
            "goalAmount": null,
            "endDateForGoal": null,
            "envelopeIdForTransfer": 3
          }
        ]
      }
    ],
    "accounts": [
      {
        "accountId": 8,
        "budgetId": 1,
        "accountName": "Bank Account 1",
        "institution": "Bank Institution",
        "balance": 1234.56,
        "accountType": "Bank",
        "bankAccountType": 2
      },
      {
        "accountId": 12,
        "budgetId": 1,
        "accountName": "Debt Account 1",
        "institution": "Debt Institution",
        "balance": 1234.56,
        "accountType": "Debt",
        "debtAccountType": 4,
        "accountNumber": 12345678,
        "dateOfNextBill": "YYYY-MM-DD",
        "amountOfNextBill": 123.45,
        "paymentRegularity": 3
      },
      {
        "accountId": 13,
        "budgetId": 1,
        "accountName": "Investment Account 1",
        "institution": "Investment Institution",
        "balance": 1234.56,
        "accountType": "Investment",
        "investmentAccountType": 2,
        "accountNumber": null,
        "isTaxDeferred": false,
        "isTaxExempt": true
      },
    ],
    "paychecks": [
      {
        "paycheckId": 9,
        "budgetId": 1,
        "startingDate": "YYYY-MM-DD",
        "secondaryDate": null,
        "amount": 8000.29,
        "regularity": 1,
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
    ],
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
            "amount": 7.00
          }
        ]
      },
      {
        "transactionId": 15,
        "budgetId": 1,
        "date": "YYYY-MM-DD",
        "totalAmount": 75.00,
        "isReconciled": false,
        "notes": "This is a note",
        "transactionType": "Transfer",
        "FromId": 4,
        "ToId": 3,
        "TransferType": 1,
      },
    ],
    "plannedExpenses": [
      {
        "plannedExpenseId": 14,
        "budgetId": 1,
        "envelopeId": 3,
        "dayOfMonth": 15,
        "amount": 160.00
      }
    ]
  }
}
```

```JSON
{
  "status": 404,
  "error": "Budget not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 7. Update a Budget

**Description:**  
Update an existing budget

#### **Request**

| Method | URL           |
| ------ | ------------- |
| PUT    | `/api/budget` |

#### **Parameters**

| Type   | Name       | Data Type | Description                                              |
| ------ | ---------- | --------- | -------------------------------------------------------- |
| Body   | `budgetId` | `int`     | The Id of the budget                                     |
| Body   | `userId`   | `int`     | The Id of the user associated with the budget - nullable |
| Body   | `month`    | `int`     | The number of the month for the budget - nullable        |
| Body   | `year`     | `int`     | The year for the budget - nullable                       |
| Body   | `name`     | `string`  | The name of the budget - nullable                        |
| Header | `token`    | `string ` | JWT authorization token                                  |

```JSON
{
  "budgetId": 2,
  "userId": null,
  "month": null,
  "year": null,
  "name": "Updated Budget"
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "budgetId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Budget not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 8. Delete a Budget

**Description:**  
Delete an existing budget

#### **Request**

| Method | URL                      |
| ------ | ------------------------ |
| DELETE | `/api/budget/{budgetId}` |

#### **Parameters**

| Type   | Name       | Data Type | Description             |
| ------ | ---------- | --------- | ----------------------- |
| Path   | `budgetId` | `int`     | The Id of the budget    |
| Header | `token`    | `string ` | JWT authorization token |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Budget not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 9. Create an Envelope Category

**Description:**  
Create a new envelope category

#### **Request**

| Method | URL                     |
| ------ | ----------------------- |
| POST   | `/api/envelopeCategory` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                                 |
| ------ | ---------------------- | --------- | ------------------------------------------- |
| Body   | `budgetId`             | `int`     | The budget id for the new envelope category |
| Body   | `envelopeCategoryName` | `string`  | The name for the envelope category          |
| Header | `token`                | `string ` | JWT authorization token                     |

```JSON
{
  "budgetId": 2,
  "envelopeCategoryName": "My Envelope Category"
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "envelopeCategoryId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 10. Update an Envelope Category

**Description:**  
Update an existing envelope category

#### **Request**

| Method | URL                     |
| ------ | ----------------------- |
| PUT    | `/api/envelopeCategory` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                                          |
| ------ | ---------------------- | --------- | ---------------------------------------------------- |
| Body   | `envelopeCategoryId`   | `int`     | The Id of the envelope category                      |
| Body   | `envelopeCategoryName` | `string`  | The updated name of the envelope category - nullable |
| Header | `token`                | `string ` | JWT authorization token                              |

```JSON
{
  "envelopeCategoryId": 1,
  "envelopeCategoryName": "New Category Name"
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "envelopeCategoryId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Envelope category not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 11. Delete an Envelope Category

**Description:**  
Delete an existing envelope category

#### **Request**

| Method | URL                                          |
| ------ | -------------------------------------------- |
| DELETE | `/api/envelopeCategory/{envelopeCategoryId}` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                     |
| ------ | -------------------- | --------- | ------------------------------- |
| Path   | `envelopeCategoryId` | `int`     | The Id of the envelope category |
| Header | `token`              | `string ` | JWT authorization token         |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Envelope category not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 12. Create an Envelope

**Description:**  
Create a new envelope

#### **Request**

| Method | URL             |
| ------ | --------------- |
| POST   | `/api/envelope` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                                                                            |
| ------ | -------------------- | --------- | ------------------------------------------------------------------------------------------------------ |
| Body   | `envelopeCategoryId` | `int`     | The envelope category id for the new envelope                                                          |
| Body   | `envelopeName`       | `string`  | The name for the envelope                                                                              |
| Body   | `plannedAmount`      | `decimal` | The planned amount for the envelope                                                                    |
| Body   | `remainingAmount`    | `decimal` | The remaining amount for the envelope                                                                  |
| Body   | `isSinkingFund`      | `boolean` | If the envelope is a sinking fun                                                                       |
| Body   | `goalAmount`         | `decimal` | The goal amount for the envelope if it is a sinking fund - nullable                                    |
| Body   | `goalEndDate`        | `Date`    | The end date for the goal if the envelope is a sinking fund - nullable                                 |
| Body   | `transferEnvelopeId` | `int`     | The id of the envelope to transfer remaining funds to if the envelope is not a sinking fund - nullable |
| Header | `token`              | `string ` | JWT authorization token                                                                                |

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

```JSON
{
  "status": 200,
  "envelopeId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 13. Update an Envelope

**Description:**  
Update an existing envelope

#### **Request**

| Method | URL             |
| ------ | --------------- |
| PUT    | `/api/envelope` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                                                                            |
| ------ | -------------------- | --------- | ------------------------------------------------------------------------------------------------------ |
| Body   | `envelopeId`         | `int`     | The Id of the envelope                                                                                 |
| Body   | `envelopeCategoryId` | `int`     | The envelope category id for the envelope - nullable                                                   |
| Body   | `envelopeName`       | `string`  | The name for the envelope - nullable                                                                   |
| Body   | `plannedAmount`      | `decimal` | The planned amount for the envelope - nullable                                                         |
| Body   | `remainingAmount`    | `decimal` | The remaining amount for the envelope - nullable                                                       |
| Body   | `isSinkingFund`      | `boolean` | If the envelope is a sinking fun - nullable                                                            |
| Body   | `goalAmount`         | `decimal` | The goal amount for the envelope if it is a sinking fund - nullable                                    |
| Body   | `goalEndDate`        | `Date`    | The end date for the goal if the envelope is a sinking fund - nullable                                 |
| Body   | `transferEnvelopeId` | `int`     | The id of the envelope to transfer remaining funds to if the envelope is not a sinking fund - nullable |
| Header | `token`              | `string ` | JWT authorization token                                                                                |

```JSON
{
  "envelopeId": 1,
  "envelopeCategoryId": null,
  "envelopeName": null,
  "plannedAmount": null,
  "remainingAmount": null,
  "isSinkingFund": false,
  "goalAmount": null,
  "goalEndDate": null,
  "transferEnvelopeId": 4
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "envelopeId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Envelope not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 14. Delete an Envelope

**Description:**  
Delete an existing envelope

#### **Request**

| Method | URL                          |
| ------ | ---------------------------- |
| DELETE | `/api/envelope/{envelopeId}` |

#### **Parameters**

| Type   | Name         | Data Type | Description             |
| ------ | ------------ | --------- | ----------------------- |
| Path   | `envelopeId` | `int`     | The Id of the envelope  |
| Header | `token`      | `string ` | JWT authorization token |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Envelope not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 15. Create a Bank Account

**Description:**  
Create a new bank account

#### **Request**

| Method | URL                |
| ------ | ------------------ |
| POST   | `/api/bankAccount` |

#### **Parameters**

| Type   | Name              | Data Type | Description                                    |
| ------ | ----------------- | --------- | ---------------------------------------------- |
| Body   | `budgetId`        | `int`     | The budget id for the new bank account         |
| Body   | `accountName`     | `string`  | The name of the bank account                   |
| Body   | `institution`     | `string`  | The institution of the bank account            |
| Body   | `balance`         | `decimal` | The balance of the bank account                |
| Body   | `bankAccountType` | `int`     | The int corresponding to the bank account type |
| Header | `token`           | `string ` | JWT authorization token                        |

```JSON
{
  "budgetId": 1,
  "accountName": "Bank Account 1",
  "institution": "Bank Institution",
  "balance": 12300.34,
  "bankAccountType": 1
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "bankAccountId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 16. Update a Bank Account

**Description:**  
Update an existing bank account

#### **Request**

| Method | URL                |
| ------ | ------------------ |
| PUT    | `/api/bankAccount` |

#### **Parameters**

| Type   | Name              | Data Type | Description                                               |
| ------ | ----------------- | --------- | --------------------------------------------------------- |
| Body   | `bankAccountId`   | `int`     | The Id of the bank account                                |
| Body   | `accountName`     | `string`  | The updated name of the bank account - nullable           |
| Body   | `institution`     | `string`  | The institution of the updated bank account - nullable    |
| Body   | `balance`         | `decimal` | The balance of the bank account - nullable                |
| Body   | `bankAccountType` | `int`     | The int corresponding to the bank account type - nullable |
| Header | `token`           | `string ` | JWT authorization token                                   |

```JSON
{
  "bankAccountId": 1,
  "accountName": "Updated Bank Account",
  "institution": null,
  "balance": null,
  "bankAccountType": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "bankAccountId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Bank account not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 17. Delete a Bank Account

**Description:**  
Delete an existing bank account

#### **Request**

| Method | URL                                |
| ------ | ---------------------------------- |
| DELETE | `/api/bankAccount/{bankAccountId}` |

#### **Parameters**

| Type   | Name            | Data Type | Description                |
| ------ | --------------- | --------- | -------------------------- |
| Path   | `bankAccountId` | `int`     | The Id of the bank account |
| Header | `token`         | `string ` | JWT authorization token    |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Bank account not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 18. Create a Debt Account

**Description:**  
Create a new debt account

#### **Request**

| Method | URL                |
| ------ | ------------------ |
| POST   | `/api/debtAccount` |

#### **Parameters**

| Type   | Name                | Data Type  | Description                                                                                             |
| ------ | ------------------- | ---------- | ------------------------------------------------------------------------------------------------------- |
| Body   | `budgetId`          | `int`      | The budget id for the new debt account                                                                  |
| Body   | `accountName`       | `string`   | The name of the debt account                                                                            |
| Body   | `institution`       | `string`   | The institution of the debt account                                                                     |
| Body   | `balance`           | `decimal`  | The balance of the debt account                                                                         |
| Body   | `debtAccountType`   | `int`      | The int corresponding to the debt account type                                                          |
| Body   | `accountNumber`     | `int`      | The account number of the debt account - nullable                                                       |
| Body   | `dateOfNextBill`    | `Date`     | The date the next bill is due                                                                           |
| Body   | `amountOfNextBill`  | `decimal ` | The amount of the next bill due                                                                         |
| Body   | `paymentRegularity` | `int`      | The int corresponding to the regularity of payment if "setup reoccurring payment" is checked - nullable |
| Header | `token`             | `string `  | JWT authorization token                                                                                 |

```JSON
{
  "budgetId": 1,
  "accountName": "Debt Account 1",
  "institution": "Debt Institution",
  "balance": 12300.34,
  "debtAccountType": 4,
  "accountNumber": null,
  "dateOfNextBill": "YYYY-MM-DD",
  "amountOfNextBill": 1234.56,
  "paymentRegularity": 4
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "debtAccountId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 19. Update a Debt Account

**Description:**  
Update an existing debt account

#### **Request**

| Method | URL                |
| ------ | ------------------ |
| PUT    | `/api/debtAccount` |

#### **Parameters**

| Type   | Name                | Data Type  | Description                                                   |
| ------ | ------------------- | ---------- | ------------------------------------------------------------- |
| Body   | `debtAccountId`     | `int`      | The Id of the debt account                                    |
| Body   | `accountName`       | `string`   | The updated name of the debt account - nullable               |
| Body   | `institution`       | `string`   | The institution of the updated debt account - nullable        |
| Body   | `balance`           | `decimal`  | The balance of the debt account - nullable                    |
| Body   | `debtAccountType`   | `int`      | The int corresponding to the debt account type - nullable     |
| Body   | `accountNumber`     | `int`      | The account number of the debt account - nullable - nullable  |
| Body   | `dateOfNextBill`    | `Date`     | The date the next bill is due - nullable                      |
| Body   | `amountOfNextBill`  | `decimal ` | The amount of the next bill due - nullable                    |
| Body   | `paymentRegularity` | `int`      | The int corresponding to the regularity of payment - nullable |
| Header | `token`             | `string `  | JWT authorization token                                       |

```JSON
{
  "debtAccountId": 1,
  "accountName": "Updated Debt Account",
  "institution": null,
  "balance": null,
  "debtAccountType": null,
  "accountNumber": null,
  "dateOfNextBill": null,
  "amountOfNextBill": 123.45,
  "paymentRegularity": 2
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "debtAccountId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Debt account not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 20. Delete a Debt Account

**Description:**  
Delete an existing debt account

#### **Request**

| Method | URL                                |
| ------ | ---------------------------------- |
| DELETE | `/api/debtAccount/{debtAccountId}` |

#### **Parameters**

| Type   | Name            | Data Type | Description                |
| ------ | --------------- | --------- | -------------------------- |
| Path   | `debtAccountId` | `int`     | The Id of the debt account |
| Header | `token`         | `string ` | JWT authorization token    |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Debt account not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 21. Create an Investment Account

**Description:**  
Create a new investment account

#### **Request**

| Method | URL                      |
| ------ | ------------------------ |
| POST   | `/api/investmentAccount` |

#### **Parameters**

| Type   | Name                    | Data Type  | Description                                             |
| ------ | ----------------------- | ---------- | ------------------------------------------------------- |
| Body   | `budgetId`              | `int`      | The budget id for the new investment account            |
| Body   | `accountName`           | `string`   | The name of the investment account                      |
| Body   | `institution`           | `string`   | The institution of the investment account               |
| Body   | `balance`               | `decimal`  | The balance of the investment account                   |
| Body   | `investmentAccountType` | `int`      | The int corresponding to the investment account type    |
| Body   | `accountNumber`         | `int`      | The account number of the investment account - nullable |
| Body   | `isTaxDeferred`         | `boolean`  | If the investment account is tax deferred               |
| Body   | `isTaxExempt`           | `boolean ` | If the investment account is tax exempt                 |
| Header | `token`                 | `string `  | JWT authorization token                                 |

```JSON
{
  "budgetId": 1,
  "accountName": "Investment Account 1",
  "institution": "Investment Institution",
  "balance": 12300.34,
  "debtAccountType": 2,
  "accountNumber": null,
  "isTaxDeferred": false,
  "isTaxExempt": true,
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "investmentAccountId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 22. Update an Investment Account

**Description:**  
Update an existing investment account

#### **Request**

| Method | URL                      |
| ------ | ------------------------ |
| PUT    | `/api/investmentAccount` |

#### **Parameters**

| Type   | Name                    | Data Type  | Description                                                     |
| ------ | ----------------------- | ---------- | --------------------------------------------------------------- |
| Body   | `investmentAccountId`   | `int`      | The Id of the investment account                                |
| Body   | `accountName`           | `string`   | The name of the investment account - nullable                   |
| Body   | `institution`           | `string`   | The institution of the investment account - nullable            |
| Body   | `balance`               | `decimal`  | The balance of the investment account - nullable                |
| Body   | `investmentAccountType` | `int`      | The int corresponding to the investment account type - nullable |
| Body   | `accountNumber`         | `int`      | The account number of the investment account - nullable         |
| Body   | `isTaxDeferred`         | `boolean`  | If the investment account is tax deferred - nullable            |
| Body   | `isTaxExempt`           | `boolean ` | If the investment account is tax exempt - nullable              |
| Header | `token`                 | `string `  | JWT authorization token                                         |

```JSON
{
  "investmentAccountId": 1,
  "accountName": "Updated Investment Account",
  "institution": null,
  "balance": null,
  "debtAccountType": null,
  "accountNumber": null,
  "isTaxDeferred": null,
  "isTaxExempt": null,
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "investmentAccountId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Investment account not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 23. Delete an Investment Account

**Description:**  
Delete an existing investment account

#### **Request**

| Method | URL                                            |
| ------ | ---------------------------------------------- |
| DELETE | `/api/investmentAccount/{investmentAccountId}` |

#### **Parameters**

| Type   | Name                  | Data Type | Description                      |
| ------ | --------------------- | --------- | -------------------------------- |
| Path   | `investmentAccountId` | `int`     | The Id of the investment account |
| Header | `token`               | `string ` | JWT authorization token          |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Investment account not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 24. Create a Paycheck

**Description:**  
Create a new paycheck

#### **Request**

| Method | URL             |
| ------ | --------------- |
| POST   | `/api/paycheck` |

#### **Parameters**

| Type   | Name            | Data Type | Description                                             |
| ------ | --------------- | --------- | ------------------------------------------------------- |
| Body   | `budgetId`      | `int`     | The budget id for the new paycheck                      |
| Body   | `startingDate`  | `Date`    | The starting date for the paycheck                      |
| Body   | `secondaryDate` | `Date`    | The secondary date for the paycheck                     |
| Body   | `totalAmount`   | `decimal` | The total amount of the paycheck                        |
| Body   | `regularity`    | `int`     | The int corresponding to the regularity of the paycheck |
| Header | `token`         | `string ` | JWT authorization token                                 |

```JSON
{
  "budgetId": 1,
  "startingDate": "YYYY-MM-DD",
  "secondaryDate": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "regularity": 2
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "paycheckId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 25. Update a Paycheck

**Description:**  
Update an existing paycheck

#### **Request**

| Method | URL             |
| ------ | --------------- |
| PUT    | `/api/paycheck` |

#### **Parameters**

| Type   | Name            | Data Type | Description                                                        |
| ------ | --------------- | --------- | ------------------------------------------------------------------ |
| Body   | `paycheckId`    | `int`     | The Id of the paycheck                                             |
| Body   | `startingDate`  | `Date`    | The starting date for the paycheck - nullable                      |
| Body   | `secondaryDate` | `Date`    | The secondary date for the paycheck - nullable                     |
| Body   | `totalAmount`   | `decimal` | The total amount of the paycheck - nullable                        |
| Body   | `regularity`    | `int`     | The int corresponding to the regularity of the paycheck - nullable |
| Header | `token`         | `string ` | JWT authorization token                                            |

```JSON
{
  "paycheckId": 1,
  "startingDate": "YYYY-MM-DD",
  "secondaryDate": null,
  "totalAmount": null,
  "regularity": 3
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "paycheckId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Paycheck not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 26. Delete a Paycheck

**Description:**  
Delete an existing paycheck

#### **Request**

| Method | URL                          |
| ------ | ---------------------------- |
| DELETE | `/api/paycheck/{paycheckId}` |

#### **Parameters**

| Type   | Name         | Data Type | Description             |
| ------ | ------------ | --------- | ----------------------- |
| Path   | `paycheckId` | `int`     | The Id of the paycheck  |
| Header | `token`      | `string ` | JWT authorization token |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Paycheck not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 27. Create a Paycheck Split

**Description:**  
Create a new paycheck split

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| POST   | `/api/paycheckSplit` |

#### **Parameters**

| Type   | Name          | Data Type | Description                                |
| ------ | ------------- | --------- | ------------------------------------------ |
| Body   | `paycheckId`  | `int`     | The paycheck id for the new paycheck split |
| Body   | `envelopeId`  | `int`     | The envelope id for the paycheck split     |
| Body   | `amount`      | `decimal` | The amount of the paycheck split           |
| Body   | `orderNumber` | `int`     | The order number for the paycheck split    |
| Header | `token`       | `string ` | JWT authorization token                    |

```JSON
{
  "paycheckId": 1,
  "envelopeId": 4,
  "amount": 234.56,
  "orderNumber": 2
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "paycheckSplitId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 28. Update a Paycheck Split

**Description:**  
Update an existing paycheck split

#### **Request**

| Method | URL                  |
| ------ | -------------------- |
| PUT    | `/api/paycheckSplit` |

#### **Parameters**

| Type   | Name              | Data Type | Description                                        |
| ------ | ----------------- | --------- | -------------------------------------------------- |
| Body   | `paycheckSplitId` | `int`     | The Id of the paycheck split                       |
| Body   | `envelopeId`      | `int`     | The envelope id for the paycheck split - nullable  |
| Body   | `amount`          | `decimal` | The amount of the paycheck - nullable              |
| Body   | `orderNumber`     | `int`     | The order number for the paycheck split - nullable |
| Header | `token`           | `string ` | JWT authorization token                            |

```JSON
{
  "paycheckSplitId": 1,
  "envelopeId": 4,
  "amount": null,
  "orderNumber": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "paycheckSplitId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Paycheck split not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 29. Delete a Paycheck Split

**Description:**  
Delete an existing paycheck split

#### **Request**

| Method | URL                                    |
| ------ | -------------------------------------- |
| DELETE | `/api/paycheckSplit/{paycheckSplitId}` |

#### **Parameters**

| Type   | Name              | Data Type | Description                  |
| ------ | ----------------- | --------- | ---------------------------- |
| Path   | `paycheckSplitId` | `int`     | The Id of the paycheck split |
| Header | `token`           | `string ` | JWT authorization token      |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Paycheck split not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 30. Create a Payment Transaction

**Description:**  
Create a new payment transaction

#### **Request**

| Method | URL                       |
| ------ | ------------------------- |
| POST   | `/api/paymentTransaction` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                                     |
| ------ | -------------------- | --------- | --------------------------------------------------------------- |
| Body   | `budgetId`           | `int`     | The budget id for the new payment transaction                   |
| Body   | `date`               | `Date`    | The date of the payment transaction                             |
| Body   | `totalAmount`        | `decimal` | The total amount of the payment transaction                     |
| Body   | `isReconciled`       | `boolean` | If the payment transaction has been reconciled                  |
| Body   | `notes`              | `string`  | Notes for the payment transaction - nullable                    |
| Body   | `accountId`          | `int`     | The account id for the new payment transaction                  |
| Body   | `paymentType`        | `int`     | The payment type for the new payment transaction                |
| Body   | `merchantSourceName` | `string`  | The name of the merchant/source for the new payment transaction |
| Body   | `checkNumber`        | `int`     | The check number for the new payment transaction - nullable     |
| Header | `token`              | `string ` | JWT authorization token                                         |

```JSON
{
  "budgetId": 1,
  "date": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "isReconciled": false,
  "notes": "A note for my payment transaction",
  "accountId": 3,
  "paymentType": 3,
  "merchantSourceName": "Walmart",
  "checkNumber": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "paymentTransactionId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 31. Update a Payment Transaction

**Description:**  
Update an existing payment transaction

#### **Request**

| Method | URL                       |
| ------ | ------------------------- |
| PUT    | `/api/paymentTransaction` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                                                                |
| ------ | ---------------------- | --------- | -------------------------------------------------------------------------- |
| Body   | `paymentTransactionId` | `int`     | The Id of the payment transaction                                          |
| Body   | `date`                 | `Date`    | The date of the payment transaction - nullable                             |
| Body   | `totalAmount`          | `decimal` | The total amount of the payment transaction - nullable                     |
| Body   | `isReconciled`         | `boolean` | If the payment transaction has been reconciled - nullable                  |
| Body   | `notes`                | `string`  | Notes for the payment transaction - nullable - nullable                    |
| Body   | `accountId`            | `int`     | The account id for the new payment transaction - nullable                  |
| Body   | `paymentType`          | `int`     | The payment type for the new payment transaction - nullable                |
| Body   | `merchantSourceName`   | `string`  | The name of the merchant/source for the new payment transaction - nullable |
| Body   | `checkNumber`          | `int`     | The check number for the new payment transaction - nullable                |
| Header | `token`                | `string ` | JWT authorization token                                                    |

```JSON
{
  "paymentTransactionId": 1,
  "date": "YYYY-MM-DD",
  "totalAmount": null,
  "isReconciled": null,
  "notes": null,
  "accountId": null,
  "paymentType": null,
  "merchantSourceName": "Frys",
  "checkNumber": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "paymentTransactionId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Payment Transaction not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 32. Delete a Payment Transaction

**Description:**  
Delete an existing payment transaction

#### **Request**

| Method | URL                                              |
| ------ | ------------------------------------------------ |
| DELETE | `/api/paymentTransaction/{paymentTransactionId}` |

#### **Parameters**

| Type   | Name                   | Data Type | Description                       |
| ------ | ---------------------- | --------- | --------------------------------- |
| Path   | `paymentTransactionId` | `int`     | The Id of the payment transaction |
| Header | `token`                | `string ` | JWT authorization token           |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Payment transaction not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 33. Create a Transfer Transaction

**Description:**  
Create a new transfer transaction

#### **Request**

| Method | URL                        |
| ------ | -------------------------- |
| POST   | `/api/transferTransaction` |

#### **Parameters**

| Type   | Name           | Data Type | Description                                                     |
| ------ | -------------- | --------- | --------------------------------------------------------------- |
| Body   | `budgetId`     | `int`     | The budget id for the new transfer transaction                  |
| Body   | `date`         | `Date`    | The date of the transfer transaction                            |
| Body   | `totalAmount`  | `decimal` | The total amount of the transfer transaction                    |
| Body   | `isReconciled` | `boolean` | If the transfer transaction has been reconciled                 |
| Body   | `notes`        | `string`  | Notes for the transfer transaction - nullable                   |
| Body   | `fromId`       | `int`     | The id for account or envelope the transfer transaction is from |
| Body   | `toId`         | `int`     | The id for account or envelope the transfer transaction is to   |
| Body   | `transferType` | `int`     | The transfer type for the new transfer transaction              |
| Header | `token`        | `string ` | JWT authorization token                                         |

```JSON
{
  "budgetId": 1,
  "date": "YYYY-MM-DD",
  "totalAmount": 1234.56,
  "isReconciled": false,
  "notes": "A note for my transfer transaction",
  "fromId": 3,
  "toId": 1,
  "transferType": 1
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "transferTransactionId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 34. Update a Transfer Transaction

**Description:**  
Update an existing transfer transaction

#### **Request**

| Method | URL                        |
| ------ | -------------------------- |
| PUT    | `/api/transferTransaction` |

#### **Parameters**

| Type   | Name                    | Data Type | Description                                                                |
| ------ | ----------------------- | --------- | -------------------------------------------------------------------------- |
| Body   | `transferTransactionId` | `int`     | The Id of the transfer transaction                                         |
| Body   | `date`                  | `Date`    | The date of the transfer transaction - nullable                            |
| Body   | `totalAmount`           | `decimal` | The total amount of the transfer transaction - nullable                    |
| Body   | `isReconciled`          | `boolean` | If the transfer transaction has been reconciled - nullable                 |
| Body   | `notes`                 | `string`  | Notes for the transfer transaction - nullable                              |
| Body   | `fromId`                | `int`     | The id for account or envelope the transfer transaction is from - nullable |
| Body   | `toId`                  | `int`     | The id for account or envelope the transfer transaction is to - nullable   |
| Body   | `transferType`          | `int`     | The transfer type for the new transfer transaction - nullable              |
| Header | `token`                 | `string ` | JWT authorization token                                                    |

```JSON
{
  "budgetId": 1,
  "date": null,
  "totalAmount": null,
  "isReconciled": null,
  "notes": null,
  "fromId": null,
  "toId": 5,
  "transferType": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "transferTransactionId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Transfer Transaction not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 35. Delete a Transfer Transaction

**Description:**  
Delete an existing transfer transaction

#### **Request**

| Method | URL                                                |
| ------ | -------------------------------------------------- |
| DELETE | `/api/transferTransaction/{transferTransactionId}` |

#### **Parameters**

| Type   | Name                    | Data Type | Description                        |
| ------ | ----------------------- | --------- | ---------------------------------- |
| Path   | `transferTransactionId` | `int`     | The Id of the transfer transaction |
| Header | `token`                 | `string ` | JWT authorization token            |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Transfer transaction not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 36. Create a Transaction Split

**Description:**  
Create a new transaction split

#### **Request**

| Method | URL                     |
| ------ | ----------------------- |
| POST   | `/api/transactionSplit` |

#### **Parameters**

| Type   | Name            | Data Type | Description                                      |
| ------ | --------------- | --------- | ------------------------------------------------ |
| Body   | `transactionId` | `int`     | The transaction id for the new transaction split |
| Body   | `envelopeId`    | `int`     | The envelope id for the transaction split        |
| Body   | `amount`        | `decimal` | The amount of the transaction split              |
| Header | `token`         | `string ` | JWT authorization token                          |

```JSON
{
  "transactionId": 1,
  "envelopeId": 4,
  "amount": 234.56
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "transactionSplitId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 37. Update a Transaction Split

**Description:**  
Update an existing transaction split

#### **Request**

| Method | URL                     |
| ------ | ----------------------- |
| PUT    | `/api/transactionSplit` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                                          |
| ------ | -------------------- | --------- | ---------------------------------------------------- |
| Body   | `transactionSplitId` | `int`     | The Id of the transaction split                      |
| Body   | `envelopeId`         | `int`     | The envelope id for the transaction split - nullable |
| Body   | `amount`             | `decimal` | The amount of the transaction - nullable             |
| Header | `token`              | `string ` | JWT authorization token                              |

```JSON
{
  "transactionSplitId": 1,
  "envelopeId": 2,
  "amount": null,
  "orderNumber": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "transactionSplitId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Transaction split not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 38. Delete a Transaction Split

**Description:**  
Delete an existing transaction split

#### **Request**

| Method | URL                                          |
| ------ | -------------------------------------------- |
| DELETE | `/api/transactionSplit/{transactionSplitId}` |

#### **Parameters**

| Type   | Name                 | Data Type | Description                     |
| ------ | -------------------- | --------- | ------------------------------- |
| Path   | `transactionSplitId` | `int`     | The Id of the transaction split |
| Header | `token`              | `string ` | JWT authorization token         |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Transaction split not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 39. Create a Planned Expense

**Description:**  
Create a new planned expense

#### **Request**

| Method | URL                   |
| ------ | --------------------- |
| POST   | `/api/plannedExpense` |

#### **Parameters**

| Type   | Name         | Data Type | Description                                  |
| ------ | ------------ | --------- | -------------------------------------------- |
| Body   | `budgetId`   | `int`     | The budget id for the new planned expense    |
| Body   | `envelopeId` | `int`     | The envelope id for the planned expense      |
| Body   | `dayOfMonth` | `int`     | The day of the month for the planned expense |
| Body   | `amount`     | `decimal` | The amount of the planned expense            |
| Header | `token`      | `string ` | JWT authorization token                      |

```JSON
{
  "budgetId": 1,
  "envelopeId": 2,
  "dayOfMonth": 10,
  "amount": 12.34
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "plannedExpenseId": 1
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 40. Update a Planned Expense

**Description:**  
Update an existing planned expense

#### **Request**

| Method | URL                   |
| ------ | --------------------- |
| PUT    | `/api/plannedExpense` |

#### **Parameters**

| Type   | Name               | Data Type | Description                                                |
| ------ | ------------------ | --------- | ---------------------------------------------------------- |
| Body   | `plannedExpenseId` | `int`     | The Id of the planned expense                              |
| Body   | `envelopeId`       | `int`     | The id for the envelope for the planned expense - nullable |
| Body   | `dayOfMonth`       | `int`     | The day of the month for the planned expense - nullable    |
| Body   | `amount`           | `decimal` | The amount of the planned expense - nullable               |
| Header | `token`            | `string ` | JWT authorization token                                    |

```JSON
{
  "plannedExpenseId": 1,
  "envelopeId": null,
  "dayOfMonth": 19,
  "amount": null
}
```

#### **Responses**

```JSON
{
  "status": 200,
  "plannedExpenseId": 1
}
```

```JSON
{
  "status": 404,
  "error": "Planned expense not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## 42. Delete a Planned Expense

**Description:**  
Delete an existing planned expense

#### **Request**

| Method | URL                                      |
| ------ | ---------------------------------------- |
| DELETE | `/api/plannedExpense/{plannedExpenseId}` |

#### **Parameters**

| Type   | Name               | Data Type | Description                   |
| ------ | ------------------ | --------- | ----------------------------- |
| Path   | `plannedExpenseId` | `int`     | The Id of the planned Expense |
| Header | `token`            | `string ` | JWT authorization token       |

#### **Responses**

```JSON
{
  "status": 200
}
```

```JSON
{
  "status": 404,
  "error": "Planned expense not found."
}
```

```JSON
{
  "status": 403,
  "error": "User not authenticated"
}
```

```JSON
{
  "status": 500,
  "error": "Something went wrong. Please try again later."
}
```

[↑ Back to top](#api-specification-doc)

---

## Glossary

### Status Codes

All status codes follow the standard HTTP conventions.

**Categories:**

- **2XX** – Success of some kind
- **4XX** – Error occurred on client’s part
- **5XX** – Error occurred on server’s part

| Status Code | Name                  | Description                                                   |
| ----------- | --------------------- | ------------------------------------------------------------- |
| 200 | OK | The request was successful. The response body contains the requested data. |
| 201 | Created | A new resource was successfully created. The response typically includes the new resource in the body and its URL in the Location header. |
| 202 | Accepted | The request has been accepted for processing but is not yet complete. Used for asynchronous operations. |
| 204 | No Content | The request was successful but there is no content to return. Commonly used for DELETE operations. |
| 400 | Bad Request | The request was malformed or contains invalid syntax. Check that all required parameters are included and properly formatted. |
| 401 | Unauthorized | Authentication credentials are missing, invalid, or expired. Verify your API key or access token. |
| 403 | Forbidden | The request is valid, but you don't have permission to access this resource. Check your account permissions. |
| 404 | Not Found | The requested resource does not exist. Verify the resource ID and endpoint URL. |
| 405 | Method Not Allowed | The HTTP method used is not supported for this endpoint. Check the allowed methods in the endpoint documentation. |
| 409 | Conflict | The request conflicts with the current state of the resource. This often occurs when trying to create a duplicate resource. |
| 422 | Unprocessable Entity | The request is well-formed but contains semantic errors. Check that field values meet validation requirements. |
| 429 | Too Many Requests | You have exceeded the rate limit. Wait before making additional requests or upgrade your plan. |
| 500 | Internal Server Error | An unexpected error occurred on the server. If this persists, contact support. |
| 502 | Bad Gateway | The server received an invalid response from an upstream server. This is usually temporary—try again. |
| 503 | Service Unavailable | The service is temporarily unavailable, often due to maintenance or high load. Retry your request after a short delay. |
| 504 | Gateway Timeout | The server did not receive a timely response from an upstream server. Retry your request. |

[↑ Back to top](#api-specification-doc)
