# TagMyDrive - Project Summary

## What This Is

TagMyDrive is a SaaS platform built on top of [Snap2HTML](http://www.rlvision.com/snap2html). It adds user authentication, disk management, QR code generation, Google Drive auto-upload, and crypto-based membership upgrades to the original directory snapshot tool.

## Components

### Desktop App (.NET Framework 4.8 WinForms)

| Feature | Status |
|---|---|
| User registration + login (BCrypt) | Done |
| Session persistence (Remember Me) | Done |
| Clean logout (app restart) | Done |
| Disk add/rename/soft-delete | Done |
| Disk photos (URL-based, Unsplash search) | Done |
| Edit disk name + description inline | Done |
| Membership disk limit enforcement (inside `DiskService.CreateDiskAsync`) | Done |
| HTML snapshot generation | Done |
| Google Drive auto-upload (pre-authenticated on login) | Done |
| QR code generation (on-the-fly, no LONGBLOB) | Done |
| Crypto payment — USDT BEP20 via BSCScan auto-verify | Done |
| Membership upgrade form (auto-verify tx hash) | Done |
| Disk Manager — GDrive links + QR previews + action buttons | Done |
| Disks tab — QR preview + disk info + clickable GDrive link | Done |

### Services Layer

| Service | File | Purpose |
|---|---|---|
| DatabaseService | `Services/DatabaseService.cs` | Connection pooling, parameterized queries |
| AuthService | `Services/AuthService.cs` | Register, login, password change |
| DiskService | `Services/DiskService.cs` | CRUD + membership limit gate + `UpdateDiskImageUrlAsync` + `UpdateDiskDetailsAsync` |
| QRCodeService | `Services/QRCodeService.cs` | On-the-fly QR from URL string, `DeactivateAllForDiskAsync` |
| GoogleDriveService | `Services/GoogleDriveService.cs` | OAuth2, upload, `DeactivateAllForDiskAsync` |
| CryptoPaymentService | `Services/CryptoPaymentService.cs` | USDT BEP20 verification via BSCScan REST API |
| MembershipService | `Services/MembershipService.cs` | Tier lookup, `IsUserAtDiskLimitAsync` |

### Configuration

- `AppConfig.cs` — reads env vars first, falls back to `app.config`
- `app.config` — placeholder values only (secrets go in `.env`)
- `.env.example` — template with all required env var names

### Database Schema

```
users, disks, qr_codes, google_drive_links, crypto_payments, memberships, export_history
```

- `disks`: has `image_url VARCHAR(2048)` for storing external image URLs (no heavy BLOB storage)
- `crypto_payments`: USDT BEP20 fields, `confirmations` column, `coin` defaults to `'USDT'`

## Key Design Decisions

1. **Membership limit enforced at service level** — `CreateDiskAsync` checks limit inside itself. All code paths (manual Add Disk, snapshot auto-create) hit the same gate.

2. **GDrive upload blocked when disk limit reached** — `PostSnapshotFlowAsync` requires `diskId.HasValue` to upload, so no orphaned temp files are created.

3. **QR codes on-the-fly** — `QRCodeService` generates bitmaps from the stored URL string at display time. No large binary blobs in the database.

4. **No `Application.DoEvents()` in async methods** — removed all three calls from `PostSnapshotFlowAsync` to prevent re-entrancy hangs.

5. **Clean logout** — `menuLogout_Click` clears session file and calls `Application.Restart()` for a clean state.

6. **Session persistence** — `AppConfig.SaveSession/LoadSession/ClearSession` uses `%AppData%/TagMyDrive/session.dat`. `Program.cs` auto-logins on startup if valid.

7. **Environment variables for secrets** — env vars override `app.config`. Public-safe: no hardcoded credentials in source.

8. **Disk images stored as URLs** — `image_url VARCHAR(2048)` in `disks` table. Users paste any image URL or search Unsplash. Both desktop and web support URL input with live preview. No heavy BLOB storage in database.

## Crypto Payment Flow

- **Coin:** USDT BEP20 only
- **Contract:** `0x55d398326f99059fF775485246999027B3197955` (BSC)
- **Verification:** BSCScan API — checks tx receipt, USDT contract transfer, recipient, amount, 12+ confirmations
- **Result:** Immediate membership upgrade on success; no admin intervention required

## Tech Stack

| Layer | Tech |
|---|---|
| Desktop | C#, WinForms, .NET Framework 4.8 |
| Database | MySQL (MySqlConnector) |
| Auth | BCrypt.Net-Next |
| QR Codes | QRCoder (on-the-fly) |
| Google Drive | Google.Apis.Drive.v3 |
| Crypto | BSCScan REST API (USDT BEP20) |
| Web companion | Next.js 14, TypeScript, Tailwind CSS |

## Files to Configure Before Use

| File | What to set |
|---|---|
| `.env` (copy from `.env.example`) | DB connection, Google OAuth, BSCScan key, wallet address |
| `app.config` (alternative to .env) | Same keys as appSettings / connectionStrings |

## Build & Run

```bash
# Set env vars (Windows PowerShell)
$env:TAGMYDRIVE_DB_CONNECTION = "Server=...;Database=...;Uid=...;Pwd=...;SslMode=Required;"
$env:TAGMYDRIVE_GOOGLE_CLIENT_ID = "..."
$env:TAGMYDRIVE_GOOGLE_CLIENT_SECRET = "..."
$env:TAGMYDRIVE_CRYPTO_WALLET_USDT = "0x..."
$env:TAGMYDRIVE_BSCSCAN_API_KEY = "..."

# Build
msbuild TagMyDrive.sln /p:Configuration=Release
```

Or set values in `.env` / `app.config` and open in Visual Studio.
