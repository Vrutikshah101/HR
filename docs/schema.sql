from pathlib import Path

sql = r'''-- =========================================================
-- Leave Management System - MySQL 8.x Schema
-- Supports:
--   * Users, Roles, Departments, Designations
--   * Configurable 2-level approval hierarchy
--     (Reviewing Officer -> Re-Reviewing Officer)
--   * Optional HR step
--   * Leave policy, balances, ledger
--   * Workflow, approval trail, activity logs
--   * Comments, attachments, notifications
--   * Holidays, weekends, delegation
-- =========================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS leave_cancellation_requests;
DROP TABLE IF EXISTS approval_delegations;
DROP TABLE IF EXISTS user_weekend_policy;
DROP TABLE IF EXISTS weekend_policies;
DROP TABLE IF EXISTS holidays;
DROP TABLE IF EXISTS notifications;
DROP TABLE IF EXISTS leave_attachments;
DROP TABLE IF EXISTS leave_comments;
DROP TABLE IF EXISTS activity_logs;
DROP TABLE IF EXISTS leave_approval_actions;
DROP TABLE IF EXISTS leave_request_workflow_steps;
DROP TABLE IF EXISTS leave_request_workflow;
DROP TABLE IF EXISTS workflow_steps;
DROP TABLE IF EXISTS workflow_definitions;
DROP TABLE IF EXISTS leave_request_days;
DROP TABLE IF EXISTS leave_requests;
DROP TABLE IF EXISTS leave_balance_transactions;
DROP TABLE IF EXISTS leave_balances;
DROP TABLE IF EXISTS user_leave_policy;
DROP TABLE IF EXISTS leave_policy_details;
DROP TABLE IF EXISTS leave_policies;
DROP TABLE IF EXISTS leave_types;
DROP TABLE IF EXISTS user_approval_hierarchy;
DROP TABLE IF EXISTS user_roles;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS departments;
DROP TABLE IF EXISTS designations;

SET FOREIGN_KEY_CHECKS = 1;

-- =========================================================
-- MASTER TABLES
-- =========================================================

CREATE TABLE departments (
    department_id            BIGINT PRIMARY KEY AUTO_INCREMENT,
    department_code          VARCHAR(50) NOT NULL UNIQUE,
    department_name          VARCHAR(100) NOT NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE designations (
    designation_id           BIGINT PRIMARY KEY AUTO_INCREMENT,
    designation_code         VARCHAR(50) NOT NULL UNIQUE,
    designation_name         VARCHAR(100) NOT NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE department_designation_maps (
    map_id                    BIGINT PRIMARY KEY AUTO_INCREMENT,
    department_id             BIGINT NOT NULL,
    designation_id            BIGINT NOT NULL,
    created_at                DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_dept_desig_map UNIQUE (department_id, designation_id),
    CONSTRAINT fk_ddm_department
        FOREIGN KEY (department_id) REFERENCES departments(department_id),
    CONSTRAINT fk_ddm_designation
        FOREIGN KEY (designation_id) REFERENCES designations(designation_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE users (
    user_id                  BIGINT PRIMARY KEY AUTO_INCREMENT,
    employee_code            VARCHAR(50) NOT NULL UNIQUE,
    first_name               VARCHAR(100) NOT NULL,
    last_name                VARCHAR(100) NULL,
    email                    VARCHAR(150) NOT NULL UNIQUE,
    phone                    VARCHAR(20) NULL,
    password_hash            VARCHAR(255) NOT NULL,
    department_id            BIGINT NULL,
    designation_id           BIGINT NULL,
    join_date                DATE NOT NULL,
    employment_status        ENUM('ACTIVE','INACTIVE','RESIGNED','TERMINATED','ON_HOLD') NOT NULL DEFAULT 'ACTIVE',
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_users_department
        FOREIGN KEY (department_id) REFERENCES departments(department_id),
    CONSTRAINT fk_users_designation
        FOREIGN KEY (designation_id) REFERENCES designations(designation_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE roles (
    role_id                  BIGINT PRIMARY KEY AUTO_INCREMENT,
    role_code                VARCHAR(50) NOT NULL UNIQUE,
    role_name                VARCHAR(100) NOT NULL,
    description              VARCHAR(255) NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE user_roles (
    user_role_id             BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id                  BIGINT NOT NULL,
    role_id                  BIGINT NOT NULL,
    assigned_at              DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    assigned_by              BIGINT NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_user_role UNIQUE (user_id, role_id),
    CONSTRAINT fk_user_roles_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_user_roles_role
        FOREIGN KEY (role_id) REFERENCES roles(role_id),
    CONSTRAINT fk_user_roles_assigned_by
        FOREIGN KEY (assigned_by) REFERENCES users(user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- APPROVAL HIERARCHY
-- Configurable 2-level hierarchy:
--   reviewing_officer_id
--   rereviewing_officer_id
-- Optional:
--   hr_officer_id
-- =========================================================

CREATE TABLE user_approval_hierarchy (
    hierarchy_id             BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id                  BIGINT NOT NULL,
    reviewing_officer_id     BIGINT NULL,
    rereviewing_officer_id   BIGINT NULL,
    hr_officer_id            BIGINT NULL,
    effective_from           DATE NOT NULL,
    effective_to             DATE NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_user_hierarchy_period UNIQUE (user_id, effective_from),
    CONSTRAINT fk_uah_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_uah_reviewing_officer
        FOREIGN KEY (reviewing_officer_id) REFERENCES users(user_id),
    CONSTRAINT fk_uah_rereviewing_officer
        FOREIGN KEY (rereviewing_officer_id) REFERENCES users(user_id),
    CONSTRAINT fk_uah_hr_officer
        FOREIGN KEY (hr_officer_id) REFERENCES users(user_id),
    CONSTRAINT chk_uah_no_self_review_1 CHECK (user_id <> reviewing_officer_id),
    CONSTRAINT chk_uah_no_self_review_2 CHECK (user_id <> rereviewing_officer_id),
    CONSTRAINT chk_uah_no_self_hr CHECK (user_id <> hr_officer_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- LEAVE CONFIGURATION
-- =========================================================

CREATE TABLE leave_types (
    leave_type_id            BIGINT PRIMARY KEY AUTO_INCREMENT,
    leave_code               VARCHAR(50) NOT NULL UNIQUE,
    leave_name               VARCHAR(100) NOT NULL,
    description              VARCHAR(255) NULL,
    requires_attachment      BOOLEAN NOT NULL DEFAULT FALSE,
    is_paid                  BOOLEAN NOT NULL DEFAULT TRUE,
    is_half_day_allowed      BOOLEAN NOT NULL DEFAULT TRUE,
    is_backdated_allowed     BOOLEAN NOT NULL DEFAULT FALSE,
    max_days_per_request     DECIMAL(6,2) NULL,
    gender_applicability     ENUM('ALL','MALE','FEMALE','OTHER') NOT NULL DEFAULT 'ALL',
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_policies (
    policy_id                BIGINT PRIMARY KEY AUTO_INCREMENT,
    policy_code              VARCHAR(50) NOT NULL UNIQUE,
    policy_name              VARCHAR(150) NOT NULL,
    description              VARCHAR(255) NULL,
    effective_from           DATE NOT NULL,
    effective_to             DATE NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_policy_details (
    policy_detail_id         BIGINT PRIMARY KEY AUTO_INCREMENT,
    policy_id                BIGINT NOT NULL,
    leave_type_id            BIGINT NOT NULL,
    annual_quota             DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    monthly_accrual          DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    carry_forward_limit      DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    encashment_allowed       BOOLEAN NOT NULL DEFAULT FALSE,
    max_consecutive_days     DECIMAL(6,2) NULL,
    min_notice_days          INT NOT NULL DEFAULT 0,
    document_mandatory_after_days DECIMAL(6,2) NULL,
    sandwich_rule_applicable BOOLEAN NOT NULL DEFAULT FALSE,
    probation_allowed        BOOLEAN NOT NULL DEFAULT TRUE,
    gender_applicability     ENUM('ALL','MALE','FEMALE','OTHER') NOT NULL DEFAULT 'ALL',
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_policy_leave_type UNIQUE (policy_id, leave_type_id),
    CONSTRAINT fk_lpd_policy
        FOREIGN KEY (policy_id) REFERENCES leave_policies(policy_id),
    CONSTRAINT fk_lpd_leave_type
        FOREIGN KEY (leave_type_id) REFERENCES leave_types(leave_type_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE user_leave_policy (
    user_leave_policy_id     BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id                  BIGINT NOT NULL,
    policy_id                BIGINT NOT NULL,
    effective_from           DATE NOT NULL,
    effective_to             DATE NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_user_policy_period UNIQUE (user_id, policy_id, effective_from),
    CONSTRAINT fk_ulp_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_ulp_policy
        FOREIGN KEY (policy_id) REFERENCES leave_policies(policy_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- LEAVE BALANCE
-- =========================================================

CREATE TABLE leave_balances (
    leave_balance_id         BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id                  BIGINT NOT NULL,
    leave_type_id            BIGINT NOT NULL,
    balance_year             INT NOT NULL,
    opening_balance          DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    credited                 DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    debited                  DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    adjusted                 DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    closing_balance          DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_user_leave_balance_year UNIQUE (user_id, leave_type_id, balance_year),
    CONSTRAINT fk_lb_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_lb_leave_type
        FOREIGN KEY (leave_type_id) REFERENCES leave_types(leave_type_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_requests (
    leave_request_id         BIGINT PRIMARY KEY AUTO_INCREMENT,
    request_code             VARCHAR(50) NOT NULL UNIQUE,
    user_id                  BIGINT NOT NULL,
    leave_type_id            BIGINT NOT NULL,
    start_date               DATE NOT NULL,
    end_date                 DATE NOT NULL,
    start_session            ENUM('FULL','FIRST_HALF','SECOND_HALF') NOT NULL DEFAULT 'FULL',
    end_session              ENUM('FULL','FIRST_HALF','SECOND_HALF') NOT NULL DEFAULT 'FULL',
    total_days               DECIMAL(6,2) NOT NULL,
    reason                   TEXT NULL,
    contact_during_leave     VARCHAR(150) NULL,
    emergency_contact        VARCHAR(20) NULL,
    current_status           ENUM(
                                'DRAFT',
                                'SUBMITTED',
                                'PENDING_REVIEWING_OFFICER',
                                'PENDING_REREVIEWING_OFFICER',
                                'PENDING_HR',
                                'APPROVED',
                                'REJECTED',
                                'CANCELLED',
                                'WITHDRAWN',
                                'RETURNED'
                             ) NOT NULL DEFAULT 'DRAFT',
    workflow_status          VARCHAR(50) NULL,
    applied_at               DATETIME NULL,
    approved_at              DATETIME NULL,
    rejected_at              DATETIME NULL,
    cancelled_at             DATETIME NULL,
    withdrawn_at             DATETIME NULL,
    returned_at              DATETIME NULL,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_lr_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_lr_leave_type
        FOREIGN KEY (leave_type_id) REFERENCES leave_types(leave_type_id),
    CONSTRAINT chk_leave_request_dates CHECK (end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_balance_transactions (
    transaction_id           BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id                  BIGINT NOT NULL,
    leave_type_id            BIGINT NOT NULL,
    leave_request_id         BIGINT NULL,
    transaction_type         ENUM('OPENING','CREDIT','DEBIT','REVERSAL','ADJUSTMENT','CARRY_FORWARD','ENCASHMENT') NOT NULL,
    quantity                 DECIMAL(6,2) NOT NULL,
    balance_before           DECIMAL(6,2) NOT NULL,
    balance_after            DECIMAL(6,2) NOT NULL,
    remarks                  VARCHAR(255) NULL,
    created_by               BIGINT NULL,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_lbt_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_lbt_leave_type
        FOREIGN KEY (leave_type_id) REFERENCES leave_types(leave_type_id),
    CONSTRAINT fk_lbt_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id),
    CONSTRAINT fk_lbt_created_by
        FOREIGN KEY (created_by) REFERENCES users(user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_request_days (
    leave_request_day_id     BIGINT PRIMARY KEY AUTO_INCREMENT,
    leave_request_id         BIGINT NOT NULL,
    leave_date               DATE NOT NULL,
    day_fraction             DECIMAL(3,2) NOT NULL DEFAULT 1.00,
    session_type             ENUM('FULL','FIRST_HALF','SECOND_HALF') NOT NULL DEFAULT 'FULL',
    status                   ENUM('REQUESTED','APPROVED','REJECTED','CANCELLED') NOT NULL DEFAULT 'REQUESTED',
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_leave_request_day UNIQUE (leave_request_id, leave_date),
    CONSTRAINT fk_lrd_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id)
        ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- CONFIGURABLE WORKFLOW
-- =========================================================

CREATE TABLE workflow_definitions (
    workflow_id              BIGINT PRIMARY KEY AUTO_INCREMENT,
    workflow_code            VARCHAR(50) NOT NULL UNIQUE,
    workflow_name            VARCHAR(100) NOT NULL,
    entity_type              VARCHAR(50) NOT NULL,
    description              VARCHAR(255) NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE workflow_steps (
    workflow_step_id         BIGINT PRIMARY KEY AUTO_INCREMENT,
    workflow_id              BIGINT NOT NULL,
    step_no                  INT NOT NULL,
    step_name                VARCHAR(100) NOT NULL,
    approver_type            ENUM(
                                'REVIEWING_OFFICER',
                                'REREVIEWING_OFFICER',
                                'HR',
                                'ADMIN',
                                'SPECIFIC_USER',
                                'ROLE_BASED'
                             ) NOT NULL,
    specific_user_id         BIGINT NULL,
    role_id                  BIGINT NULL,
    is_mandatory             BOOLEAN NOT NULL DEFAULT TRUE,
    auto_approve             BOOLEAN NOT NULL DEFAULT FALSE,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_workflow_step UNIQUE (workflow_id, step_no),
    CONSTRAINT fk_ws_workflow
        FOREIGN KEY (workflow_id) REFERENCES workflow_definitions(workflow_id),
    CONSTRAINT fk_ws_specific_user
        FOREIGN KEY (specific_user_id) REFERENCES users(user_id),
    CONSTRAINT fk_ws_role
        FOREIGN KEY (role_id) REFERENCES roles(role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_request_workflow (
    request_workflow_id      BIGINT PRIMARY KEY AUTO_INCREMENT,
    leave_request_id         BIGINT NOT NULL,
    workflow_id              BIGINT NOT NULL,
    current_step_no          INT NOT NULL,
    workflow_status          ENUM('IN_PROGRESS','COMPLETED','REJECTED','CANCELLED','RETURNED','WITHDRAWN') NOT NULL DEFAULT 'IN_PROGRESS',
    started_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    completed_at             DATETIME NULL,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_leave_request_workflow UNIQUE (leave_request_id),
    CONSTRAINT fk_lrw_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id),
    CONSTRAINT fk_lrw_workflow
        FOREIGN KEY (workflow_id) REFERENCES workflow_definitions(workflow_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_request_workflow_steps (
    request_workflow_step_id BIGINT PRIMARY KEY AUTO_INCREMENT,
    request_workflow_id      BIGINT NOT NULL,
    leave_request_id         BIGINT NOT NULL,
    workflow_step_id         BIGINT NOT NULL,
    step_no                  INT NOT NULL,
    step_name                VARCHAR(100) NOT NULL,
    approver_type            ENUM(
                                'REVIEWING_OFFICER',
                                'REREVIEWING_OFFICER',
                                'HR',
                                'ADMIN',
                                'SPECIFIC_USER',
                                'ROLE_BASED'
                             ) NOT NULL,
    assigned_to_user_id      BIGINT NULL,
    assigned_to_role_id      BIGINT NULL,
    step_status              ENUM('PENDING','APPROVED','REJECTED','RETURNED','SKIPPED','CANCELLED','WITHDRAWN') NOT NULL DEFAULT 'PENDING',
    action_taken_by          BIGINT NULL,
    action_taken_at          DATETIME NULL,
    action_comment           TEXT NULL,
    is_mandatory             BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT uq_request_workflow_step UNIQUE (request_workflow_id, step_no),
    CONSTRAINT fk_lrws_request_workflow
        FOREIGN KEY (request_workflow_id) REFERENCES leave_request_workflow(request_workflow_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_lrws_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_lrws_workflow_step
        FOREIGN KEY (workflow_step_id) REFERENCES workflow_steps(workflow_step_id),
    CONSTRAINT fk_lrws_assigned_to_user
        FOREIGN KEY (assigned_to_user_id) REFERENCES users(user_id),
    CONSTRAINT fk_lrws_assigned_to_role
        FOREIGN KEY (assigned_to_role_id) REFERENCES roles(role_id),
    CONSTRAINT fk_lrws_action_taken_by
        FOREIGN KEY (action_taken_by) REFERENCES users(user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_approval_actions (
    approval_action_id       BIGINT PRIMARY KEY AUTO_INCREMENT,
    leave_request_id         BIGINT NOT NULL,
    request_workflow_step_id BIGINT NULL,
    workflow_step_id         BIGINT NULL,
    acted_by                 BIGINT NOT NULL,
    acted_role_id            BIGINT NULL,
    action                   ENUM('SUBMITTED','APPROVED','REJECTED','RETURNED','CANCELLED','WITHDRAWN','ESCALATED','SKIPPED') NOT NULL,
    action_comment           TEXT NULL,
    action_at                DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    step_no                  INT NULL,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_laa_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id),
    CONSTRAINT fk_laa_request_workflow_step
        FOREIGN KEY (request_workflow_step_id) REFERENCES leave_request_workflow_steps(request_workflow_step_id),
    CONSTRAINT fk_laa_workflow_step
        FOREIGN KEY (workflow_step_id) REFERENCES workflow_steps(workflow_step_id),
    CONSTRAINT fk_laa_acted_by
        FOREIGN KEY (acted_by) REFERENCES users(user_id),
    CONSTRAINT fk_laa_acted_role
        FOREIGN KEY (acted_role_id) REFERENCES roles(role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- AUDIT / ACTIVITY
-- =========================================================

CREATE TABLE activity_logs (
    activity_log_id          BIGINT PRIMARY KEY AUTO_INCREMENT,
    entity_type              VARCHAR(50) NOT NULL,
    entity_id                BIGINT NOT NULL,
    action_type              VARCHAR(50) NOT NULL,
    action_by                BIGINT NULL,
    action_by_role_id        BIGINT NULL,
    old_values               JSON NULL,
    new_values               JSON NULL,
    ip_address               VARCHAR(50) NULL,
    user_agent               VARCHAR(255) NULL,
    remarks                  VARCHAR(255) NULL,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_al_action_by
        FOREIGN KEY (action_by) REFERENCES users(user_id),
    CONSTRAINT fk_al_action_by_role
        FOREIGN KEY (action_by_role_id) REFERENCES roles(role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- COMMENTS / ATTACHMENTS / NOTIFICATIONS
-- =========================================================

CREATE TABLE leave_comments (
    comment_id               BIGINT PRIMARY KEY AUTO_INCREMENT,
    leave_request_id         BIGINT NOT NULL,
    commented_by             BIGINT NOT NULL,
    comment_text             TEXT NOT NULL,
    is_internal              BOOLEAN NOT NULL DEFAULT FALSE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_lc_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_lc_commented_by
        FOREIGN KEY (commented_by) REFERENCES users(user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_attachments (
    attachment_id            BIGINT PRIMARY KEY AUTO_INCREMENT,
    leave_request_id         BIGINT NOT NULL,
    file_name                VARCHAR(255) NOT NULL,
    file_path                VARCHAR(500) NOT NULL,
    file_type                VARCHAR(100) NULL,
    uploaded_by              BIGINT NOT NULL,
    uploaded_at              DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_la_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_la_uploaded_by
        FOREIGN KEY (uploaded_by) REFERENCES users(user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE notifications (
    notification_id          BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id                  BIGINT NOT NULL,
    entity_type              VARCHAR(50) NOT NULL,
    entity_id                BIGINT NOT NULL,
    title                    VARCHAR(200) NOT NULL,
    message                  TEXT NOT NULL,
    notification_type        ENUM('EMAIL','IN_APP','SMS','WHATSAPP') NOT NULL DEFAULT 'IN_APP',
    is_read                  BOOLEAN NOT NULL DEFAULT FALSE,
    sent_at                  DATETIME NULL,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_notifications_user
        FOREIGN KEY (user_id) REFERENCES users(user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- HOLIDAY / WEEKEND
-- =========================================================

CREATE TABLE holidays (
    holiday_id               BIGINT PRIMARY KEY AUTO_INCREMENT,
    holiday_name             VARCHAR(150) NOT NULL,
    holiday_date             DATE NOT NULL,
    location                 VARCHAR(100) NULL,
    is_optional              BOOLEAN NOT NULL DEFAULT FALSE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_holiday_date_location UNIQUE (holiday_date, location)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE weekend_policies (
    weekend_policy_id        BIGINT PRIMARY KEY AUTO_INCREMENT,
    policy_name              VARCHAR(100) NOT NULL,
    saturday_is_off          BOOLEAN NOT NULL DEFAULT TRUE,
    sunday_is_off            BOOLEAN NOT NULL DEFAULT TRUE,
    second_saturday_off      BOOLEAN NOT NULL DEFAULT FALSE,
    fourth_saturday_off      BOOLEAN NOT NULL DEFAULT FALSE,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE user_weekend_policy (
    user_weekend_policy_id   BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id                  BIGINT NOT NULL,
    weekend_policy_id        BIGINT NOT NULL,
    effective_from           DATE NOT NULL,
    effective_to             DATE NULL,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_uwp_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_uwp_policy
        FOREIGN KEY (weekend_policy_id) REFERENCES weekend_policies(weekend_policy_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- OPTIONAL TABLES
-- =========================================================

CREATE TABLE approval_delegations (
    delegation_id            BIGINT PRIMARY KEY AUTO_INCREMENT,
    delegator_user_id        BIGINT NOT NULL,
    delegate_user_id         BIGINT NOT NULL,
    role_id                  BIGINT NOT NULL,
    start_date               DATE NOT NULL,
    end_date                 DATE NOT NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_ad_delegator
        FOREIGN KEY (delegator_user_id) REFERENCES users(user_id),
    CONSTRAINT fk_ad_delegate
        FOREIGN KEY (delegate_user_id) REFERENCES users(user_id),
    CONSTRAINT fk_ad_role
        FOREIGN KEY (role_id) REFERENCES roles(role_id),
    CONSTRAINT chk_ad_no_self_delegate CHECK (delegator_user_id <> delegate_user_id),
    CONSTRAINT chk_ad_valid_dates CHECK (end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE leave_cancellation_requests (
    cancellation_request_id  BIGINT PRIMARY KEY AUTO_INCREMENT,
    leave_request_id         BIGINT NOT NULL,
    requested_by             BIGINT NOT NULL,
    reason                   TEXT NULL,
    status                   ENUM('PENDING','APPROVED','REJECTED') NOT NULL DEFAULT 'PENDING',
    created_at               DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    processed_by             BIGINT NULL,
    processed_at             DATETIME NULL,
    CONSTRAINT fk_lcr_leave_request
        FOREIGN KEY (leave_request_id) REFERENCES leave_requests(leave_request_id),
    CONSTRAINT fk_lcr_requested_by
        FOREIGN KEY (requested_by) REFERENCES users(user_id),
    CONSTRAINT fk_lcr_processed_by
        FOREIGN KEY (processed_by) REFERENCES users(user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================
-- INDEXES
-- =========================================================

CREATE INDEX idx_users_department ON users(department_id);
CREATE INDEX idx_users_designation ON users(designation_id);
CREATE INDEX idx_user_roles_user ON user_roles(user_id);
CREATE INDEX idx_user_roles_role ON user_roles(role_id);

CREATE INDEX idx_uah_user_active_dates ON user_approval_hierarchy(user_id, is_active, effective_from, effective_to);
CREATE INDEX idx_uah_reviewing_officer ON user_approval_hierarchy(reviewing_officer_id);
CREATE INDEX idx_uah_rereviewing_officer ON user_approval_hierarchy(rereviewing_officer_id);
CREATE INDEX idx_uah_hr_officer ON user_approval_hierarchy(hr_officer_id);

CREATE INDEX idx_ulp_user ON user_leave_policy(user_id);
CREATE INDEX idx_leave_balances_user_year ON leave_balances(user_id, balance_year);
CREATE INDEX idx_lbt_user_type_date ON leave_balance_transactions(user_id, leave_type_id, created_at);

CREATE INDEX idx_leave_requests_user ON leave_requests(user_id);
CREATE INDEX idx_leave_requests_status ON leave_requests(current_status);
CREATE INDEX idx_leave_requests_dates ON leave_requests(start_date, end_date);
CREATE INDEX idx_leave_request_days_date ON leave_request_days(leave_date);
CREATE INDEX idx_leave_request_days_status ON leave_request_days(status);

CREATE INDEX idx_workflow_steps_workflow ON workflow_steps(workflow_id);
CREATE INDEX idx_lrw_leave_request ON leave_request_workflow(leave_request_id);
CREATE INDEX idx_lrws_request_workflow ON leave_request_workflow_steps(request_workflow_id);
CREATE INDEX idx_lrws_assigned_user_status ON leave_request_workflow_steps(assigned_to_user_id, step_status);
CREATE INDEX idx_leave_approval_actions_request ON leave_approval_actions(leave_request_id);
CREATE INDEX idx_leave_approval_actions_actor ON leave_approval_actions(acted_by, action_at);

CREATE INDEX idx_activity_logs_entity ON activity_logs(entity_type, entity_id);
CREATE INDEX idx_activity_logs_actor ON activity_logs(action_by, created_at);

CREATE INDEX idx_notifications_user_read ON notifications(user_id, is_read);
CREATE INDEX idx_holidays_date ON holidays(holiday_date);

-- =========================================================
-- SEED DATA
-- =========================================================

INSERT INTO roles (role_code, role_name, description) VALUES
('USER', 'User', 'Default employee/user role'),
('MANAGER', 'Manager', 'Managerial role'),
('HR', 'HR', 'Human resources role'),
('ADMIN', 'Admin', 'System administrator role');

INSERT INTO workflow_definitions (workflow_code, workflow_name, entity_type, description) VALUES
('LEAVE_L1_L2_HR', 'Leave Approval - Reviewing + Re-Reviewing + HR', 'LEAVE_REQUEST', '3-step leave workflow'),
('LEAVE_L1_L2', 'Leave Approval - Reviewing + Re-Reviewing', 'LEAVE_REQUEST', '2-step leave workflow'),
('LEAVE_L1_ONLY', 'Leave Approval - Reviewing Only', 'LEAVE_REQUEST', 'Single-step leave workflow'),
('LEAVE_HR_ONLY', 'Leave Approval - HR Only', 'LEAVE_REQUEST', 'HR direct approval workflow');

INSERT INTO workflow_steps (workflow_id, step_no, step_name, approver_type, is_mandatory, auto_approve)
SELECT workflow_id, 1, 'Reviewing Officer Approval', 'REVIEWING_OFFICER', TRUE, FALSE
FROM workflow_definitions
WHERE workflow_code = 'LEAVE_L1_L2_HR';

INSERT INTO workflow_steps (workflow_id, step_no, step_name, approver_type, is_mandatory, auto_approve)
SELECT workflow_id, 2, 'Re-Reviewing Officer Approval', 'REREVIEWING_OFFICER', TRUE, FALSE
FROM workflow_definitions
WHERE workflow_code = 'LEAVE_L1_L2_HR';

INSERT INTO workflow_steps (workflow_id, step_no, step_name, approver_type, is_mandatory, auto_approve)
SELECT workflow_id, 3, 'HR Approval', 'HR', TRUE, FALSE
FROM workflow_definitions
WHERE workflow_code = 'LEAVE_L1_L2_HR';

INSERT INTO workflow_steps (workflow_id, step_no, step_name, approver_type, is_mandatory, auto_approve)
SELECT workflow_id, 1, 'Reviewing Officer Approval', 'REVIEWING_OFFICER', TRUE, FALSE
FROM workflow_definitions
WHERE workflow_code = 'LEAVE_L1_L2';

INSERT INTO workflow_steps (workflow_id, step_no, step_name, approver_type, is_mandatory, auto_approve)
SELECT workflow_id, 2, 'Re-Reviewing Officer Approval', 'REREVIEWING_OFFICER', TRUE, FALSE
FROM workflow_definitions
WHERE workflow_code = 'LEAVE_L1_L2';

INSERT INTO workflow_steps (workflow_id, step_no, step_name, approver_type, is_mandatory, auto_approve)
SELECT workflow_id, 1, 'Reviewing Officer Approval', 'REVIEWING_OFFICER', TRUE, FALSE
FROM workflow_definitions
WHERE workflow_code = 'LEAVE_L1_ONLY';

INSERT INTO workflow_steps (workflow_id, step_no, step_name, approver_type, is_mandatory, auto_approve)
SELECT workflow_id, 1, 'HR Approval', 'HR', TRUE, FALSE
FROM workflow_definitions
WHERE workflow_code = 'LEAVE_HR_ONLY';

-- Sample leave types
INSERT INTO leave_types (
    leave_code, leave_name, description, requires_attachment, is_paid,
    is_half_day_allowed, is_backdated_allowed, max_days_per_request
) VALUES
('CL', 'Casual Leave', 'Casual leave', FALSE, TRUE, TRUE, FALSE, 3.00),
('SL', 'Sick Leave', 'Sick leave', TRUE, TRUE, TRUE, TRUE, 10.00),
('EL', 'Earned Leave', 'Earned leave', FALSE, TRUE, TRUE, FALSE, 30.00);

-- =========================================================
-- USAGE NOTES
-- =========================================================
-- 1. Create users in users table.
-- 2. Assign roles in user_roles.
-- 3. Configure user hierarchy in user_approval_hierarchy.
-- 4. Map policy using user_leave_policy.
-- 5. Pick workflow from workflow_definitions.
-- 6. On leave submit:
--      a) create leave_requests row
--      b) create leave_request_days rows
--      c) create leave_request_workflow row
--      d) expand workflow_steps into leave_request_workflow_steps
--         and assign actual approver users from user_approval_hierarchy
-- 7. On each approval/rejection:
--      a) update leave_request_workflow_steps
--      b) insert leave_approval_actions
--      c) update leave_request_workflow.current_step_no
--      d) update leave_requests.current_status
--      e) insert activity_logs
-- 8. On final approval:
--      a) debit leave_balances
--      b) insert leave_balance_transactions
-- =========================================================
'''

path = Path('/mnt/data/leave_management_mysql_schema.sql')
path.write_text(sql, encoding='utf-8')
print(f"Wrote {path}")
