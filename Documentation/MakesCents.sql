-- Makes Cents Database Script v3

-- ============================================
-- DROP & CREATE DATABASE
-- ============================================
DROP DATABASE IF EXISTS makes_cents;
CREATE DATABASE makes_cents;
USE makes_cents;

-- ============================================
-- ENUM TABLES  (unchanged from v2)
-- ============================================

CREATE TABLE month_enum (
    month_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    month_name VARCHAR(50) NOT NULL
);

INSERT INTO month_enum (month_name) VALUES ('Unknown'), ('January'), ('February'), ('March'), ('April'), ('May'), ('June'), ('July'), ('August'), ('September'), ('October'), ('November'), ('December');

CREATE TABLE account_type_enum (
    account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO account_type_enum (account_type_name) VALUES ('Unknown'), ('Bank'), ('Debt'), ('Investment');

CREATE TABLE bank_account_type_enum (
    bank_account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    bank_account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO bank_account_type_enum (bank_account_type_name) VALUES ('Unknown'), ('Savings'), ('Checking');

CREATE TABLE debt_account_type_enum (
    debt_account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    debt_account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO debt_account_type_enum (debt_account_type_name) VALUES ('Unknown'), ('Credit Card'), ('Line of Credit / Loan'), ('Car Loan'), ('Mortgage');

CREATE TABLE debt_payment_regularity_enum (
    debt_payment_regularity_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    debt_payment_regularity_name VARCHAR(50) NOT NULL
);

INSERT INTO debt_payment_regularity_enum (debt_payment_regularity_name) VALUES ('Unknown'), ('Weekly'), ('Bi-Weekly (Every Two Weeks)'), ('Monthly'), ('Bi-Monthly (Every Two Months)'), ('Quarterly'), ('Twice-Annually'), ('Annually');

CREATE TABLE investment_account_type_enum (
    investment_account_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    investment_account_type_name VARCHAR(50) NOT NULL
);

INSERT INTO investment_account_type_enum (investment_account_type_name) VALUES ('Unknown'), ('IRA'), ('401K/403B'), ('Brokerage'), ('Other');

CREATE TABLE paycheck_regularity_enum (
    paycheck_regularity_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    paycheck_regularity_name VARCHAR(50) NOT NULL
);

INSERT INTO paycheck_regularity_enum (paycheck_regularity_name) VALUES ('Unknown'), ('Weekly'), ('Bi-Weekly (Every Two Weeks)'), ('Bi-Monthly (Twice a Month)'), ('Monthly');

CREATE TABLE transaction_type_enum (
    transaction_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    transaction_type_name VARCHAR(50) NOT NULL
);

INSERT INTO transaction_type_enum (transaction_type_name) VALUES ('Unknown'), ('Payment'), ('Transfer');

CREATE TABLE payment_transaction_type_enum (
    payment_transaction_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    payment_transaction_type_name VARCHAR(50) NOT NULL
);

INSERT INTO payment_transaction_type_enum (payment_transaction_type_name) VALUES ('Unknown'), ('ATM'), ('Check'), ('Debit Card'), ('Credit Card'), ('Deposit'), ('Paycheck'), ('Refund'), ('Loan Deposit');

CREATE TABLE transfer_transaction_type_enum (
    transfer_transaction_type_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    transfer_transaction_type_name VARCHAR(50) NOT NULL
);

-- NOTE: 'Unknown' occupies id 1, so 'Account' = 2 and 'Envelope' = 3.
-- v2's transfer_transaction CHECK referenced ids 1/2 (i.e. Unknown/Account) instead of 2/3 (Account/Envelope) - fixed below.
INSERT INTO transfer_transaction_type_enum (transfer_transaction_type_name) VALUES ('Unknown'), ('Account'), ('Envelope');

CREATE TABLE planned_expense_regularity_enum (
    planned_expense_regularity_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    planned_expense_regularity_name VARCHAR(50) NOT NULL
);

INSERT INTO planned_expense_regularity_enum (planned_expense_regularity_name) VALUES ('Unknown'), ('Day of Month'), ('Last Day of Month'), ('Weekday Occurrence');

CREATE TABLE weekday_enum (
    weekday_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    weekday_name VARCHAR(50) NOT NULL
);

INSERT INTO weekday_enum (weekday_name) VALUES ('Unknown'), ('Monday'), ('Tuesday'), ('Wednesday'), ('Thursday'), ('Friday'), ('Saturday'), ('Sunday');

CREATE TABLE planned_expense_occurrence_enum (
    planned_expense_occurrence_enum_id INT PRIMARY KEY AUTO_INCREMENT,
    planned_expense_occurrence_name VARCHAR(50) NOT NULL
);

INSERT INTO planned_expense_occurrence_enum (planned_expense_occurrence_name) VALUES ('Unknown'), ('First'), ('Second'), ('Third'), ('Fourth'), ('Last');

-- ============================================
-- CORE PERSISTENT ENTITIES
-- ============================================

CREATE TABLE user (
    user_id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(30) NOT NULL UNIQUE,
    email VARCHAR(320) NOT NULL UNIQUE,
    password_hash VARCHAR(100) NOT NULL,
    is_dark_mode BOOLEAN DEFAULT FALSE,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Budget is now a persistent entity: a household/plan, not a single month.
-- Month-specific data (year, month) lives on budget_month below.
CREATE TABLE budget (
    budget_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_name VARCHAR(50) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Many-to-many join between users and budgets. Surrogate PK to match the
-- existing DAO base class pattern. Role/permissions deferred for now.
CREATE TABLE budget_users (
    budget_users_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    user_id INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_budget_user (budget_id, user_id),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES user(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- One row per month a budget is actually used. Transactions and rollover
-- envelope balances live here since they reset/can vary month to month.
CREATE TABLE budget_month (
    budget_month_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    month_id INT NOT NULL,
    year INT NOT NULL CHECK (year BETWEEN 1900 AND 2200),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_budget_month (budget_id, month_id, year),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (month_id) REFERENCES month_enum(month_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Categories are persistent (tied to budget, not budget_month) so that
-- sinking fund envelopes - which must persist across months - have a
-- stable parent.
CREATE TABLE envelope_category (
    envelope_category_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    envelope_category_name VARCHAR(50) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_category_name (budget_id, envelope_category_name),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- ============================================
-- ENVELOPES  (replaces the single v2 `envelope` table)
-- ============================================

-- Sinking fund envelopes are persistent: one row, remaining_amount updates
-- in place, and it holds the fields specific to savings goals.
CREATE TABLE sinking_fund_envelope (
    sinking_fund_envelope_id INT PRIMARY KEY AUTO_INCREMENT,
    envelope_category_id INT NOT NULL,
    envelope_name VARCHAR(50) NOT NULL,
    planned_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    remaining_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    goal_amount DECIMAL(18,2) NULL CHECK (goal_amount > 0),
    goal_end_date DATE NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_sinking_envelope_name (envelope_category_id, envelope_name),
    FOREIGN KEY (envelope_category_id) REFERENCES envelope_category(envelope_category_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Rollover envelopes are just identity here - no dollar amounts. A given
-- rollover envelope may or may not be active in any given month; that's
-- expressed by whether a row exists for it in rollover_envelope_month.
CREATE TABLE rollover_envelope (
    rollover_envelope_id INT PRIMARY KEY AUTO_INCREMENT,
    envelope_category_id INT NOT NULL,
    envelope_name VARCHAR(50) NOT NULL,
    transfer_envelope_id INT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_rollover_envelope_name (envelope_category_id, envelope_name),
    FOREIGN KEY (envelope_category_id) REFERENCES envelope_category(envelope_category_id) ON DELETE CASCADE ON UPDATE CASCADE,
    -- A rollover envelope's default month-end sweep target must be a sinking fund.
    FOREIGN KEY (transfer_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE SET NULL ON UPDATE CASCADE
);

-- The monthly instance of a rollover envelope. planned_amount and
-- remaining_amount are set fresh each month; absence of a row for a given
-- (rollover_envelope_id, budget_month_id) pair means it's inactive that month.
CREATE TABLE rollover_envelope_month (
    rollover_envelope_month_id INT PRIMARY KEY AUTO_INCREMENT,
    rollover_envelope_id INT NOT NULL,
    budget_month_id INT NOT NULL,
    planned_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    remaining_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_rollover_month (rollover_envelope_id, budget_month_id),
    FOREIGN KEY (rollover_envelope_id) REFERENCES rollover_envelope(rollover_envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (budget_month_id) REFERENCES budget_month(budget_month_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- ============================================
-- ACCOUNTS  (persistent - unchanged in shape from v2, still tied to budget)
-- ============================================

CREATE TABLE account (
    account_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    account_type_id INT NOT NULL,
    account_name VARCHAR(50) NOT NULL,
    institution VARCHAR(50) NOT NULL,
    balance DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_account_name (budget_id, account_name),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (account_type_id) REFERENCES account_type_enum(account_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

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
    debt_payment_regularity_id INT NULL,
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

-- ============================================
-- TRANSACTIONS
-- ============================================

-- Now scoped to budget_month, since transactions happen within a specific month.
CREATE TABLE transaction (
    transaction_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_month_id INT NOT NULL,
    transaction_date DATE NOT NULL,
    total_amount DECIMAL(18,2) NOT NULL CHECK (total_amount != 0),
    is_reconciled BOOLEAN DEFAULT FALSE,
    notes TEXT NULL,
    transaction_type_id INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    deleted_at DATETIME NULL,
    FOREIGN KEY (budget_month_id) REFERENCES budget_month(budget_month_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transaction_type_id) REFERENCES transaction_type_enum(transaction_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Paycheck stays persistent (tied to budget) since it's a recurring
-- definition, like planned_expense, not a one-off monthly event.
CREATE TABLE paycheck (
    paycheck_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    paycheck_name VARCHAR(50) NOT NULL,
    starting_date DATE NOT NULL,
    secondary_date DATE NULL,
    total_amount DECIMAL(18,2) NOT NULL CHECK (total_amount > 0),
    paycheck_regularity_id INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (paycheck_regularity_id) REFERENCES paycheck_regularity_enum(paycheck_regularity_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Dual nullable FK: a split targets exactly one of a sinking fund envelope
-- or a specific month's rollover envelope instance.
CREATE TABLE paycheck_split (
    paycheck_split_id INT PRIMARY KEY AUTO_INCREMENT,
    paycheck_id INT NOT NULL,
    sinking_fund_envelope_id INT NULL,
    rollover_envelope_month_id INT NULL,
    amount DECIMAL(18,2) NOT NULL CHECK (amount > 0),
    order_index INT NOT NULL CHECK (order_index > 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_paycheck_order (paycheck_id, order_index),
    CHECK (
        (sinking_fund_envelope_id IS NOT NULL AND rollover_envelope_month_id IS NULL)
        OR
        (sinking_fund_envelope_id IS NULL AND rollover_envelope_month_id IS NOT NULL)
    ),
    FOREIGN KEY (paycheck_id) REFERENCES paycheck(paycheck_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (sinking_fund_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (rollover_envelope_month_id) REFERENCES rollover_envelope_month(rollover_envelope_month_id) ON DELETE CASCADE ON UPDATE CASCADE
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

-- Simplified: handles ONLY account-to-account transfers now. Envelope-to-envelope
-- movement (including overspending fixes / manual reallocation, any envelope to
-- any envelope) is expressed as two opposite-signed transaction_split rows instead -
-- see transaction_split below. This row still exists for Envelope-type transfers
-- as a type marker (account columns NULL) so `transaction_type_id = Transfer` always
-- has a corresponding transfer_transaction row you can join to for the subtype.
CREATE TABLE transfer_transaction (
    transfer_transaction_id INT PRIMARY KEY AUTO_INCREMENT,
    transaction_id INT NOT NULL,
    transfer_transaction_type_id INT NOT NULL,
    transfer_from_account_id INT NULL,
    transfer_to_account_id INT NULL,
    CHECK (
        (transfer_transaction_type_id = 2 AND transfer_from_account_id IS NOT NULL AND transfer_to_account_id IS NOT NULL)
        OR
        (transfer_transaction_type_id = 3 AND transfer_from_account_id IS NULL AND transfer_to_account_id IS NULL)
    ),
    FOREIGN KEY (transaction_id) REFERENCES transaction(transaction_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_from_account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_to_account_id) REFERENCES account(account_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_transaction_type_id) REFERENCES transfer_transaction_type_enum(transfer_transaction_type_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Dual nullable FK, same pattern as paycheck_split. Used for normal multi-envelope
-- payment splits AND for envelope-to-envelope transfers (as a pair of rows with
-- opposite-signed amounts summing to zero - enforced at the application layer,
-- same as the cross-table envelope name uniqueness check).
CREATE TABLE transaction_split (
    transaction_split_id INT PRIMARY KEY AUTO_INCREMENT,
    transaction_id INT NOT NULL,
    sinking_fund_envelope_id INT NULL,
    rollover_envelope_month_id INT NULL,
    amount DECIMAL(18,2) NOT NULL CHECK (amount != 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CHECK (
        (sinking_fund_envelope_id IS NOT NULL AND rollover_envelope_month_id IS NULL)
        OR
        (sinking_fund_envelope_id IS NULL AND rollover_envelope_month_id IS NOT NULL)
    ),
    FOREIGN KEY (transaction_id) REFERENCES transaction(transaction_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (sinking_fund_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (rollover_envelope_month_id) REFERENCES rollover_envelope_month(rollover_envelope_month_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Stays tied to budget (persistent) like paycheck. References the PERSISTENT
-- envelope definitions (not a specific month's rollover instance) so a
-- recurring planned expense stays valid regardless of which future months
-- actually end up activating that rollover envelope.
CREATE TABLE planned_expense (
    planned_expense_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    sinking_fund_envelope_id INT NULL,
    rollover_envelope_id INT NULL,
    planned_expense_regularity_id INT NOT NULL,
    day_of_month INT NULL CHECK (day_of_month BETWEEN 1 AND 31),
    weekday_id INT NULL,
    occurrence_id INT NULL,
    amount DECIMAL(18,2) NOT NULL CHECK (amount > 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CHECK (
        (sinking_fund_envelope_id IS NOT NULL AND rollover_envelope_id IS NULL)
        OR
        (sinking_fund_envelope_id IS NULL AND rollover_envelope_id IS NOT NULL)
    ),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (sinking_fund_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (rollover_envelope_id) REFERENCES rollover_envelope(rollover_envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (planned_expense_regularity_id) REFERENCES planned_expense_regularity_enum(planned_expense_regularity_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (weekday_id) REFERENCES weekday_enum(weekday_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (occurrence_id) REFERENCES planned_expense_occurrence_enum(planned_expense_occurrence_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);