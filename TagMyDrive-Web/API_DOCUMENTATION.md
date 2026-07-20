# TagMyDrive - API Documentation

## Base URL

- Desktop App: Direct database connection (MySQL)
- Web App: `http://localhost:3000/api`
- Production: `https://tagmydrive.yourdomain.com/api`

## Authentication

All protected endpoints require a JWT token in the `Authorization` header:

```
Authorization: Bearer <jwt_token>
```

JWT tokens are obtained via the `/api/auth/login` endpoint and are valid for 7 days.

## Error Responses

Standard error response format:

```json
{
  "message": "Error description",
  "status": 400
}
```

HTTP Status Codes:
- `200` - OK
- `201` - Created
- `400` - Bad Request
- `401` - Unauthorized (missing or invalid token)
- `404` - Not Found
- `500` - Internal Server Error

---

## Authentication Endpoints

### POST /api/auth/login

User login and token generation.

**Request Body:**
```json
{
  "username": "john_doe",
  "password": "password123"
}
```

**Success Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
	"id": 1,
	"username": "john_doe",
	"email": "john@example.com",
	"firstName": "John",
	"lastName": "Doe",
	"membershipId": 1
  }
}
```

**Error Response (401):**
```json
{
  "message": "Invalid username or password"
}
```

---

### POST /api/auth/register

User registration.

**Request Body:**
```json
{
  "username": "jane_smith",
  "email": "jane@example.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "password": "securepassword123"
}
```

**Success Response (201):**
```json
{
  "userId": 2,
  "message": "Registration successful"
}
```

**Error Response (400):**
```json
{
  "message": "Username or email already exists"
}
```

---

## Disk Endpoints

### GET /api/disks

Get all disks for the authenticated user.

**Authorization:** Required

**Query Parameters:** None

**Success Response (200):**
```json
{
  "disks": [
	{
	  "id": 1,
	  "userId": 1,
	  "name": "My Documents",
	  "description": "Work and personal documents",
	  "diskPath": "D:/Documents",
	  "diskType": "local",
	  "createdAt": "2024-01-15T10:30:00Z",
	  "updatedAt": "2024-01-15T10:30:00Z",
	  "isActive": true
	},
	{
	  "id": 2,
	  "userId": 1,
	  "name": "External Drive",
	  "description": "Backup drive",
	  "diskPath": "/mnt/backup",
	  "diskType": "external",
	  "createdAt": "2024-01-16T14:20:00Z",
	  "updatedAt": "2024-01-16T14:20:00Z",
	  "isActive": true
	}
  ]
}
```

---

### POST /api/disks

Create a new disk.

**Authorization:** Required

**Request Body:**
```json
{
  "name": "Archive",
  "description": "Old project files",
  "diskPath": "E:/Archive",
  "diskType": "external"
}
```

**Validation:**
- `name`: Required, max 255 characters
- `diskType`: One of: local, external, network

**Success Response (201):**
```json
{
  "message": "Disk created successfully"
}
```

**Error Response (400):**
```json
{
  "message": "Disk name is required"
}
```

---

### GET /api/disks/{id}

Get a specific disk.

**Authorization:** Required

**Path Parameters:**
- `id`: Disk ID

**Success Response (200):**
```json
{
  "disk": {
	"id": 1,
	"userId": 1,
	"name": "My Documents",
	"description": "Work and personal documents",
	"diskPath": "D:/Documents",
	"diskType": "local",
	"createdAt": "2024-01-15T10:30:00Z",
	"updatedAt": "2024-01-15T10:30:00Z",
	"isActive": true
  }
}
```

**Error Response (404):**
```json
{
  "message": "Disk not found"
}
```

---

### PUT /api/disks/{id}

Update a disk.

**Authorization:** Required

**Path Parameters:**
- `id`: Disk ID

**Request Body:**
```json
{
  "name": "My Documents (Updated)",
  "description": "Updated description"
}
```

**Success Response (200):**
```json
{
  "message": "Disk updated successfully"
}
```

---

### DELETE /api/disks/{id}

Delete a disk (soft delete).

**Authorization:** Required

**Path Parameters:**
- `id`: Disk ID

**Success Response (200):**
```json
{
  "message": "Disk deleted successfully"
}
```

---

## QR Code Endpoints

### GET /api/qr-codes/{diskId}

Get all QR codes for a disk.

**Authorization:** Required

**Path Parameters:**
- `diskId`: Disk ID

**Success Response (200):**
```json
{
  "qrCodes": [
	{
	  "id": 1,
	  "diskId": 1,
	  "qrCodeUrl": "https://tagmydrive.local/view/1",
	  "htmlExportFilename": "export.html",
	  "createdAt": "2024-01-15T11:00:00Z",
	  "isActive": true,
	  "downloadCount": 5
	}
  ]
}
```

---

### POST /api/qr-codes/{diskId}

Generate a new QR code for a disk.

**Authorization:** Required

**Path Parameters:**
- `diskId`: Disk ID

**Request Body:**
```json
{
  "htmlExportFilename": "export_2024-01-15.html",
  "qrCodeUrl": "https://tagmydrive.local/view/1"
}
```

**Success Response (201):**
```json
{
  "message": "QR code created successfully"
}
```

---

## Google Drive Endpoints

### GET /api/google-drive/{diskId}

Get all Google Drive links for a disk.

**Authorization:** Required

**Path Parameters:**
- `diskId`: Disk ID

**Success Response (200):**
```json
{
  "links": [
	{
	  "id": 1,
	  "diskId": 1,
	  "googleFileId": "1ABC123XYZ...",
	  "googleFileName": "export_2024-01-15.html",
	  "googleDriveUrl": "https://drive.google.com/file/d/1ABC123XYZ.../view",
	  "uploadedAt": "2024-01-15T12:00:00Z"
	}
  ]
}
```

---

### POST /api/google-drive/{diskId}

Save a Google Drive upload link.

**Authorization:** Required

**Path Parameters:**
- `diskId`: Disk ID

**Request Body:**
```json
{
  "googleFileId": "1ABC123XYZ...",
  "googleFileName": "export.html",
  "googleDriveUrl": "https://drive.google.com/file/d/1ABC123XYZ.../view",
  "htmlFileName": "export.html"
}
```

**Success Response (201):**
```json
{
  "message": "Google Drive link saved successfully"
}
```

---

## User Endpoints

### GET /api/user/profile

Get authenticated user's profile and membership info.

**Authorization:** Required

**Success Response (200):**
```json
{
  "user": {
	"id": 1,
	"username": "john_doe",
	"email": "john@example.com",
	"firstName": "John",
	"lastName": "Doe",
	"membershipId": 1,
	"membershipName": "Free",
	"maxDisks": 2,
	"diskCount": 1,
	"createdAt": "2024-01-15T10:00:00Z"
  }
}
```

---

### PUT /api/user/profile

Update user profile.

**Authorization:** Required

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe"
}
```

**Success Response (200):**
```json
{
  "message": "Profile updated successfully"
}
```

---

## Membership Endpoints

### GET /api/memberships

Get all available memberships.

**Authorization:** Not required

**Success Response (200):**
```json
{
  "memberships": [
	{
	  "id": 1,
	  "name": "Free",
	  "description": "Perfect for getting started",
	  "maxDisks": 2,
	  "priceMonthly": "0.00"
	},
	{
	  "id": 2,
	  "name": "Basic",
	  "description": "For power users",
	  "maxDisks": 10,
	  "priceMonthly": "4.99"
	},
	{
	  "id": 3,
	  "name": "Premium",
	  "description": "Unlimited everything",
	  "maxDisks": 999,
	  "priceMonthly": "9.99"
	}
  ]
}
```

---

## Rate Limiting

Currently not implemented. Recommended for production:
- 100 requests per minute per user
- 1000 requests per minute per IP

---

## Pagination

Currently not implemented. For future: Add `page` and `limit` query parameters to list endpoints.

Example:
```
GET /api/disks?page=1&limit=20
```

---

## Versioning

Current API version: **v1**

Future versions will use URL path:
```
/api/v1/...
/api/v2/...
```

---

## Data Types

### DateTime
ISO 8601 format: `2024-01-15T10:30:00Z`

### Boolean
JSON boolean: `true` or `false`

### Decimal
String representation for precision: `"4.99"` or float: `4.99`

---

## CORS (Cross-Origin Resource Sharing)

Configured for Next.js development. Update for production:

```typescript
// middleware.ts
export function middleware(request: NextRequest) {
  const response = NextResponse.next()

  response.headers.set('Access-Control-Allow-Origin', 'https://yourdomain.com')
  response.headers.set('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE')
  response.headers.set('Access-Control-Allow-Headers', 'Content-Type, Authorization')

  return response
}
```

---

## Webhooks

Not currently implemented. Future feature for events:
- User registered
- Disk created
- QR code generated
- Google Drive upload completed

---

## Changelog

### v1.0.0 (Initial Release)
- User authentication (login/register)
- Disk CRUD operations
- QR code generation endpoints
- Google Drive link tracking
- User profile management
- Membership info endpoints
