-- ============================================
-- DROP & CREATE DATABASE
-- ============================================
DROP DATABASE IF EXISTS makes_cents;
CREATE DATABASE makes_cents;
USE makes_cents;

-- ============================================
-- ENUM TABLES
-- ============================================

CREATE TABLE month_enum (
    month_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    month_name VARCHAR(50) NOT NULL
);

INSERT INTO month_enum (month_name) VALUES ('January'), ('February'), ('March'), ('April'), ('May'), ('June'), ('July'), ('August'), ('September'), ('October'), ('November'), ('December');

CREATE TABLE account_type_enum (
    account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO account_type_enum (account_type_name) VALUES ('Bank'), ('Debt'), ('Investment');

CREATE TABLE bank_account_type_enum (
    bank_account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    bank_account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO bank_account_type_enum (bank_account_type_name) VALUES ('Savings'), ('Checking');

CREATE TABLE debt_account_type_enum (
    debt_account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT, 
    debt_account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO debt_account_type_enum (debt_account_type_name) VALUES ('Credit Card'), ('Line of Credit / Loan'), ('Car Loan'), ('Mortgage');

CREATE TABLE debt_payment_regularity_enum (
    debt_payment_regularity_enum_id INT PRIMARY KEY AUTO_INCREMENT, 
    debt_payment_regularity_name VARCHAR(50) NOT NULL
);

INSERT INTO debt_payment_regularity_enum (debt_payment_regularity_name) VALUES ('Weekly'), ('Bi-Weekly (Every Two Weeks)'), ('Monthly'), ('Bi-Monthly (Every Two Months)'), ('Quarterly'), ('Twice-Annually'), ('Annually');

CREATE TABLE investment_account_type_enum (
    investment_account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT, 
    investment_account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO investment_account_type_enum (investment_account_type_name) VALUES ('IRA'), ('401K/403B'), ('Brokerage'), ('Other');

CREATE TABLE paycheck_regularity_enum (
    paycheck_regularity_enum_id INT PRIMARY KEY AUTO_INCREMENT, 
    paycheck_regularity_name VARCHAR(50) NOT NULL
);

INSERT INTO paycheck_regularity_enum (paycheck_regularity_name) VALUES ('Monthly'), ('Bi-Monthly (Twice a Month)'), ('Every Other Week'), ('Weekly');

CREATE TABLE transaction_type_enum (
    transaction_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    transaction_type_name VARCHAR(50) NOT NULL
);

INSERT INTO transaction_type_enum (transaction_type_name) VALUES ('Payment'), ('Transfer');

CREATE TABLE payment_transaction_type_enum (
    payment_transaction_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    payment_transaction_type_name VARCHAR(50) NOT NULL
);

INSERT INTO payment_transaction_type_enum (payment_transaction_type_name) VALUES ('ATM'), ('Check'), ('Debit Card'), ('Credit Card'), ('Deposit'), ('Paycheck'), ('Refund'), ('Loan Deposit');

CREATE TABLE transfer_transaction_type_enum (
    transfer_transaction_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    transfer_transaction_type_name VARCHAR(50) NOT NULL
);

INSERT INTO transfer_transaction_type_enum (transfer_transaction_type_name) VALUES ('Account'), ('Envelope');

CREATE TABLE planned_expense_regularity_enum (
    planned_expense_regularity_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    planned_expense_regularity_name VARCHAR(50) NOT NULL
);

INSERT INTO planned_expense_regularity_enum (planned_expense_regularity_name) VALUES ('Day of Month'), ('Last Day of Month'), ('Weekday Occurrence');

CREATE TABLE weekday_enum (
    weekday_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    weekday_name VARCHAR(50) NOT NULL
);

INSERT INTO weekday_enum (weekday_name) VALUES ('Monday'), ('Tuesday'), ('Wednesday'), ('Thursday'), ('Friday'), ('Saturday'), ('Sunday');

CREATE TABLE planned_expense_occurrence_enum (
    planned_expense_occurrence_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    planned_expense_occurrence_name VARCHAR(50) NOT NULL
);

INSERT INTO planned_expense_occurrence_enum (planned_expense_occurrence_name) VALUES ('First'), ('Second'), ('Third'), ('Fourth'), ('Last');

-- ============================================
-- REGULAR TABLES 1/2
-- ============================================

CREATE TABLE user (
    user_id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(30) NOT NULL UNIQUE,
    email VARCHAR(320) NOT NULL UNIQUE,
    password_hash VARCHAR(100) NOT NULL,
    is_dark_mode BOOLEAN DEFAULT FALSE,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE budget (
    budget_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    month_id INT NOT NULL,
    year INT NOT NULL CHECK (year BETWEEN 1900 AND 2200),
    budget_name VARCHAR(50) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_user_budget (user_id, month_id, year),
    FOREIGN KEY (user_id) REFERENCES user(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (month_id) REFERENCES month_enum(month_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE envelope_category (
    envelope_category_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    envelope_category_name VARCHAR(50) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_category_name (budget_id, envelope_category_name),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE envelope (
    envelope_id INT PRIMARY KEY AUTO_INCREMENT,
    envelope_category_id INT NOT NULL,
    envelope_name VARCHAR(50) NOT NULL,
    planned_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    remaining_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    is_sinking_fund BOOLEAN DEFAULT FALSE,
    goal_amount DECIMAL(18,2) NULL CHECK (goal_amount > 0),
    goal_end_date DATE NULL,
    transfer_envelope_id INT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_envelope_name (envelope_category_id, envelope_name),
    FOREIGN KEY (envelope_category_id) REFERENCES envelope_category(envelope_category_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_envelope_id) REFERENCES envelope(envelope_id) ON DELETE SET NULL ON UPDATE CASCADE
);

-- ============================================
-- BASE TABLES
-- ============================================

CREATE TABLE account (
    account_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    account_type_id INT NOT NULL,
    account_name VARCHAR(50) NOT NULL,
    institution VARCHAR(50) NOT NULL,
    balance DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (account_type_id) REFERENCES account_type_enum(account_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE transaction (
    transaction_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    transaction_date DATE NOT NULL,
    total_amount DECIMAL(18,2) NOT NULL CHECK (total_amount != 0),
    is_reconciled BOOLEAN DEFAULT FALSE,
    notes TEXT NULL,
    transaction_type_id INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    deleted_at DATETIME NULL,
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transaction_type_id) REFERENCES transaction_type_enum(transaction_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- ============================================
-- REGULAR TABLES 2/2
-- ============================================

CREATE TABLE bank_account (
    bank_account_id INT PRIMARY KEY AUTO_INCREMENT,
    account_id INT NOT NULL,
    bank_account_type_id INT NOT NULL,
    FOREIGN KEY (account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (bank_account_type_id) REFERENCES bank_account_type_enum(bank_account_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE debt_account (
    debt_account_id INT PRIMARY KEY AUTO_INCREMENT,
    account_id INT NOT NULL,
    debt_account_type_id INT NOT NULL,
    debt_account_number VARCHAR(50) NULL,
    date_of_next_bill DATE NULL,
    amount_of_next_bill DECIMAL(18,2) NULL,
    debt_payment_regularity_id INT NOT NULL,
    FOREIGN KEY (account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (debt_account_type_id) REFERENCES debt_account_type_enum(debt_account_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (debt_payment_regularity_id) REFERENCES debt_payment_regularity_enum(debt_payment_regularity_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE investment_account (
    investment_account_id INT PRIMARY KEY AUTO_INCREMENT,
    account_id INT NOT NULL,
    investment_account_type_id INT NOT NULL,
    investment_account_number VARCHAR(50) NULL,
    is_tax_deferred BOOLEAN DEFAULT FALSE,
    is_tax_exempt BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (investment_account_type_id) REFERENCES investment_account_type_enum(investment_account_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE paycheck (
    paycheck_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    starting_date DATE NOT NULL,
    secondary_date DATE NULL,
    total_amount DECIMAL(18,2) NOT NULL CHECK (total_amount > 0),
    paycheck_regularity_id INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (paycheck_regularity_id) REFERENCES paycheck_regularity_enum(paycheck_regularity_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE paycheck_split (
    paycheck_split_id INT PRIMARY KEY AUTO_INCREMENT,
    paycheck_id INT NOT NULL,
    envelope_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL CHECK (amount > 0),
    order_index INT NOT NULL CHECK (order_index > 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_paycheck_order (paycheck_id, order_index),
    FOREIGN KEY (paycheck_id) REFERENCES paycheck(paycheck_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (envelope_id) REFERENCES envelope(envelope_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE payment_transaction (
    payment_transaction_id INT PRIMARY KEY AUTO_INCREMENT,
    transaction_id INT NOT NULL,
    account_id INT NOT NULL,
    payment_transaction_type_id INT NOT NULL,
    merchant_source_name VARCHAR(50) NOT NULL,
    check_number INT NULL CHECK (check_number > 0),
    FOREIGN KEY (transaction_id) REFERENCES transaction(transaction_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (payment_transaction_type_id) REFERENCES payment_transaction_type_enum(payment_transaction_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE transfer_transaction (
    transfer_transaction_id INT PRIMARY KEY AUTO_INCREMENT,
    transaction_id INT NOT NULL,
    transfer_from_account_id INT NULL,
    transfer_to_account_id INT NULL,
    transfer_from_envelope_id INT NULL,
    transfer_to_envelope_id INT NULL,
    transfer_transaction_type_id INT NOT NULL,
    -- Enforce that transfers are either account-to-account OR envelope-to-envelope, never mixed
    CHECK (
        ((transfer_transaction_type_id = 1) AND
        (transfer_from_account_id IS NOT NULL AND transfer_to_account_id IS NOT NULL) AND
        (transfer_from_envelope_id IS NULL AND transfer_to_envelope_id IS NULL))
        OR
        ((transfer_transaction_type_id = 2) AND
        (transfer_from_envelope_id IS NOT NULL AND transfer_to_envelope_id IS NOT NULL) AND
        (transfer_from_account_id IS NULL AND transfer_to_account_id IS NULL))
    ),
    FOREIGN KEY (transaction_id) REFERENCES transaction(transaction_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_from_account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_to_account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_from_envelope_id) REFERENCES envelope(envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_to_envelope_id) REFERENCES envelope(envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_transaction_type_id) REFERENCES transfer_transaction_type_enum(transfer_transaction_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE transaction_split (
    transaction_split_id INT PRIMARY KEY AUTO_INCREMENT,
    transaction_id INT NOT NULL,
    envelope_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL CHECK (amount != 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (transaction_id) REFERENCES transaction(transaction_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (envelope_id) REFERENCES envelope(envelope_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE planned_expense (
    planned_expense_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    envelope_id INT NOT NULL,
    planned_expense_regularity_id INT NOT NULL,
    day_of_month INT NULL CHECK (day_of_month BETWEEN 1 AND 31),
    weekday_id INT NULL,
    occurrence_id INT NULL,
    amount DECIMAL(18,2) NOT NULL CHECK (amount > 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (envelope_id) REFERENCES envelope(envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (planned_expense_regularity_id) REFERENCES planned_expense_regularity_enum(planned_expense_regularity_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (weekday_id) REFERENCES weekday_enum(weekday_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (occurrence_id) REFERENCES planned_expense_occurrence_enum(planned_expense_occurrence_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);