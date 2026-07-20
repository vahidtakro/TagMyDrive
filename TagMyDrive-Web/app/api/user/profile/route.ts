import { NextRequest, NextResponse } from 'next/server'
import { verifyToken } from '@/lib/auth'
import { query } from '@/lib/db'

// GET user profile
export async function GET(request: NextRequest) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json(
		{ message: 'Unauthorized' },
		{ status: 401 }
	  )
	}

	const userId = (tokenData as any).userId

	const results: any[] = await query(
	  'SELECT u.id, u.username, u.email, u.first_name, u.last_name, u.membership_id, u.created_at, m.name as membership_name, m.max_disks FROM users u LEFT JOIN memberships m ON u.membership_id = m.id WHERE u.id = ?',
	  [userId]
	)

	if (results.length === 0) {
	  return NextResponse.json(
		{ message: 'User not found' },
		{ status: 404 }
	  )
	}

	const user = results[0]

	// Get disk count
	const diskCountResults: any[] = await query(
	  'SELECT COUNT(*) as count FROM disks WHERE user_id = ? AND is_active = TRUE',
	  [userId]
	)

	const diskCount = diskCountResults[0].count

	return NextResponse.json(
	  {
		user: {
		  id: user.id,
		  username: user.username,
		  email: user.email,
		  firstName: user.first_name,
		  lastName: user.last_name,
		  membershipId: user.membership_id,
		  membershipName: user.membership_name,
		  maxDisks: user.max_disks,
		  diskCount: diskCount,
		  createdAt: user.created_at,
		},
	  },
	  { status: 200 }
	)
  } catch (error: any) {
	return NextResponse.json(
	  { message: 'Error: ' + error.message },
	  { status: 500 }
	)
  }
}

// UPDATE user profile
export async function PUT(request: NextRequest) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json(
		{ message: 'Unauthorized' },
		{ status: 401 }
	  )
	}

	const userId = (tokenData as any).userId
	const { firstName, lastName } = await request.json()

	await query(
	  'UPDATE users SET first_name = ?, last_name = ? WHERE id = ?',
	  [firstName || '', lastName || '', userId]
	)

	return NextResponse.json(
	  { message: 'Profile updated successfully' },
	  { status: 200 }
	)
  } catch (error: any) {
	return NextResponse.json(
	  { message: 'Error: ' + error.message },
	  { status: 500 }
	)
  }
}
