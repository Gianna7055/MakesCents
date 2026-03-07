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

   **Planned Expenses**
   - [50. Create a Planned Expense](#50-create-a-planned-expense)
   - [51. Get All Planned Expenses For a Budget](#51-get-all-planned-expenses-for-a-budget)
   - [52. Get All Planned Expenses For an Envelope](#52-get-all-planned-expenses-for-an-envelope)
   - [53. Get All Planned Expenses For an Account](#53-get-all-planned-expenses-for-an-account)
   - [54. Get a Specific Planned Expense](#54-get-a-specific-planned-expense)
   - [55. Update a Planned Expense](#55-update-a-planned-expense)
   - [56. Delete a Planned Expense](#56-delete-a-planned-expense)

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
