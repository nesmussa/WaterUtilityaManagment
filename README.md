# Water Utility Management System

Desktop water utility billing and customer management application built with `VB.NET WinForms` and `MySQL`.

## Student Information

- **Name:** Neserelah mussa
- **ID:** IUNSR/0623/13

## Project Description

This system automates the core workflow of a water utility office:

1. Registering customers and assigning meters.
2. Recording meter readings.
3. Generating bills from consumption and active tariff rates.
4. Receiving payments and allocating them to bills.
5. Monitoring operations through manager reports and audit logs.

The application is role-based and provides separate dashboards and tools for managers, staff, and customers.

## Technology Stack

- `VB.NET` (`net10.0-windows`)
- `Windows Forms`
- `MySQL`
- `MySql.Data`
- `System.Configuration.ConfigurationManager`
- `iText7` (PDF export)

## Key Functional Modules

### Authentication and Session
- Secure login with SHA-256 hashed password verification.
- Role-based navigation to dashboards.
- Session state handled using `CurrentUser` and `SessionManager`.

### Customer Registration and Meter Setup
- Staff can register a customer in `frmRegisterCustomer`.
- Auto-generates meter number.
- Captures initial reading and creates initial meter reading record.
- Includes form validation for:
  - email format
  - Ethiopian phone format (`09`, `07`, `+2519`, `+2517`)

### Meter Reading and Billing
- Staff records new readings in `frmEnterReading`.
- Consumption is calculated from previous reading.
- Bill is generated automatically using active tariff.

### Payment Management
- Staff can process payments in `frmPayment`.
- Customer online payment available in `frmOnlinePayment`.
- Bill allocation and outstanding balance calculations are supported.

### Reporting and Export
Manager dashboard `frmReports` includes:
- Paid/Unpaid/Partial bill status summary
- Revenue by period
- Outstanding balances
- Staff activity
- Export support:
  - CSV export
  - Styled PDF table export (headers, borders, alternating row colors)

### User and Profile Management
- User account administration in `frmUserManagement`.
- Add staff, reset passwords, enable/disable users.
- Profile updates (username/full name/password) through `frmProfileSettings`.

### Auditing and Maintenance
- Action logging through `AuditLogger`.
- Backup and restore tools.
- Runtime schema compatibility checks via `DatabaseHelper.EnsureCoreSchema()`.

## Important Project Files

- `WaterUtilityaManagment/frmLogin.vb` – login and role routing
- `WaterUtilityaManagment/frmReports.vb` – manager dashboard/reports
- `WaterUtilityaManagment/frmEnterReading.vb` – staff operations
- `WaterUtilityaManagment/frmCustomerDashboard.vb` – customer dashboard
- `WaterUtilityaManagment/frmRegisterCustomer.vb` – customer onboarding
- `WaterUtilityaManagment/frmUserManagement.vb` – user management
- `WaterUtilityaManagment/GridExportHelper.vb` – CSV/PDF exports
- `WaterUtilityaManagment/DatabaseHelper.vb` – DB utilities and schema checks
- `WaterUtilityaManagment/Database/WaterUtilityDB.sql` – schema and seed script

## Database

Run `WaterUtilityaManagment/Database/WaterUtilityDB.sql` to create:

- Tables: `users`, `customers`, `meter_readings`, `bills`, `payments`, `payment_allocations`, `tariffs`, `audit_log`
- Views/stored procedures for reporting and history
- Seed users for manager, staff, and customer testing

The application also adds/aligns some compatibility columns at runtime (`is_active`, `force_password_change`, and `audit_log` structure fields).

## Setup Instructions

1. Install MySQL Server.
2. Execute `WaterUtilityaManagment/Database/WaterUtilityDB.sql`.
3. Update DB connection string in `WaterUtilityaManagment/App.config`.
4. Restore NuGet packages.
5. Build and run the project.
6. From welcome screen, click **Go To Login**.

## Default Test Accounts

| Role | Username | Password | Access |
|---|---|---|---|
| Manager | `admin` | `admin123` | Full reporting and administration |
| Staff | `staff1` | `staff123` | Meter reading, customer registration, payment processing |
| Customer | `cust1` | `customer123` | View own usage, bills, and payments |

## Security Notes

- Passwords are hashed using SHA-256 before storage.
- Sensitive actions are logged in `audit_log`.
- Default credentials are for demo/testing only.
- Change default passwords before production deployment.
