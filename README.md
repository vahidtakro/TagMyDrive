# TagMyDrive

**Never lose track of what's on your hard drives again.**

TagMyDrive is a personal data inventory platform that lets you save your hard drive's file structure as a tiny, self-contained HTML file — file names, dates, sizes, and folder trees — then backs it up to Google Drive and generates a QR code you can print and stick onto your physical drives. Scan the code, instantly see what's on that drive.

---

## Why This Exists

In August 2022, after returning from a trip to Armenia, Georgia, and Türkiye, I experienced the disaster of a "hard drive failure." That catastrophe resulted in the loss of many of my photos and memories, a severe emotional blow whose negative impact resonated throughout my life, goals, lifestyle, travel style, photography, relationships, and more. I won't go into the details of the recovery attempts and everything else, as that is a long story in itself.

Now, 4 years after that event, I've realized that not knowing which files you've actually lost is an even greater pain. So, as a computer and web developer, I created an application that allows you to save your hard drive's metadata as a small HTML file. Only the file names, dates, sizes, and folder structures are stored in this tiny file, which is then backed up to your personal Google Drive. It also generates a QR code that you can print and stick onto your physical hard drives. Whenever you scan it, you can instantly see what files are on that drive. This will greatly help you stay organized with your backups.

---

## What TagMyDrive Does

1. **Snapshots your drive** — Point the app at any folder and it generates an interactive HTML file with a treeview of your entire folder structure. File names, sizes, dates, everything — but no actual file content. The result is tiny.

2. **Uploads to Google Drive** — The snapshot HTML is automatically uploaded to your personal Google Drive via OAuth2. Your data stays in your own cloud.

3. **Generates QR codes** — A QR code linking to the Google Drive-hosted snapshot is generated on-the-fly. Print it, stick it on your physical hard drive. Scan it anytime to see what's on that drive without plugging it in.

4. **Manages your drives** — Name your drives, add descriptions, assign photos (paste any URL or search Unsplash). Keep a visual catalog of every drive you own.

5. **Web dashboard** — A companion web app lets you manage everything from a browser too — same database, same data, accessible from anywhere.

6. **Crypto payments for upgrades** — Membership tiers (Free / Basic / Premium) unlock more drives. Upgrades are handled via USDT BEP20 on the Binance Smart Chain, verified automatically through BSCScan.

---

## How It Was Built

### The Foundation: Snap2HTML

The core directory-scanning engine comes from [Snap2HTML](http://www.rlvision.com/snap2html) by RL Vision — an open-source tool that takes "snapshots" of folder structures and saves them as self-contained HTML files with a treeview navigator, search, and export. I customized and extended it heavily:

- **Multi-user system** — Snap2HTML is a single-user desktop tool. I added user registration, login, BCrypt password hashing, and session persistence so multiple people can use the platform.
- **Cloud storage integration** — After generating the HTML snapshot, the app uploads it to the user's Google Drive via OAuth2. Snap2HTML originally just saved locally.
- **QR code generation** — QR codes are generated on-the-fly from the stored Google Drive URL using QRCoder. No binary blobs stored in the database.
- **Membership & payment system** — Added a freemium model with disk limits and crypto-based payment verification via BSCScan API.
- **Disk management** — Users can create, rename, and organize virtual representations of their physical drives with custom names, descriptions, and photos.
- **HTML template modifications** — Customized the Snap2HTML template to support the new data flow and integration points.

The original Snap2HTML copyright (c) RL Vision 2011-2026 is preserved under GPL v3.

### Desktop App (C# / WinForms / .NET Framework 4.8)

The primary client. A Windows Forms application with a service-layer architecture:

| Service | Purpose |
|---|---|
| `AuthService` | Registration, login, password change, session persistence |
| `DiskService` | CRUD operations, membership limit enforcement |
| `QRCodeService` | On-the-fly QR generation from URL strings |
| `GoogleDriveService` | OAuth2 flow, file upload to user's Drive |
| `CryptoPaymentService` | USDT BEP20 verification via BSCScan REST API |
| `MembershipService` | Tier management, disk limit checks |
| `DatabaseService` | MySQL connection pooling, parameterized queries |

Session data is persisted at `%AppData%/TagMyDrive/session.dat` for automatic re-login on startup.

### Web Companion (Next.js 14 / TypeScript / Tailwind CSS)

A full web application that shares the same MySQL database as the desktop app:

- **Pages** — Landing page, login, register, dashboard, per-disk detail view, membership management
- **API routes** — Authentication, disk CRUD, QR code retrieval, Google Drive link management, Unsplash image search, user profile, membership upgrades
- **Auth** — JWT-based authentication (7-day tokens) with bcryptjs password hashing

### Database (MySQL)

Both apps connect to the same MySQL 5.7+ database with 8 tables:

`users` · `memberships` · `disks` · `qr_codes` · `google_drive_links` · `export_history` · `user_sessions` · `crypto_payments`

---

## Tech Stack

| Layer | Technology |
|---|---|
| Desktop | C#, WinForms, .NET Framework 4.8 |
| Web | Next.js 14, React 18, TypeScript, Tailwind CSS |
| Database | MySQL 5.7+ (MySqlConnector / mysql2) |
| Auth | BCrypt.Net-Next (desktop), bcryptjs (web), JWT (web sessions) |
| QR Codes | QRCoder (desktop), qrcode (web) |
| Google Drive | Google.Apis.Drive.v3 (OAuth2) |
| Crypto Payments | BSCScan REST API (USDT BEP20 on Binance Smart Chain) |
| Images | Unsplash API integration |

---

## Getting Started

### Prerequisites

- Windows with .NET Framework 4.8 (desktop app)
- Node.js 18+ (web app)
- MySQL 5.7+ or MariaDB

### 1. Database

```bash
mysql -h your-host -u your_user -p your_db < TagMyDrive-App/TagMyDrive/Database/schema.sql
```

### 2. Desktop App

```bash
cd TagMyDrive-App
cp .env.example .env
# Edit .env with your database, Google OAuth, and BSCScan credentials
```

Open `TagMyDrive.sln` in Visual Studio and build, or:

```bash
msbuild TagMyDrive.sln /p:Configuration=Release
```

### 3. Web App

```bash
cd TagMyDrive-Web
npm install
# Configure .env.local with your database credentials and JWT_SECRET
npm run dev
```

Open [http://localhost:3000](http://localhost:3000).

### Environment Variables

| Variable | Description |
|---|---|
| `TAGMYDRIVE_DB_CONNECTION` | Full MySQL connection string (desktop) |
| `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD` | MySQL connection (web) |
| `TAGMYDRIVE_GOOGLE_CLIENT_ID` | Google Cloud OAuth client ID |
| `TAGMYDRIVE_GOOGLE_CLIENT_SECRET` | Google Cloud OAuth client secret |
| `TAGMYDRIVE_CRYPTO_WALLET_USDT` | BEP20 wallet address for payments |
| `TAGMYDRIVE_BSCSCAN_API_KEY` | Free key from [bscscan.com/myapikey](https://bscscan.com/myapikey) |
| `JWT_SECRET` | Secret key for web app JWT tokens |

---

## Project Structure

```
TagMyDrive/
├── TagMyDrive-App/                  # Desktop application
│   ├── TagMyDrive/
│   │   ├── Services/                # Business logic layer
│   │   │   ├── AuthService.cs
│   │   │   ├── DiskService.cs
│   │   │   ├── QRCodeService.cs
│   │   │   ├── GoogleDriveService.cs
│   │   │   ├── CryptoPaymentService.cs
│   │   │   ├── MembershipService.cs
│   │   │   └── DatabaseService.cs
│   │   ├── Database/
│   │   │   └── schema.sql           # Full database schema
│   │   ├── frmMain.cs               # Main window + snapshot flow
│   │   ├── frmLogin.cs              # Login with remember-me
│   │   ├── frmRegister.cs           # User registration
│   │   ├── frmDiskManager.cs        # Disk management UI
│   │   ├── frmQRCodeManager.cs      # QR code management
│   │   ├── frmMembership.cs         # Crypto payment + upgrade
│   │   ├── AppConfig.cs             # Config with env var fallbacks
│   │   ├── template.html            # Customized Snap2HTML template
│   │   ├── ReadMe.txt               # Original Snap2HTML readme
│   │   └── RedeMeDev.txt            # Snap2HTML developer docs
│   ├── .env.example
│   └── TagMyDrive.sln
│
├── TagMyDrive-Web/                  # Web companion application
│   ├── app/
│   │   ├── api/                     # REST API routes
│   │   │   ├── auth/                # Login, register
│   │   │   ├── disks/               # Disk CRUD
│   │   │   ├── qr-codes/            # QR generation
│   │   │   ├── google-drive/        # Drive integration
│   │   │   ├── memberships/         # Tier management
│   │   │   ├── unsplash/            # Image search
│   │   │   └── user/                # Profile
│   │   ├── dashboard/               # Disk listing + detail views
│   │   ├── login/                   # Authentication pages
│   │   └── register/
│   ├── lib/                         # Auth + DB utilities
│   ├── package.json
│   └── .env.local
│
└── README.md                        # This file
```

---

## Membership Tiers

| Tier | Max Drives | Price |
|---|---|---|
| Free | 2 | $0 |
| Basic | 10 | $4.99/mo |
| Premium | Unlimited | $9.99/mo |

Payments are processed via USDT BEP20 cryptocurrency on the Binance Smart Chain and verified automatically through the BSCScan API (requires 12+ confirmations).

---

## License

This project is built on top of [Snap2HTML](http://www.rlvision.com/snap2html) by RL Vision. Original Snap2HTML copyright (c) RL Vision 2011-2026 is preserved. Licensed under GPL v3.
