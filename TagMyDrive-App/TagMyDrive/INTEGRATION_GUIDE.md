# TagMyDrive - Integration Guide

## Project Structure

```
TagMyDrive-master/
├── TagMyDrive/                        # .NET Framework 4.8 WinForms app
│   ├── Services/
│   │   ├── DatabaseService.cs         # Connection pooling, parameterized queries
│   │   ├── AuthService.cs             # Register, login, BCrypt password hashing
│   │   ├── DiskService.cs             # Disk CRUD + membership limit enforcement
│   │   ├── QRCodeService.cs           # QR on-the-fly generation (no LONGBLOB in DB)
│   │   ├── GoogleDriveService.cs      # OAuth2 + Drive file upload
│   │   ├── CryptoPaymentService.cs    # USDT BEP20 verification via BSCScan API
│   │   └── MembershipService.cs       # Tier lookup + IsUserAtDiskLimitAsync
│   ├── Database/
│   │   └── schema.sql                 # Full MySQL schema
│   ├── frmMain.cs                     # Main window, PostSnapshotFlowAsync
│   ├── frmLogin.cs                    # Login with "Remember Me"
│   ├── frmRegister.cs                 # User registration
│   ├── frmDiskManager.cs              # Disk list + image URL preview + edit name/desc/image + GDrive links + QR previews
│   ├── frmQRCodeManager.cs            # QR code list + download
│   ├── frmMembership.cs               # Crypto payment upgrade flow
│   ├── frmSnapshotResult.cs           # Post-snapshot result dialog
│   ├── AppConfig.cs                   # Env var fallback → app.config
│   ├── app.config                     # Template config (no hardcoded secrets)
│   └── Program.cs                     # Auto-login from saved session
├── .env.example                       # Env var template
├── .gitignore
├── TagMyDrive.sln
└── README.md
```

## Configuration

All secrets are read via environment variables first, falling back to `app.config`:

| Env Var | app.config Key | Purpose |
|---|---|---|
| `TAGMYDRIVE_DB_CONNECTION` | `ConnectionStrings[TagMyDriveDb]` | MySQL connection string |
| `TAGMYDRIVE_GOOGLE_CLIENT_ID` | `GoogleDriveClientId` | Google OAuth client ID |
| `TAGMYDRIVE_GOOGLE_CLIENT_SECRET` | `GoogleDriveClientSecret` | Google OAuth client secret |
| `TAGMYDRIVE_CRYPTO_WALLET_USDT` | `CryptoWalletUSDT` | BEP20 wallet for payments |
| `TAGMYDRIVE_BSCSCAN_API_KEY` | `BSCScanApiKey` | BSCScan API key for tx verification |

## Setup

### 1. Database

```bash
mysql -h your-host -u your_user -p your_db < TagMyDrive/Database/schema.sql
```

### 2. Environment

```bash
cp .env.example .env
# Fill in your credentials
```

### 3. Build

```bash
msbuild TagMyDrive.sln /p:Configuration=Release
```

Or open in Visual Studio and build.

## Key Flows

### Login + Session Persistence

1. User enters credentials → `AuthService.LoginAsync`
2. If "Remember Me" checked → `AppConfig.SaveSession(userId)` writes to `%AppData%/TagMyDrive/session.dat`
3. On next launch → `Program.cs` calls `AppConfig.LoadSession()` → auto-login if valid
4. `InitializeUserSessionAsync()` pre-authenticates Google Drive

### Snapshot → Upload → QR

1. User selects folder → `PostSnapshotFlowAsync()` in `frmMain.cs`
2. HTML snapshot generated into temp folder
3. `DiskService.CreateDiskAsync()` checks membership limit **inside itself** — blocks if at limit
4. If disk created (`diskId.HasValue`) → `GoogleDriveService.UploadFileAsync()` runs
5. Temp folder cleaned up
6. QR code metadata saved (URL only, no image blob)
7. `frmSnapshotResult` shows outcome

**When disk limit reached:** `CreateDiskAsync` returns false → `diskId` is null → GDrive upload skipped entirely → no orphaned files.

### Crypto Payment (USDT BEP20)

1. `frmMembership` shows wallet address from `AppConfig.GetCryptoWalletUSDT()`
2. User submits tx hash
3. `CryptoPaymentService.VerifyTransactionAsync()` calls BSCScan API:
   - Fetches tx receipt → checks status
   - Checks USDT contract (`0x55d398326f99059fF775485246999027B3197955`)
   - Checks recipient matches wallet address
   - Checks amount >= membership price
   - Requires 12+ confirmations
4. On success → `MembershipService` upgrades tier immediately

## Database Schema (Key Tables)

```
users          — id, username, email, password_hash, membership_id, is_active
disks          — id, user_id, name, description, disk_path, disk_type, image_url (VARCHAR 2048), is_active
qr_codes       — id, disk_id, user_id, qr_code_url, download_count, is_active
google_drive_links — id, disk_id, user_id, google_drive_url, google_file_id, is_active
crypto_payments    — id, user_id, tx_hash, amount, coin='USDT', status, confirmations
memberships     — id, name, max_disks, price_monthly
```

## Services Reference

| Service | Key Methods |
|---|---|
| `AuthService` | `RegisterAsync`, `LoginAsync`, `ChangePasswordAsync` |
| `DiskService` | `CreateDiskAsync` (limit check inside), `GetDisksByUserAsync`, `UpdateDiskSnapshotAsync`, `GetDiskByPathAsync`, `UpdateDiskImageUrlAsync`, `UpdateDiskDetailsAsync` |
| `MembershipService` | `GetMembershipByIdAsync`, `IsUserAtDiskLimitAsync` |
| `QRCodeService` | `GenerateQRCodeAsync` (metadata only), `GetQRCodeImageAsync` (on-the-fly bitmap), `DeactivateAllForDiskAsync` |
| `GoogleDriveService` | `UploadFileAsync`, `AuthenticateAsync`, `DeactivateAllForDiskAsync` |
| `CryptoPaymentService` | `VerifyTransactionAsync` (BSCScan auto-verify), `RecordPaymentAsync` |
