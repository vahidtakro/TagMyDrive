# TagMyDrive Web Application

A Next.js web application for managing disks and QR codes...

## Features

- User authentication (login/register)
- Disk management
- QR code generation and tracking
- Google Drive integration
- Dashboard for viewing disks and QR codes
 
## Installation
 
1. Install dependencies:
```bash
npm install
```

2. Configure environment variables in `.env.local`:
```
DB_HOST=
DB_PORT=3306
DB_NAME=
DB_USER=
DB_PASSWORD=
JWT_SECRET=your-secret-key
```

3. Run development server:
```bash
npm run dev
```

4. Open [http://localhost:3000](http://localhost:3000) in your browser

## API Routes

### Authentication
- POST `/api/auth/login` - User login
- POST `/api/auth/register` - User registration

### Disks
- GET `/api/disks` - Get user's disks
- POST `/api/disks` - Create new disk

### QR Codes (to be implemented)
- GET `/api/qr-codes/:diskId` - Get disk's QR codes
- POST `/api/qr-codes` - Generate new QR code

## Shared Database

This web application connects to the same MySQL database as the Windows Forms desktop application, allowing users to manage their disks and QR codes from either platform.

## Technologies Used

- Next.js 14
- TypeScript
- Tailwind CSS
- Axios
- MySQL2
- JWT for authentication
- Bcrypt for password hashing
