-- TagMyDrive Database Schema
-- MySQL database for TagMyDrive platform
-- Users, Memberships, Disks/Folders, QR Codes, Google Drive Links, and Crypto Payments

-- Memberships table (tiers: Free, Basic, Premium)
CREATE TABLE IF NOT EXISTS memberships (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(100) NOT NULL UNIQUE,
  description TEXT,
  max_disks INT NOT NULL,
  price_monthly DECIMAL(10, 2),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert default membership tiers
INSERT INTO memberships (name, description, max_disks, price_monthly) VALUES
  ('Free', 'Free tier with limited disks', 2, 0.00),
  ('Basic', 'Basic membership for power users', 10, 4.99),
  ('Premium', 'Unlimited everything', 999, 9.99)
ON DUPLICATE KEY UPDATE name=name;

-- Users table
CREATE TABLE IF NOT EXISTS users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(255) NOT NULL UNIQUE,
  email VARCHAR(255) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  first_name VARCHAR(255),
  last_name VARCHAR(255),
  membership_id INT NOT NULL DEFAULT 1,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  is_active BOOLEAN DEFAULT TRUE,
  FOREIGN KEY (membership_id) REFERENCES memberships(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Disks table (user's hard drives/folders)
CREATE TABLE IF NOT EXISTS disks (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  disk_path VARCHAR(500),
  disk_type ENUM('local', 'external', 'network', 'snapshot') DEFAULT 'local',
  image_url VARCHAR(2048),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  is_active BOOLEAN DEFAULT TRUE,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  UNIQUE KEY unique_user_disk (user_id, name),
  INDEX idx_user_id (user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- QR Codes table (QR code metadata, images generated on-the-fly from URL)
CREATE TABLE IF NOT EXISTS qr_codes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  disk_id INT NOT NULL,
  user_id INT NOT NULL,
  qr_code_url VARCHAR(500),
  html_export_filename VARCHAR(255),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  expires_at TIMESTAMP NULL,
  is_active BOOLEAN DEFAULT TRUE,
  download_count INT DEFAULT 0,
  last_accessed TIMESTAMP NULL,
  FOREIGN KEY (disk_id) REFERENCES disks(id) ON DELETE CASCADE,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  INDEX idx_disk_id (disk_id),
  INDEX idx_user_id (user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Google Drive Links table (tracking uploaded files to Google Drive)
CREATE TABLE IF NOT EXISTS google_drive_links (
  id INT AUTO_INCREMENT PRIMARY KEY,
  qr_code_id INT,
  disk_id INT NOT NULL,
  user_id INT NOT NULL,
  google_file_id VARCHAR(255),
  google_file_name VARCHAR(255),
  google_drive_url VARCHAR(500),
  html_file_name VARCHAR(255),
  uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  is_active BOOLEAN DEFAULT TRUE,
  access_token VARCHAR(500),
  refresh_token VARCHAR(500),
  token_expires_at TIMESTAMP NULL,
  FOREIGN KEY (qr_code_id) REFERENCES qr_codes(id) ON DELETE SET NULL,
  FOREIGN KEY (disk_id) REFERENCES disks(id) ON DELETE CASCADE,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  INDEX idx_disk_id (disk_id),
  INDEX idx_user_id (user_id),
  INDEX idx_google_file_id (google_file_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Export History table (track all HTML exports)
CREATE TABLE IF NOT EXISTS export_history (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  disk_id INT,
  html_filename VARCHAR(255),
  file_size BIGINT,
  exported_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  export_format VARCHAR(50),
  include_links BOOLEAN DEFAULT FALSE,
  include_system BOOLEAN DEFAULT FALSE,
  include_hidden BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  FOREIGN KEY (disk_id) REFERENCES disks(id) ON DELETE SET NULL,
  INDEX idx_user_id (user_id),
  INDEX idx_disk_id (disk_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- User Sessions table (track logged-in sessions)
CREATE TABLE IF NOT EXISTS user_sessions (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  session_token VARCHAR(500) NOT NULL UNIQUE,
  ip_address VARCHAR(45),
  user_agent VARCHAR(500),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  expires_at TIMESTAMP,
  is_active BOOLEAN DEFAULT TRUE,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  INDEX idx_user_id (user_id),
  INDEX idx_session_token (session_token)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Crypto Payments table (USDT BEP20 payments with automatic BSCScan verification)
CREATE TABLE IF NOT EXISTS crypto_payments (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  membership_id INT NOT NULL,
  coin VARCHAR(10) NOT NULL DEFAULT 'USDT',
  wallet_address VARCHAR(500) NOT NULL,
  amount DECIMAL(10, 2) NOT NULL,
  status ENUM('pending', 'awaiting_verification', 'confirmed', 'rejected', 'expired') DEFAULT 'pending',
  transaction_hash VARCHAR(500),
  confirmations INT DEFAULT 0,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  verified_at TIMESTAMP NULL,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  FOREIGN KEY (membership_id) REFERENCES memberships(id),
  INDEX idx_user_id (user_id),
  INDEX idx_status (status),
  INDEX idx_transaction_hash (transaction_hash)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create indexes for better query performance
CREATE INDEX idx_users_email ON users(email);

-- Add image_url column to existing disks tables (safe to run multiple times)
SET @column_exists_url = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'disks' AND COLUMN_NAME = 'image_url');
SET @column_exists_img = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'disks' AND COLUMN_NAME = 'image_data');
SET @sql_url = IF(@column_exists_url = 0, 'ALTER TABLE disks ADD COLUMN image_url VARCHAR(2048) AFTER disk_type', 'SELECT 1');
SET @sql_img = IF(@column_exists_img > 0, 'ALTER TABLE disks DROP COLUMN image_data', 'SELECT 1');
PREPARE stmt_url FROM @sql_url;
EXECUTE stmt_url;
DEALLOCATE PREPARE stmt_url;
PREPARE stmt_img FROM @sql_img;
EXECUTE stmt_img;
DEALLOCATE PREPARE stmt_img;
CREATE INDEX idx_users_membership_id ON users(membership_id);
CREATE INDEX idx_disks_user_created ON disks(user_id, created_at);
CREATE INDEX idx_qr_codes_created ON qr_codes(created_at);
CREATE INDEX idx_google_drive_links_uploaded ON google_drive_links(uploaded_at);
CREATE INDEX idx_export_history_exported ON export_history(exported_at);
