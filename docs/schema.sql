-- schema.sql
-- Baseline schema derived from current domain entities.
-- Confirmed columns mirror domain model classes present in source.
-- Assumption markers are included where storage mapping is not explicit in code.

CREATE TABLE users (
    id CHAR(36) NOT NULL PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(500) NOT NULL,
    is_active TINYINT(1) NOT NULL DEFAULT 1,
    created_at_utc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UNIQUE KEY uq_users_email (email)
);

-- Assumption: roles are persisted in a separate table for User.Roles collection.
CREATE TABLE user_roles (
    user_id CHAR(36) NOT NULL,
    role_code INT NOT NULL,
    PRIMARY KEY (user_id, role_code),
    CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TABLE employees (
    id CHAR(36) NOT NULL PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    employee_code VARCHAR(50) NOT NULL,
    full_name VARCHAR(200) NOT NULL,
    department VARCHAR(100) NOT NULL,
    designation VARCHAR(100) NOT NULL,
    UNIQUE KEY uq_employees_user (user_id),
    UNIQUE KEY uq_employees_code (employee_code),
    CONSTRAINT fk_employees_user FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TABLE reporting_hierarchies (
    id CHAR(36) NOT NULL PRIMARY KEY,
    employee_id CHAR(36) NOT NULL,
    level1_approver_employee_id CHAR(36) NULL,
    level2_approver_employee_id CHAR(36) NULL,
    UNIQUE KEY uq_reporting_employee (employee_id),
    CONSTRAINT fk_reporting_employee FOREIGN KEY (employee_id) REFERENCES employees(id),
    CONSTRAINT fk_reporting_level1 FOREIGN KEY (level1_approver_employee_id) REFERENCES employees(id),
    CONSTRAINT fk_reporting_level2 FOREIGN KEY (level2_approver_employee_id) REFERENCES employees(id)
);

CREATE TABLE leave_requests (
    id CHAR(36) NOT NULL PRIMARY KEY,
    employee_id CHAR(36) NOT NULL,
    leave_type_code VARCHAR(50) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    days DECIMAL(5,2) NOT NULL,
    reason VARCHAR(1000) NOT NULL,
    status INT NOT NULL,
    created_at_utc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT fk_leave_requests_employee FOREIGN KEY (employee_id) REFERENCES employees(id)
);

CREATE TABLE leave_request_approvals (
    id CHAR(36) NOT NULL PRIMARY KEY,
    leave_request_id CHAR(36) NOT NULL,
    approval_level INT NOT NULL,
    approver_employee_id CHAR(36) NOT NULL,
    action VARCHAR(50) NOT NULL,
    comment VARCHAR(1000) NULL,
    actioned_at_utc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT fk_approval_request FOREIGN KEY (leave_request_id) REFERENCES leave_requests(id),
    CONSTRAINT fk_approval_approver FOREIGN KEY (approver_employee_id) REFERENCES employees(id)
);

CREATE TABLE leave_balances (
    id CHAR(36) NOT NULL PRIMARY KEY,
    employee_id CHAR(36) NOT NULL,
    leave_type_code VARCHAR(50) NOT NULL,
    opening_balance DECIMAL(8,2) NOT NULL,
    used DECIMAL(8,2) NOT NULL DEFAULT 0,
    adjustments DECIMAL(8,2) NOT NULL DEFAULT 0,
    UNIQUE KEY uq_leave_balance_employee_type (employee_id, leave_type_code),
    CONSTRAINT fk_leave_balance_employee FOREIGN KEY (employee_id) REFERENCES employees(id)
);
