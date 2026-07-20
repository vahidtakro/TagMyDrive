# TagMyDrive - Deployment & Testing Checklist

## Pre-Deployment

### Secrets & Configuration

- [ ] Copy `.env.example` to `.env` and fill in all values
- [ ] **Or** fill in `app.config` directly (env vars override app.config)
- [ ] Verify `TAGMYDRIVE_DB_CONNECTION` connects successfully
- [ ] Verify Google OAuth client ID + secret are valid (console.cloud.google.com)
- [ ] Verify `TAGMYDRIVE_BSCSCAN_API_KEY` is active (bscscan.com/myapikey)
- [ ] Set `TAGMYDRIVE_CRYPTO_WALLET_USDT` to your BEP20 receiving address

### Database

- [ ] Create database on MySQL host
- [ ] Run `TagMyDrive/Database/schema.sql`
- [ ] Verify default memberships inserted (Free, Basic, Premium)
- [ ] Verify `crypto_payments` table has `confirmations` column
- [ ] Verify `qr_codes` table has no `qr_code_data` LONGBLOB column
- [ ] Verify `disks` table has `image_url VARCHAR(2048)` column (run migration SQL if migrating from LONGBLOB)

### NuGet Packages

- [ ] MySqlConnector 2.3.0
- [ ] QRCoder 1.4.3
- [ ] Google.Apis.Drive.v3 1.67.0
- [ ] Google.Apis.Auth 1.67.0
- [ ] BCrypt.Net-Next 4.0.3
- [ ] System.Memory (binding redirect to 4.0.1.2)

### Build

- [ ] Build solution in Release — 0 errors, 0 warnings
- [ ] `app.config` copied to output directory

## Testing

### Authentication

- [ ] Register new user — BCrypt hash stored, Free membership assigned
- [ ] Login with correct credentials — session saved if "Remember Me" checked
- [ ] Login with wrong password — error shown, no data leaked
- [ ] Change password — old password stops working
- [ ] Logout — session cleared, app restarts cleanly
- [ ] Close and reopen with "Remember Me" — auto-login without credentials

### Disk Management

- [ ] Add a disk — appears in list
- [ ] Add disk with image URL — image preview shown in list
- [ ] Add 2nd disk (Free tier) — allowed
- [ ] Add 3rd disk (Free tier) — blocked with limit message
- [ ] Edit disk name and description — changes reflected in list
- [ ] Search Unsplash for disk photo — results displayed, select one
- [ ] Change or remove disk photo — image updated
- [ ] Soft-delete disk — removed from list, data preserved

### Snapshot & Google Drive

- [ ] Select a disk and run snapshot on a folder
- [ ] HTML file generated in temp folder
- [ ] Auto-upload to Google Drive (pre-authenticated on login)
- [ ] Temp folder cleaned up after upload
- [ ] GDrive link appears in disk manager
- [ ] When disk limit reached — snapshot blocked, no upload attempted

### QR Codes

- [ ] Generate QR for disk with GDrive link — QR preview shown in disk manager
- [ ] Disks tab — QR preview + disk info + clickable GDrive link
- [ ] QR Manager — generate, list, download (on-the-fly PNG)
- [ ] Deactivate old QR codes on snapshot re-run

### Crypto Payments

- [ ] Open Membership form — shows USDT BEP20 wallet address
- [ ] Submit a valid tx hash — BSCScan verifies (12+ confirmations, correct recipient/amount)
- [ ] Invalid tx hash — error shown
- [ ] On successful verification — membership upgraded immediately
- [ ] Disk limit reflects new tier

### Error Handling

- [ ] Database offline — graceful error, app doesn't crash
- [ ] Google Drive token expired — re-auth triggered
- [ ] BSCScan API down — error message shown, no upgrade applied
- [ ] Network timeout — user can retry

## Post-Deployment

- [ ] Confirm `.env` is in `.gitignore` (never committed)
- [ ] Confirm `session.dat` is in `.gitignore`
- [ ] Test on a clean machine (no leftover config)
- [ ] Monitor database connections under load
