-- Create database
CREATE DATABASE IF NOT EXISTS WaterUtilityDB;
USE WaterUtilityDB;

-- 1. Users table (login credentials + role)
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,   -- store hashed passwords
    role ENUM('customer','staff','manager') NOT NULL,
    full_name VARCHAR(100),
    email VARCHAR(100),
    phone VARCHAR(20),
    force_password_change BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. Customers table (extends users with role='customer')
CREATE TABLE customers (
    id INT PRIMARY KEY,                     -- same as users.id
    address TEXT,
    meter_number VARCHAR(50) UNIQUE NOT NULL,
    installation_date DATE,
    last_reading DECIMAL(10,2) DEFAULT 0,   -- last recorded meter value
    last_reading_date DATE,
    FOREIGN KEY (id) REFERENCES users(id) ON DELETE CASCADE
);

-- 3. Tariffs table (water rates)
CREATE TABLE tariffs (
    tariff_id INT AUTO_INCREMENT PRIMARY KEY,
    rate_per_unit DECIMAL(10,2) NOT NULL,   -- price per cubic meter
    effective_from DATE NOT NULL,
    effective_to DATE DEFAULT NULL           -- NULL means currently active
);

-- 4. Meter readings table (corrected - no GENERATED column)
CREATE TABLE meter_readings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    reading_value DECIMAL(10,2) NOT NULL,
    reading_date DATE NOT NULL,
    entered_by INT NOT NULL,                 -- staff user id
    consumption DECIMAL(10,2),                -- to be filled by VB.NET logic
    FOREIGN KEY (customer_id) REFERENCES customers(id) ON DELETE CASCADE,
    FOREIGN KEY (entered_by) REFERENCES users(id)
);

-- 5. Bills table
CREATE TABLE bills (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    reading_id INT NOT NULL,                 -- the reading that generated this bill
    bill_date DATE NOT NULL,
    due_date DATE NOT NULL,
    total_amount DECIMAL(10,2) NOT NULL,
    status ENUM('Paid','Unpaid','Partial') DEFAULT 'Unpaid',
    FOREIGN KEY (customer_id) REFERENCES customers(id),
    FOREIGN KEY (reading_id) REFERENCES meter_readings(id)
);

-- 6. Payments table
CREATE TABLE payments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    amount_paid DECIMAL(10,2) NOT NULL,
    payment_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    mode ENUM('Cash','Bank Transfer','Mobile Money','Online') NOT NULL,
    reference VARCHAR(100),
    received_by INT NOT NULL,                 -- staff user id
    FOREIGN KEY (customer_id) REFERENCES customers(id),
    FOREIGN KEY (received_by) REFERENCES users(id)
);

-- 7. Payment allocations (link payments to bills)
CREATE TABLE payment_allocations (
    payment_id INT NOT NULL,
    bill_id INT NOT NULL,
    amount_applied DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (payment_id, bill_id),
    FOREIGN KEY (payment_id) REFERENCES payments(id) ON DELETE CASCADE,
    FOREIGN KEY (bill_id) REFERENCES bills(id)
);

-- 8. Audit log table (for tracking actions)
CREATE TABLE audit_log (
    audit_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NULL,
    action_name VARCHAR(100) NOT NULL,
    table_name VARCHAR(50),
    record_id INT,
    details TEXT,
    ip_address VARCHAR(45),
    action_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- Insert a default admin/manager (password: admin123, hashed with SHA2)
INSERT INTO users (username, password_hash, role, full_name, email, phone, force_password_change, is_active) 
VALUES ('admin', SHA2('admin123', 256), 'manager', 'System Administrator', 'admin@water.com', '1234567890', FALSE, TRUE);

-- Insert a sample staff user (password: staff123)
INSERT INTO users (username, password_hash, role, full_name, email, phone, force_password_change, is_active) 
VALUES ('staff1', SHA2('staff123', 256), 'staff', 'John Staff', 'john@water.com', '1234567891', FALSE, TRUE);

-- Insert a sample customer user (password: customer123)
INSERT INTO users (username, password_hash, role, full_name, email, phone, force_password_change, is_active) 
VALUES ('cust1', SHA2('customer123', 256), 'customer', 'Jane Customer', 'jane@email.com', '1234567892', TRUE, TRUE);

-- Insert the customer details (using the last inserted ID)
INSERT INTO customers (id, address, meter_number, installation_date, last_reading, last_reading_date) 
VALUES (LAST_INSERT_ID(), '123 Main St, City', 'MTR001', CURDATE(), 70, CURDATE());

-- Insert a sample tariff (effective from today)
INSERT INTO tariffs (rate_per_unit, effective_from) VALUES (5.00, CURDATE());

-- Insert sample meter reading for the customer (initial reading)
INSERT INTO meter_readings (customer_id, reading_value, reading_date, entered_by, consumption) 
VALUES (3, 70, CURDATE(), 2, 0);

-- Create some useful views for reporting

-- View: Customer outstanding balances
CREATE VIEW view_outstanding_balances AS
SELECT 
    u.id AS customer_id,
    u.full_name,
    c.meter_number,
    u.phone,
    SUM(b.total_amount) AS total_outstanding
FROM users u
JOIN customers c ON u.id = c.id
JOIN bills b ON c.id = b.customer_id
WHERE b.status IN ('Unpaid', 'Partial')
GROUP BY u.id, u.full_name, c.meter_number, u.phone;

-- View: Monthly revenue summary
CREATE VIEW view_monthly_revenue AS
SELECT 
    DATE_FORMAT(payment_date, '%Y-%m') AS month,
    COUNT(DISTINCT id) AS transaction_count,
    SUM(amount_paid) AS total_revenue,
    AVG(amount_paid) AS avg_transaction
FROM payments
GROUP BY DATE_FORMAT(payment_date, '%Y-%m')
ORDER BY month DESC;

-- View: Staff activity summary
CREATE VIEW view_staff_activity AS
SELECT 
    u.full_name AS staff_name,
    u.username,
    COUNT(DISTINCT mr.id) AS readings_entered,
    COUNT(DISTINCT p.id) AS payments_recorded,
    MAX(mr.reading_date) AS last_reading_date,
    MAX(p.payment_date) AS last_payment_date
FROM users u
LEFT JOIN meter_readings mr ON u.id = mr.entered_by
LEFT JOIN payments p ON u.id = p.received_by
WHERE u.role IN ('staff', 'manager')
GROUP BY u.id, u.full_name, u.username;

-- View: Paid vs Unpaid summary
CREATE VIEW view_paid_unpaid_summary AS
SELECT 
    status,
    COUNT(*) AS bill_count,
    SUM(total_amount) AS total_amount
FROM bills
GROUP BY status;

-- Add indexes for better performance
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_customers_meter ON customers(meter_number);
CREATE INDEX idx_readings_customer_date ON meter_readings(customer_id, reading_date);
CREATE INDEX idx_bills_customer_status ON bills(customer_id, status);
CREATE INDEX idx_bills_due_date ON bills(due_date);
CREATE INDEX idx_payments_date ON payments(payment_date);
CREATE INDEX idx_payments_customer ON payments(customer_id);

-- Stored procedure: Get customer bill history
DELIMITER //
CREATE PROCEDURE GetCustomerBillHistory(IN p_customer_id INT)
BEGIN
    SELECT 
        b.id AS bill_id,
        b.bill_date,
        b.due_date,
        b.total_amount,
        b.status,
        mr.reading_value,
        mr.reading_date,
        mr.consumption,
        t.rate_per_unit
    FROM bills b
    JOIN meter_readings mr ON b.reading_id = mr.id
    JOIN tariffs t ON mr.reading_date BETWEEN t.effective_from AND COALESCE(t.effective_to, CURDATE())
    WHERE b.customer_id = p_customer_id
    ORDER BY b.bill_date DESC;
END//
DELIMITER ;

-- Stored procedure: Get customer payment history
DELIMITER //
CREATE PROCEDURE GetCustomerPaymentHistory(IN p_customer_id INT)
BEGIN
    SELECT 
        p.id AS payment_id,
        p.amount_paid,
        p.payment_date,
        p.mode,
        p.reference,
        GROUP_CONCAT(DISTINCT b.id) AS bill_ids,
        SUM(pa.amount_applied) AS total_applied
    FROM payments p
    JOIN payment_allocations pa ON p.id = pa.payment_id
    JOIN bills b ON pa.bill_id = b.id
    WHERE p.customer_id = p_customer_id
    GROUP BY p.id, p.amount_paid, p.payment_date, p.mode, p.reference
    ORDER BY p.payment_date DESC;
END//
DELIMITER ;

-- Trigger: Automatically log new user registrations
DELIMITER //
CREATE TRIGGER after_user_insert
AFTER INSERT ON users
FOR EACH ROW
BEGIN
    INSERT INTO audit_log (user_id, action_name, table_name, record_id, details, action_time)
    VALUES (NEW.id, 'UserInserted', 'users', NEW.id, CONCAT('New user created: ', NEW.username, ' with role: ', NEW.role), NOW());
END//
DELIMITER ;

-- Trigger: Log meter reading entries
DELIMITER //
CREATE TRIGGER after_meter_reading_insert
AFTER INSERT ON meter_readings
FOR EACH ROW
BEGIN
    INSERT INTO audit_log (user_id, action_name, table_name, record_id, details, action_time)
    VALUES (NEW.entered_by, 'MeterReadingInserted', 'meter_readings', NEW.id, 
            CONCAT('Reading entered: ', NEW.reading_value, ' for customer: ', NEW.customer_id), NOW());
END//
DELIMITER ;

-- Trigger: Log payments
DELIMITER //
CREATE TRIGGER after_payment_insert
AFTER INSERT ON payments
FOR EACH ROW
BEGIN
    INSERT INTO audit_log (user_id, action_name, table_name, record_id, details, action_time)
    VALUES (NEW.received_by, 'PaymentInserted', 'payments', NEW.id, 
            CONCAT('Payment of ', NEW.amount_paid, ' received from customer: ', NEW.customer_id), NOW());
END//
DELIMITER ;

-- Function: Calculate consumption between two readings
DELIMITER //
CREATE FUNCTION CalculateConsumption(start_reading DECIMAL(10,2), end_reading DECIMAL(10,2))
RETURNS DECIMAL(10,2)
DETERMINISTIC
BEGIN
    RETURN end_reading - start_reading;
END//
DELIMITER ;

-- Function: Get current active tariff
DELIMITER //
CREATE FUNCTION GetCurrentTariff()
RETURNS DECIMAL(10,2)
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_rate DECIMAL(10,2);
    
    SELECT rate_per_unit INTO v_rate
    FROM tariffs
    WHERE (effective_to IS NULL OR effective_to >= CURDATE())
    ORDER BY effective_from DESC
    LIMIT 1;
    
    RETURN v_rate;
END//
DELIMITER ;

-- Show all tables created
SHOW TABLES;

-- Display sample data
SELECT 'Users table:' as '';
SELECT id, username, role, full_name, is_active FROM users;

SELECT 'Tariffs table:' as '';
SELECT * FROM tariffs;

SELECT 'Customers table:' as '';
SELECT * FROM customers;

SELECT 'Sample views created:' as '';
SHOW FULL TABLES IN WaterUtilityDB WHERE TABLE_TYPE LIKE 'VIEW';