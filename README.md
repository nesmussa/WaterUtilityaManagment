# Water Utility Management System

Desktop water billing and customer management application built with VB.NET Windows Forms and MySQL.

## Overview

This project supports a role-based workflow for:
- customer registration and meter management,
- bill generation from meter readings,
- payment collection and bill allocation,
- manager-level reporting and staff administration.

The application uses MySQL as the data store and includes SQL schema/setup scripts in `WaterUtilityaManagment/Database/WaterUtilityDB.sql`.

## Tech Stack

- VB.NET (`net10.0-windows`)
- Windows Forms
- MySQL
- `MySql.Data`
- `System.Configuration.ConfigurationManager`

## Role-Based Features

### Manager
- Access manager dashboard (`frmReports`)
- View reports:
  - paid vs unpaid summary,
  - revenue by date range,
  - outstanding balances,
  - staff activity
- Manage staff accounts (`frmUserManagement`):
  - add staff,
  - reset password,
  - enable/disable account
- Manage tariffs (`frmTariffs`, `frmTariffEdit`)

### Staff
- Access meter reading form (`frmEnterReading`)
- Register customers (`frmRegisterCustomer`)
- Save new readings and auto-generate bills from active tariff
- Process payments and allocate amounts to selected unpaid bills (`frmPayment`)

### Customer
- Access customer dashboard (`frmCustomerDashboard`)
- View latest readings, bills, and payment history
- Change password
- Pay outstanding bills through online payment form (`frmOnlinePayment`)

## Core Components

- `DatabaseHelper.vb`: shared DB operations and core schema checks
- `PasswordHelper.vb`: SHA-256 hashing/verification
- `AuditLogger.vb`: audit log writes for key actions
- `CurrentUser.vb`: in-memory session user state
- `SessionManager.vb`: centralized logout/navigation reset

## Database

Use `WaterUtilityaManagment/Database/WaterUtilityDB.sql` to create:
- tables (`users`, `customers`, `meter_readings`, `bills`, `payments`, `payment_allocations`, `tariffs`, `audit_log`)
- views and stored procedures for reporting/history
- sample seed users

The app also ensures some schema elements at runtime (for example `is_active`, `force_password_change`, and `audit_log` structure updates).

## Setup

1. Install MySQL Server.
2. Run `WaterUtilityaManagment/Database/WaterUtilityDB.sql`.
3. Update connection string in `WaterUtilityaManagment/App.config`.
4. Build and run the project.
5. Open the app and click **Go To Login**.

## Default Login Accounts

Use these accounts for initial testing.

| Role | Username | Password | Access |
|---|---|---|---|
| Manager | `admin` | `admin123` | Full access to all features, reports, and user management |
| Staff | `staff1` | `staff123` | Can enter meter readings, record payments, and view customers |
| Customer | `cust1` | `customer123` | Can view own bills, payments, and consumption history |

## Security Notes

- Passwords are stored as SHA-256 hashes.
- Audit records are created for login and key operations.
- Default credentials are for development/demo use only.
- Change all default passwords before production use.
