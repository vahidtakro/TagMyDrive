# TagMyDrive

A multi-user SaaS platform for managing hard disk directory listings, generating QR codes, and uploading snapshots to Google Drive — built on top of [Snap2HTML](http://www.rlvision.com/snap2html).

## Features

- **User authentication** — register, login, change password, session persistence (remember me)
- **Disk management** — add, rename, soft-delete disks; set custom names, descriptions, and photos; membership-enforced disk limits
- **Disk photos** — paste any image URL or search Unsplash to visually identify your hard drives
- **Directory snapshots** — generate HTML directory listings from any folder and upload to Google Drive
- **QR codes** — generate QR codes linking to uploaded Google Drive snapshots (generated on-the-fly, no LONGBLOB storage)
- **Google Drive auto-upload** — authenticated users upload snapshots directly to their own Drive
- **Crypto payments** — USDT BEP20 auto-verification via BSCScan API for membership upgrades
- **Membership tiers** — Free (2 disks), Basic (10 disks), Premium (unlimited)
- **Web companion** — separate Next.js web app sharing the same MySQL database

## Quick Start

### Prerequisites

- Windows with .NET Framework 4.8
- MySQL 5.7+ (or compatible MariaDB)
- Visual Studio 2019+ (or MSBuild)

### 1. Database

```bash
mysql -h your-host -u your_user -p your_db < TagMyDrive/Database/schema.sql
```

For existing databases, migrate disk images from LONGBLOB to URL:

```sql
ALTER TABLE disks ADD COLUMN image_url VARCHAR(2048) AFTER disk_type;
ALTER TABLE disks DROP COLUMN image_data;
```

### 2. Configure

Copy `.env.example` to `.env` and fill in your values, **or** edit `TagMyDrive/app.config` directly:

```bash
cp .env.example .env
# Edit .env with your database, Google OAuth, and BSCScan credentials
```

| Variable | Description |
|---|---|
| `TAGMYDRIVE_DB_CONNECTION` | Full MySQL connection string |
| `TAGMYDRIVE_GOOGLE_CLIENT_ID` | Google Cloud OAuth client ID |
| `TAGMYDRIVE_GOOGLE_CLIENT_SECRET` | Google Cloud OAuth client secret |
| `TAGMYDRIVE_CRYPTO_WALLET_USDT` | BEP20 wallet address for payments |
| `TAGMYDRIVE_BSCSCAN_API_KEY` | Free key from [bscscan.com/myapikey](https://bscscan.com/myapikey) |

Environment variables take priority over `app.config` values.

### 3. Build & Run

Open `TagMyDrive.sln` in Visual Studio and build, or:

```bash
msbuild TagMyDrive.sln /p:Configuration=Release
```

## Project Structure

```
TagMyDrive-master/
├── TagMyDrive/
│   ├── Services/
│   │   ├── DatabaseService.cs        # MySQL connection + query execution
│   │   ├── AuthService.cs            # Register, login, password change
│   │   ├── DiskService.cs            # Disk CRUD + membership limit enforcement
│   │   ├── QRCodeService.cs          # QR code on-the-fly generation
│   │   ├── GoogleDriveService.cs     # OAuth2 + file upload
│   │   ├── CryptoPaymentService.cs   # USDT BEP20 auto-verify via BSCScan
│   │   └── MembershipService.cs      # Tier management + disk limit checks
│   ├── Database/
│   │   └── schema.sql
│   ├── frmMain.cs                    # Main window + snapshot flow
│   ├── frmLogin.cs                   # Login with remember-me
│   ├── frmRegister.cs                # User registration
│   ├── frmDiskManager.cs             # Disk list + images + GDrive links + QR previews + edit
│   ├── frmQRCodeManager.cs           # QR code list + on-the-fly generation
│   ├── frmMembership.cs              # Upgrade via crypto payment
│   ├── frmSnapshotResult.cs          # Post-snapshot dialog
│   ├── AppConfig.cs                  # Config with env var fallbacks
│   └── app.config                    # Placeholder config (secrets go in .env)
├── .env.example                      # Template for environment variables
├── .gitignore
├── TagMyDrive.sln
└── README.md
```

## Tech Stack

| Layer | Technology |
|---|---|
| Desktop | C#, WinForms, .NET Framework 4.8 |
| Database | MySQL via MySqlConnector |
| Auth | BCrypt.Net-Next password hashing |
| QR Codes | QRCoder (on-the-fly bitmap generation) |
| Google Drive | Google.Apis.Drive.v3 |
| Crypto payments | BSCScan REST API (USDT BEP20) |

## License

Original Snap2HTML copyright (c) RL Vision 2011-2026 preserved.
