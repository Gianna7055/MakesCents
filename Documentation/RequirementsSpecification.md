# Makes Cents - Requirements Specification

---

## Table of Contents

1. [Functional Requirements](#functional-requirements)
2. [Non-Functional Requirements](#non-functional-requirements)
3. [Technical Specifications](#technical-specifications)
4. [Traceability Matrix](#traceability-matrix)

---

## Functional Requirements

| ID     | Description                                                                                                                                                                                       | Inputs                                                  | Behavior                        | Output       | Derived From |
| ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------- | ------------------------------- | ------------ | ------------ |
| FR-001 | The system shall allow the user to create a new bank account by entering required details such as account name, account type (e.g., checking, savings), bank name, and optional starting balance. | Account name, account type, bank name, starting balance | Create a new bank account       | Success flag | SCRUM-1      |
| FR-002 | The system shall validate that all required fields are provided and that the starting balance is a valid numeric value before saving a bank account.                                              |                                                         |                                 |              |              |
| FR-003 | The system shall allow the user to edit and update an existing bank account. The following fields will be updatable: account name, type, and bank name.                                           | All nullable: account name, type, bank name             | Update an existing bank account | Success flag | SCRUM-2      |

---

## Non-Functional Requirements

| ID      | Testable Requirement                                                                                                                       | Derived From |
| ------- | ------------------------------------------------------------------------------------------------------------------------------------------ | ------------ |
| NFR-001 | The bank account setup process shall be intuitive and require no more than three steps to complete.                                        | FR-001       |
| NFR-002 | The bank account update process will be user friendly, making sure it is clear which fields are editable and showing the current contents. | FR-002       |

---

## Technical Specifications

| ID     | Description                                                    | Acceptance Criteria | Derived From |
| ------ | -------------------------------------------------------------- | ------------------- | ------------ |
| TS-001 | BankAccountModel (Id, BudgetId, AccountName) [See UMLs in xyz] |                     | SCRUM-1      |

---

## Traceability Matrix

### 1. SCRUM-1

**As a user, I want to be able to set up a bank account so that I can associate my transactions with my bank accounts.**

- **FR-001** - The system shall allow the user to create a new bank account by entering required details such as account name, account type (e.g., checking, savings), bank name, and optional starting balance.
- **FR-002** - The system shall validate that all required fields are provided and that the starting balance is a valid numeric value before saving a bank account.
- **NFR-001** - The bank account setup process shall be intuitive and require no more than three steps to complete.

### 2. SCRUM-2

**As a user, I want to be able to delete a bank account so that I can remove bank accounts from my app if I get rid of them.**

- **FR-003** - The system shall allow the user to edit and update an existing bank account. The following fields will be updatable: account name, type, and bank name.
- **NFR-002** - The bank account update process will be user friendly, making sure it is clear which fields are editable and showing the current contents.
