-- ============================================
-- DROP & CREATE DATABASE
-- ============================================
DROP DATABASE IF EXISTS makes_cents;
CREATE DATABASE makes_cents;
USE makes_cents;

-- ============================================
-- ENUM TABLES  (unchanged)
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

-- Kept as a forecast-only regularity for the paycheck table below.
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

CREATE TABLE budget (
    budget_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_name VARCHAR(50) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

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

-- A named, reusable preset of envelope funding amounts (e.g. "Summer", "Holiday") that can be
-- applied to a budget month to bulk-create its funding queue. Persistent - not tied to any
-- specific month. Defined here (before budget_month) so budget_month's own FK to it can be
-- declared inline instead of needing a follow-up ALTER.
CREATE TABLE budget_template (
    budget_template_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    template_name VARCHAR(50) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_template_name (budget_id, template_name),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- One row per month a budget is actually used. This is also the scope
-- that envelope_funding (below) hangs off of - the whole-month funding
-- queue lives here, independent of any specific paycheck.
-- budget_template_id records which template (if any) was applied to this month's
-- funding queue - nullable, and SET NULL on template delete (deleting a template
-- shouldn't delete the months that used it, just forget the association).
CREATE TABLE budget_month (
    budget_month_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_id INT NOT NULL,
    budget_template_id INT NULL,
    month_id INT NOT NULL,
    year INT NOT NULL CHECK (year BETWEEN 1900 AND 2200),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_budget_month (budget_id, month_id, year),
    FOREIGN KEY (budget_id) REFERENCES budget(budget_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (budget_template_id) REFERENCES budget_template(budget_template_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (month_id) REFERENCES month_enum(month_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

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
-- ENVELOPES
-- ============================================

-- planned_amount removed: the "what's the target this month" number now
-- lives only on envelope_funding, since it's a month-scoped guideline,
-- not a permanent envelope attribute. remaining_amount stays stored here
-- (kept denormalized for read performance - it's read constantly, unlike
-- allocated_amount which only matters inside the funding flow).
CREATE TABLE sinking_fund_envelope (
    sinking_fund_envelope_id INT PRIMARY KEY AUTO_INCREMENT,
    envelope_category_id INT NOT NULL,
    envelope_name VARCHAR(50) NOT NULL,
    remaining_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    goal_amount DECIMAL(18,2) NULL CHECK (goal_amount > 0),
    goal_end_date DATE NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_sinking_envelope_name (envelope_category_id, envelope_name),
    FOREIGN KEY (envelope_category_id) REFERENCES envelope_category(envelope_category_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE rollover_envelope (
    rollover_envelope_id INT PRIMARY KEY AUTO_INCREMENT,
    envelope_category_id INT NOT NULL,
    envelope_name VARCHAR(50) NOT NULL,
    transfer_envelope_id INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_rollover_envelope_name (envelope_category_id, envelope_name),
    FOREIGN KEY (envelope_category_id) REFERENCES envelope_category(envelope_category_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- planned_amount removed for the same reason as sinking_fund_envelope.
-- remaining_amount stays. Absence of a row for a given
-- (rollover_envelope_id, budget_month_id) pair still means inactive
-- that month - normally created by applying a budget_template.
CREATE TABLE rollover_envelope_month (
    rollover_envelope_month_id INT PRIMARY KEY AUTO_INCREMENT,
    rollover_envelope_id INT NOT NULL,
    budget_month_id INT NOT NULL,
    remaining_amount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_rollover_month (rollover_envelope_id, budget_month_id),
    FOREIGN KEY (rollover_envelope_id) REFERENCES rollover_envelope(rollover_envelope_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (budget_month_id) REFERENCES budget_month(budget_month_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- One row = one envelope's planned funding amount and priority order for a specific budget
-- month. This is a plan only - it doesn't move money itself, the way a transaction does.
-- Two separate UNIQUE keys (rather than one combined one) prevent the same envelope from
-- getting two funding rows in the same month: sinking_fund_envelope_id and
-- rollover_envelope_month_id are each nullable, and MySQL unique indexes allow unlimited
-- NULLs, so each key only enforces uniqueness for the column it actually applies to.
-- Defined here (rather than in the TRANSACTIONS section) since it depends only on envelope
-- and budget_month tables, all already defined by this point.
CREATE TABLE envelope_funding (
    envelope_funding_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_month_id INT NOT NULL,
    sinking_fund_envelope_id INT NULL,
    rollover_envelope_month_id INT NULL,
    planned_amount DECIMAL(18,2) NOT NULL CHECK (planned_amount > 0),
    order_index INT NOT NULL CHECK (order_index > 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_envelope_funding_order (budget_month_id, order_index),
    UNIQUE KEY unique_envelope_funding_sinking (budget_month_id, sinking_fund_envelope_id),
    UNIQUE KEY unique_envelope_funding_rollover (budget_month_id, rollover_envelope_month_id),
    CHECK (
        (sinking_fund_envelope_id IS NOT NULL AND rollover_envelope_month_id IS NULL)
        OR
        (sinking_fund_envelope_id IS NULL AND rollover_envelope_month_id IS NOT NULL)
    ),
    FOREIGN KEY (budget_month_id) REFERENCES budget_month(budget_month_id) ON DELETE CASCADE ON UPDATE CASCADE,
    -- RESTRICT (not CASCADE/SET NULL) on these two: MySQL error 3823 forbids CASCADE/SET NULL
    -- on a column that's also part of a CHECK constraint.
    FOREIGN KEY (sinking_fund_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (rollover_envelope_month_id) REFERENCES rollover_envelope_month(rollover_envelope_month_id) ON DELETE RESTRICT ON UPDATE RESTRICT
);

-- One row = one envelope's planned inclusion (order + amount) in one template. References the
-- PERSISTENT rollover_envelope (not a month instance), since a template isn't tied to any
-- specific month - applying a template is what creates/updates the rollover_envelope_month row.
-- Two separate UNIQUE keys prevent the same envelope from appearing twice in one template, for
-- the same reason envelope_funding has the equivalent pair: the two envelope columns are each
-- nullable, and MySQL unique indexes allow unlimited NULLs, so each key only enforces
-- uniqueness for the column it actually applies to. Defined here (not right after
-- budget_template above) since it depends on sinking_fund_envelope/rollover_envelope, which
-- aren't defined until the ENVELOPES section.
CREATE TABLE budget_template_envelope (
    budget_template_envelope_id INT PRIMARY KEY AUTO_INCREMENT,
    budget_template_id INT NOT NULL,
    sinking_fund_envelope_id INT NULL,
    rollover_envelope_id INT NULL,
    planned_amount DECIMAL(18,2) NOT NULL CHECK (planned_amount > 0),
    order_index INT NOT NULL CHECK (order_index > 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_template_order (budget_template_id, order_index),
    UNIQUE KEY unique_template_envelope_sinking (budget_template_id, sinking_fund_envelope_id),
    UNIQUE KEY unique_template_envelope_rollover (budget_template_id, rollover_envelope_id),
    CHECK (
        (sinking_fund_envelope_id IS NOT NULL AND rollover_envelope_id IS NULL)
        OR
        (sinking_fund_envelope_id IS NULL AND rollover_envelope_id IS NOT NULL)
    ),
    FOREIGN KEY (budget_template_id) REFERENCES budget_template(budget_template_id) ON DELETE CASCADE ON UPDATE CASCADE,
    -- RESTRICT (not CASCADE/SET NULL) on these two: MySQL error 3823 forbids CASCADE/SET NULL
    -- on a column that's also part of a CHECK constraint.
    FOREIGN KEY (sinking_fund_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (rollover_envelope_id) REFERENCES rollover_envelope(rollover_envelope_id) ON DELETE RESTRICT ON UPDATE RESTRICT
);

-- ============================================
-- ACCOUNTS  (unchanged)
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

-- Forecast-only. No funding logic hangs off this table anymore - actual
-- income is entered as a transaction (Payment / Paycheck type) and
-- funds envelope_funding rows directly via transaction_split. This is
-- purely informational: "Job A pays ~biweekly, ~$X" for planning/visibility.
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
    transfer_transaction_type_id INT NOT NULL,
    transfer_from_account_id INT NULL,
    transfer_to_account_id INT NULL,
    CHECK (
        (transfer_transaction_type_id = 2 AND transfer_from_account_id IS NOT NULL AND transfer_to_account_id IS NOT NULL)
        OR
        (transfer_transaction_type_id = 3 AND transfer_from_account_id IS NULL AND transfer_to_account_id IS NULL)
    ),
    FOREIGN KEY (transaction_id) REFERENCES transaction(transaction_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (transfer_from_account_id) REFERENCES account(account_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (transfer_to_account_id) REFERENCES account(account_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (transfer_transaction_type_id) REFERENCES transfer_transaction_type_enum(transfer_transaction_type_enum_id) ON DELETE RESTRICT ON UPDATE RESTRICT
);

-- Used for normal multi-envelope payment splits, funding an
-- envelope_funding target (paycheck income), AND envelope-to-envelope
-- transfers (as a pair of opposite-signed rows).
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
    FOREIGN KEY (sinking_fund_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (rollover_envelope_month_id) REFERENCES rollover_envelope_month(rollover_envelope_month_id) ON DELETE RESTRICT ON UPDATE RESTRICT
);

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
    FOREIGN KEY (sinking_fund_envelope_id) REFERENCES sinking_fund_envelope(sinking_fund_envelope_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (rollover_envelope_id) REFERENCES rollover_envelope(rollover_envelope_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (planned_expense_regularity_id) REFERENCES planned_expense_regularity_enum(planned_expense_regularity_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (weekday_id) REFERENCES weekday_enum(weekday_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (occurrence_id) REFERENCES planned_expense_occurrence_enum(planned_expense_occurrence_enum_id) ON DELETE RESTRICT ON UPDATE CASCADE
);