-- ============================================
--  Makes Cents Database Schema (MySQL)
--  Includes abstract Account & Transaction tables
-- ============================================

-- ========== USER & BUDGET ==========
CREATE TABLE Budget (
    BudgetId INT AUTO_INCREMENT PRIMARY KEY,
    BudgetName VARCHAR(100) NOT NULL,
    Month INT NOT NULL,
    Year INT NOT NULL
) ENGINE=InnoDB;

CREATE TABLE User (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    HashedPassword VARCHAR(255) NOT NULL,
    IsDarkMode BOOLEAN DEFAULT FALSE,
    BudgetId INT,
    FOREIGN KEY (BudgetId) REFERENCES Budget(BudgetId)
        ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ========== ENVELOPE STRUCTURE ==========
CREATE TABLE EnvelopeCategory (
    EnvelopeCategoryId INT AUTO_INCREMENT PRIMARY KEY,
    BudgetId INT NOT NULL,
    EnvelopeCategoryName VARCHAR(100) NOT NULL,
    FOREIGN KEY (BudgetId) REFERENCES Budget(BudgetId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE Envelope (
    EnvelopeId INT AUTO_INCREMENT PRIMARY KEY,
    EnvelopeCategoryId INT NOT NULL,
    BudgetId INT NOT NULL,
    EnvelopeName VARCHAR(100) NOT NULL,
    PlannedAmount DECIMAL(10,2) DEFAULT 0.00,
    RemainingAmount DECIMAL(10,2) DEFAULT 0.00,
    IsSinkingFund BOOLEAN DEFAULT FALSE,
    GoalAmount DECIMAL(10,2),
    GoalEndDate DATE,
    TransferEnvelopeId INT,
    FOREIGN KEY (EnvelopeCategoryId) REFERENCES EnvelopeCategory(EnvelopeCategoryId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (BudgetId) REFERENCES Budget(BudgetId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ========== ABSTRACT BASE TABLES ==========
CREATE TABLE Account (
    AccountId INT AUTO_INCREMENT PRIMARY KEY,
    BudgetId INT NOT NULL,
    Institution VARCHAR(255),
    AccountName VARCHAR(100) NOT NULL,
    Balance DECIMAL(12,2) DEFAULT 0.00,
    AccountType INT NOT NULL, -- 1 = Bank, 2 = Debt, 3 = Investment
    FOREIGN KEY (BudgetId) REFERENCES Budget(BudgetId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB COMMENT='Abstract base table – do not insert directly.';

CREATE TABLE Transaction (
    TransactionId INT AUTO_INCREMENT PRIMARY KEY,
    AccountId INT NOT NULL,
    BudgetId INT NOT NULL,
    Date DATE NOT NULL,
    TotalAmount DECIMAL(12,2) NOT NULL,
    IsReconciled BOOLEAN DEFAULT FALSE,
    Notes VARCHAR(500),
    TransactionType INT NOT NULL, -- 1 = Payment, 2 = Transfer
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (BudgetId) REFERENCES Budget(BudgetId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB COMMENT='Abstract base table – do not insert directly.';

-- ========== ACCOUNT SUBCLASSES ==========
CREATE TABLE BankAccount (
    AccountId INT PRIMARY KEY,
    BankAccountType INT NOT NULL, -- 1 = Checking, 2 = Savings, 3 = MoneyMarket, 4 = Other
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE DebtAccount (
    AccountId INT PRIMARY KEY,
    DebtAccountType INT NOT NULL, -- 1 = CreditCard, 2 = Loan, 3 = Mortgage, 4 = Other
    AccountNumber VARCHAR(50) NOT NULL,
    DateOfNextBill DATE,
    AmountOfNextBill DECIMAL(10,2),
    PaymentRegularity INT, -- 1 = Weekly, 2 = Biweekly, 3 = Monthly, 4 = Quarterly, 5 = Annually
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE InvestmentAccount (
    AccountId INT PRIMARY KEY,
    InvestmentAccountType INT NOT NULL, -- 1 = Brokerage, 2 = Retirement, 3 = Crypto, 4 = Other
    AccountNumber VARCHAR(50) NOT NULL,
    IsTaxDeferred BOOLEAN DEFAULT FALSE,
    IsTaxExempt BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ========== PAYCHECKS & SPLITS ==========
CREATE TABLE Paycheck (
    PaycheckId INT AUTO_INCREMENT PRIMARY KEY,
    BudgetId INT NOT NULL,
    StartingDate DATE NOT NULL,
    SecondaryDate DATE,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Regularity INT, -- 1 = Weekly, 2 = Biweekly, 3 = Monthly, 4 = Custom
    FOREIGN KEY (BudgetId) REFERENCES Budget(BudgetId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE PaycheckSplit (
    PaycheckSplitId INT AUTO_INCREMENT PRIMARY KEY,
    PaycheckId INT NOT NULL,
    EnvelopeId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    OrderNumber INT,
    FOREIGN KEY (PaycheckId) REFERENCES Paycheck(PaycheckId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (EnvelopeId) REFERENCES Envelope(EnvelopeId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ========== TRANSACTION SUBCLASSES ==========
CREATE TABLE PaymentTransaction (
    TransactionId INT PRIMARY KEY,
    AccountId INT NOT NULL,
    PaymentType INT, -- 1 = ATM, 2 = Check, 3 = DebitCard, 4 = CreditCard, 5 = Deposit, 6 = Paycheck, 7 = Refund, 8 = LoanDeposit
    MerchantSourceName VARCHAR(255),
    CheckNumber VARCHAR(50),
    FOREIGN KEY (TransactionId) REFERENCES Transaction(TransactionId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE TransactionSplit (
    TransactionSplitId INT AUTO_INCREMENT PRIMARY KEY,
    TransactionId INT NOT NULL,
    EnvelopeId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (TransactionId) REFERENCES Transaction(TransactionId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (EnvelopeId) REFERENCES Envelope(EnvelopeId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE TransferTransaction (
    TransactionId INT PRIMARY KEY,
    FromId INT NOT NULL,
    ToId INT NOT NULL,
    TransferType INT, -- 1 = EnvelopeToEnvelope, 2 = AccountToAccount, 3 = AccountToEnvelope, 4 = EnvelopeToAccount
    FOREIGN KEY (TransactionId) REFERENCES Transaction(TransactionId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (FromId) REFERENCES Account(AccountId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (ToId) REFERENCES Account(AccountId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ========== PLANNED EXPENSES ==========
CREATE TABLE PlannedExpense (
    PlannedExpenseId INT AUTO_INCREMENT PRIMARY KEY,
    BudgetId INT NOT NULL,
    EnvelopeId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    DayOfMonth INT CHECK (DayOfMonth BETWEEN 1 AND 31),
    FOREIGN KEY (BudgetId) REFERENCES Budget(BudgetId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (EnvelopeId) REFERENCES Envelope(EnvelopeId)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;
